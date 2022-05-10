using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public class AnswerRepository : Repository<Answer>, IAnswerRepository
    {
        private readonly Guid discardChoiceId = Program.DiscardChoiceId;
        public AnswerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Answer> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Answer>().FirstOrDefaultAsync(p => p.Id == id);
        }
        public bool AnsweredCard(Guid cardId, Guid userProfileId)
        {
            return DbContext.Answers.Where(answer => answer.CardId == cardId && answer.AnsweredById == userProfileId).Any();
        }

        public Guid GetChoiceMoodId(string name)
        {
            var choicesMoodCard = DbContext.Set<Card>().Where(c => c.Id == Program.MoodId).Select(c => c.Choices).FirstOrDefault();

            if (choicesMoodCard == null)
                return Guid.Empty;

            return choicesMoodCard.Where(c => c.Name == name).Select(c => c.Id).FirstOrDefault();
        }

        public async Task AddAnswer(Answer answer)
        {
            try
            {
                var card = DbContext.Set<Card>()
                    .Where(c => c.Id == answer.CardId)
                    .Include(c => c.CardGroup)
                    .FirstOrDefault();

                var membersNumber = 0;
                if (card.Type == CardTypes.Event.ToString())
                {
                    var newList = new List<Guid>();
                    foreach(var group in card.CardGroup)
                    {
                        var users = await DbContext.Set<Group>()
                            .Where(g => g.Id == group.GroupId)
                            .Select(g => g.GroupUser)
                            .FirstOrDefaultAsync();

                        foreach(var user in users)
                        {
                            if (!(newList.Exists(userId => userId == user.UserId)))
                                newList.Add(user.UserId);
                        }
                    }
                    membersNumber = newList.Count;
                }

                var report = await DbContext.Set<Reporting>()
                    .Where(c => card.Type == CardTypes.Reporting.ToString() ? c.Id == answer.CardId : c.LinkedCardId == answer.CardId)
                    .Include(c => c.Results)
                    .FirstOrDefaultAsync();

                if (DbContext.Set<Answer>().Where(a => a.AnsweredById == answer.AnsweredById && a.CardId == answer.CardId).Count() == 1)
                    report.Answers++;

                var answerNbr = DbContext.Set<Answer>()
                    .Where(a => a.CardId == answer.CardId && a.ChoiceId != discardChoiceId)
                    .Count();

                for (int i = 0; i < report.Results.Count; i++)
                {
                    var result = report.Results.ElementAt(i);
                    int choiceNbr = DbContext.Answers
                        .Where(a => a.ChoiceId == result.ChoiceId)
                        .Count();

                    var value = card.Type == CardTypes.Event.ToString() ? (membersNumber == 0 ? 0 : (double)answerNbr / membersNumber) : (answerNbr == 0 ? 0 : (double)choiceNbr / answerNbr);
                    result.Value = (int)(value * 100);
                }

                await DbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //log exception
                throw;
            }
        }

        public async Task DeleteAnswer(Guid cardId, Guid userProfileId)
        {
            var answer = DbContext.Set<Answer>().Where(a => a.CardId == cardId && a.AnsweredById == userProfileId).FirstOrDefault();
            if (answer != null)
            {
                DbContext.Set<Answer>().RemoveRange(answer);
                await DbContext.SaveChangesAsync();
            }
                
        }
    }
}
