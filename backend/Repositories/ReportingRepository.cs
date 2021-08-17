using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public class ReportingRepository : Repository<Reporting>, IReportingRepository
    {
        private readonly Guid discardChoiceId = Program.DiscardChoiceId;

        public ReportingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Reporting> GetAllWithReferences()
        {
            return DbContext.Set<Reporting>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.Results).AsNoTracking();
        }

        public async Task<Reporting> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Reporting>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.Results).FirstOrDefaultAsync(p => p.Id == id);
        }
        /// <summary>
        /// Fills Results and choices of the Reporting card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Reporting FillInfos(Reporting card)
        {
            var choices = DbContext.Set<Card>()
                .Include(a => a.Choices)
                .Where(c => c.Id == card.LinkedCardId)
                .Select(c => c.Choices)
                .FirstOrDefault();

            foreach (Choice c in choices)
            {
                card.Choices.Add(c);
            }

            var results = DbContext.Set<Result>()
                .Where(r => r.ReportingId == card.Id)
                .Select(r => new { r.Id, r.ReportingId, r.Choice, r.Value })
                .ToList();

            card.Results = new HashSet<Result>();
            foreach (var r in results)
            {
                card.Results.Add(new Result
                {
                    Id = r.Id,
                    Value = r.Value,
                    ReportingId = r.ReportingId,
                    Choice = r.Choice
                });
            }
            return card;
        }

        /// <summary>
        /// Update Reporting and Results when a user answers a card
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public async Task AddAnswer(Answer answer)
        {
            try {
                var card = DbContext.Set<Card>()
                        .Where(c => c.Id == answer.CardId)
                        .Include(c => c.CardGroup)
                        .FirstOrDefault();
                int membersNumber = 0;
                if (card.Type == "Event")
                {
                    var newList = new List<Guid>();
                    card.CardGroup.ForEach(group =>
                    {
                        var membersList = DbContext.Set<Group>()
                            .Where(g => g.Id == group.GroupId)
                            .Select(g => g.GroupUser)
                            .FirstOrDefault()
                            .ToList();

                        membersList.ForEach(member =>
                        {

                            if (!(newList.Exists(userId => userId == member.UserId)))
                                newList.Add(member.UserId);
                        });
                        
                    });
                    membersNumber = newList.Count;
                }

                Reporting report;
                if(card.Type == "Reporting")
                {
                    report = DbContext.Set<Reporting>()
                        .Where(r => r.Id == answer.CardId)
                        .FirstOrDefault();
                }
                else
                {
                    report = DbContext.Set<Reporting>()
                        .Where(r => r.LinkedCardId == answer.CardId)
                        .FirstOrDefault();
                }

                report = await this.GetByIdWithReferences(report.Id);
                if (DbContext.Set<Answer>().Where(a => a.AnsweredById == answer.AnsweredById && a.CardId == answer.CardId).Count() == 1)
                {
                    report.Answers++;
                }
            
            
                var choices = DbContext.Set<Card>()
                    .Include(a => a.Choices)
                    .Where(c => c.Id == report.LinkedCardId)
                    .Select(c => c.Choices)
                    .FirstOrDefault();

                    foreach (Choice c in choices)
                {
                    report.Choices.Add(c);
                }

                int answerNbr = DbContext.Set<Answer>()
                    .Where(a => a.CardId == answer.CardId & a.ChoiceId != discardChoiceId)
                    .Count();

                for (int i = 0; i < report.Results.Count; i++)
                {
                    var result = report.Results.ElementAt(i);
                    int choiceNbr = DbContext.Answers
                    .Where(a => a.ChoiceId == result.Choice.Id)
                    .Count();

                    if (card.Type == "Event")
                    {
                        double division = membersNumber == 0 ? 0 : (double)answerNbr / membersNumber;
                        result.Value = (int)(division * 100);
                    }
                    else
                    {
                        double division = answerNbr == 0 ? 0 : (double)choiceNbr / answerNbr;
                        result.Value = (int)(division * 100);
                    }

                    DbContext.Entry(result).State = EntityState.Modified;
                }

                var lenght = report.Choices.Count;
                for (int i = 0; i < lenght; i++)
                {
                    var choice = report.Choices.ElementAt(0);
                    DbContext.Entry(choice).State = EntityState.Unchanged;
                }

                await DbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //log exception
                throw;
            }
        }
    }
}
