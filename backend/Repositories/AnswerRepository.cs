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
        public AnswerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Answer> GetAllWithReferences()
        {
            return DbContext.Set<Answer>().Include(m => m.AnsweredBy).Include(m => m.Card).Include(m => m.Choice).AsNoTracking();
        }

        public async Task<Answer> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Answer>().Include(m => m.AnsweredBy).Include(m => m.Card).Include(m => m.Choice).FirstOrDefaultAsync(p => p.Id == id);
        }
        public bool AnsweredCard(Guid cardId, Guid userProfileId)
        {
            return DbContext.Answers.Where(answer => answer.CardId == cardId && answer.AnsweredById == userProfileId).Any();
        }

        public Guid GetChoiceMoodId(string name)
        {
            var choice = DbContext.Set<Choice>().Where(c => c.Name == name).FirstOrDefault();
            var choicesCard = DbContext.Set<Card>().Where(c => c.Id == Program.MoodId).Select(c => c.Choices).FirstOrDefault();
            if(choicesCard != null)
            {
                foreach (var ch in choicesCard)
                {
                    if (ch.Id == choice.Id)
                        return choice.Id;
                    
                }
            }
            return Guid.Empty;
            
        }

        public async Task DeleteAnswer(Guid cardId, Guid userProfileId)
        {
            var typeCard = DbContext.Set<Card>().Where(c => c.Id == cardId).Select(c => c.Type).FirstOrDefault();
            // if Type Card is Survey ==> Dlete the answer of his Question too
            if(typeCard == CardTypes.Survey.ToString())
            {
                var Ids = DbContext.Set<Question>().Where(q => q.SurveyId == cardId).Select(q => q.Id).ToList();
                if(Ids != null)
                {
                    foreach(var idQuestionSurvey in Ids)
                    {
                        var answerQuestionSurvey = DbContext.Set<Answer>().Where(a => a.CardId == idQuestionSurvey && a.AnsweredById == userProfileId).FirstOrDefault();
                        if(answerQuestionSurvey != null)
                        {
                            DbContext.Set<Answer>().RemoveRange(answerQuestionSurvey);
                            await DbContext.SaveChangesAsync();
                        }
                    }
                }
            }

            var answer = DbContext.Set<Answer>().Where(a => a.CardId == cardId && a.AnsweredById == userProfileId).FirstOrDefault();
            if (answer != null)
            {
                DbContext.Set<Answer>().RemoveRange(answer);
                await DbContext.SaveChangesAsync();
            }
                
        }
    }
}
