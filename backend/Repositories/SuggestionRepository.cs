using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.Repositories;
using Hive.Backend.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public class SuggestionRepository : Repository<Suggestion>, ISuggestionRepository
    {
        public SuggestionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Suggestion> GetAllWithReferences()
        {
            return DbContext.Set<Suggestion>().Include(m => m.CreatedBy).Include(m => m.Picture).AsNoTracking();
        }

        public async Task<Suggestion> GetByIdWithReferences(Guid id)
        {
            var card = await DbContext.Set<Card>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.CardGroup).Include(m => m.CardGroup)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (card != null)
            {
                if (card.Type == CardTypes.Reporting.ToString())
                    return await DbContext.Set<Suggestion>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.CardGroup).Include(m => m.CardGroup)
                    .FirstOrDefaultAsync(p => p.LinkedCardId == id);
                else return await DbContext.Set<Suggestion>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.CardGroup).Include(m => m.CardGroup)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }

            return await DbContext.Set<Suggestion>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.CardGroup).Include(m => m.CardGroup)
                .FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task UpdateSuggestion(Suggestion oldEntity, Suggestion entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            //if the picture changed
            if (entity.PictureId != Guid.Empty)
            {
                //if the picture changed, remove the old one from the database
                var oldCardPictureId = oldEntity.PictureId;
                if (oldCardPictureId != Program.EventDefaultPicId)
                    DbContext.Pictures.Remove(DbContext.Pictures.Find(oldCardPictureId));
                oldEntity.PictureId = entity.PictureId;
            }



            await DbContext.SaveChangesAsync();

            //get the reporting of the concerned card
            var report = DbContext.Set<Reporting>().Include(r => r.Choices)
                .Where(r => r.LinkedCardId == oldEntity.Id)
                .FirstOrDefault();

            //update the report with new infos
            report.PictureId = oldEntity.PictureId;
            report.Content = entity.Content;
            report.PublicationDate = oldEntity.PublicationDate.Date == entity.PublicationDate.Date ? oldEntity.PublicationDate : entity.PublicationDate.Date.AddHours(1);
            report.EndDate = oldEntity.EndDate.Date == entity.EndDate.Date ? report.EndDate : entity.EndDate.Date.AddDays(1).AddHours(23);

            //update the card itself
            oldEntity.Content = entity.Content;
            oldEntity.PublicationDate = oldEntity.PublicationDate.Date == entity.PublicationDate.Date ? oldEntity.PublicationDate : entity.PublicationDate.Date.AddHours(1);
            oldEntity.EndDate = oldEntity.EndDate.Date == entity.EndDate.Date ? oldEntity.EndDate : entity.EndDate.Date.AddHours(23);

            //keep the old targetted groups
        /*  var cardGroups = new List<CardGroup>();
            cardGroups.AddRange(oldEntity.cardGroup);*/

            //tests on those targetted groups
     /*     foreach (CardGroup gr in cardGroups)
            {
                //if this group is also targetted by the new entity, do not change it
                if (entity.cardGroup.Exists(g => g.groupId == gr.groupId))
                {
                    var cardGroup = DbContext.Set<CardGroup>().AsNoTracking().Where(g => g.groupId == gr.groupId).Where(g => g.cardId == oldEntity.Id).FirstOrDefault();
                    var reportGroup = DbContext.Set<CardGroup>().AsNoTracking().Where(g => g.groupId == gr.groupId).Where(g => g.cardId == report.Id).FirstOrDefault();
                }
                //if it's not, remove it because it's not targeted anymore 
                else
                {
                    var group = DbContext.Groups.Find(gr.groupId);
                    DbContext.Groups.Attach(group);
                    var cardGroup = DbContext.Set<CardGroup>().Where(g => g.groupId == gr.groupId).Where(g => g.cardId == oldEntity.Id).FirstOrDefault();
                    DbContext.Set<CardGroup>().Remove(cardGroup);
                    oldEntity.cardGroup.Remove(gr);
                    var reportGroup = DbContext.Set<CardGroup>().Where(g => g.groupId == gr.groupId).Where(g => g.cardId == report.Id).FirstOrDefault();
                    report.cardGroup.Remove(reportGroup);
                }

            }
            await DbContext.SaveChangesAsync();

            //now tests on the new entity's target groups
            foreach (CardGroup gr in entity.cardGroup)
            {
                if (!oldEntity.cardGroup.Exists(g => g.groupId == gr.groupId))
                {
                    var group = DbContext.Groups.Find(gr.groupId);
                    DbContext.Groups.Attach(group);
                    var groupCard = new CardGroup
                    {
                        cardId = oldEntity.Id,
                        groupId = gr.groupId
                    };
                    var groupReport = new CardGroup
                    {
                        cardId = report.Id,
                        groupId = gr.groupId
                    };
                    report.cardGroup.Add(groupReport);
                    oldEntity.cardGroup.Add(groupCard);
                    DbContext.Set<CardGroup>().Add(groupReport);
                    DbContext.Set<CardGroup>().Add(groupCard);
                }
            }*/



            await DbContext.SaveChangesAsync();

        }

        public async Task<Suggestion> InsertSuggestion(Suggestion suggestion)
        {
            if (suggestion == null)
            {
                throw new ArgumentNullException(nameof(suggestion));
            }

            //Attach creator to DbCOntext to avoid creation of a new user
            if (suggestion.CreatedById != Guid.Empty)
            {
                var createdBy = DbContext.UserProfiles.Find(suggestion.CreatedById);
                DbContext.UserProfiles.Attach(createdBy);
            }

            //Attach Picture to avoid creation of a new picture
            if (suggestion.PictureId == Guid.Empty)
                suggestion.PictureId = Program.SuggestionDefaultPicId;
            var picture = DbContext.Pictures.Find(suggestion.PictureId);
            DbContext.Pictures.Attach(picture);

            //get all groups referenced by the event (targetGroups)
            List<Group> groups = new List<Group>
            {
                DbContext.Groups.Find(Program.PublicGroupId)
            };

            //Attach them all to avoid creation of groups
            foreach (Group gr in groups)
                DbContext.Groups.Attach(gr);

            suggestion.PublicationDate = DateTime.Now < suggestion.PublicationDate ? suggestion.PublicationDate.Date.AddHours(1) : suggestion.PublicationDate;
            suggestion.EndDate = suggestion.EndDate.Date.AddHours(23);

            //create the associated reporting
            Reporting Reporting = new Reporting
            {
                Views = 0,
                Answers = 0,
                CreatedById = suggestion.CreatedById,
                Content = suggestion.Content,
                Type = CardTypes.Reporting.ToString(),
                PublicationDate = suggestion.PublicationDate,
                PictureId = suggestion.PictureId,
                EndDate = suggestion.EndDate.AddDays(1)
            };

            /*Reporting.Choices = new HashSet<Choice>();
            Reporting.Choices = suggestion.Choices;*/

            DbContext.Cards.Add(suggestion);
            DbContext.Cards.Add(Reporting);
            await DbContext.SaveChangesAsync();
            Reporting.LinkedCardId = suggestion.Id;
            suggestion.LinkedCardId = Reporting.Id;
            /*if (await DbContext.SaveChangesAsync() != 0)
            {
                Reporting.LinkedCardId = suggestion.Id;
                suggestion.LinkedCardId = Reporting.Id;

                //create the associated results
                var Results = new Result[suggestion.Choices.Count()];
                for (var i = 0; i < suggestion.Choices.Count(); i++)
                {
                    var result = new Result
                    {
                        ReportingId = Reporting.Id,
                        Value = 0,
                        Choice = suggestion.Choices.ElementAt(i)
                    };

                    Results[i] = result;

                }

                Reporting.Results = new HashSet<Result>();
                foreach (Result r in Results)
                    Reporting.Results.Add(r);

                Reporting.cardGroup = new List<CardGroup>();
                //create a row in the join table for each group referenced by the entity
                foreach (Group gr in groups)
                {
                    var cardGroupReporting = new CardGroup
                    {
                        card = Reporting,
                        cardId = Reporting.Id,
                        group = gr,
                        groupId = gr.Id
                    };
                    //Add the row in the Card to keep navigation references
                    Reporting.cardGroup.Add(cardGroupReporting);
                    //Add the row to the context before saving
                    DbContext.Set<CardGroup>().Add(cardGroupReporting);
                }*/

            suggestion.CardGroup = new List<CardGroup>
            {
                new CardGroup
                {
                    Card = suggestion,
                    CardId = suggestion.Id,
                    Group = DbContext.Set<Group>().Where(g => g.Id == Program.PublicGroupId).FirstOrDefault(),
                    GroupId = Program.PublicGroupId
                }
            };
            await DbContext.SaveChangesAsync();
            
            return suggestion;
        }

        public async Task<EditableSuggestionVM> GetEditableSuggestion(Guid idSuggestion)
        {

            var result = await DbContext.Set<Card>().Include(ev => ev.Choices)
                .Where(ev => ev.Id == idSuggestion)
                .Select(ev => new EditableSuggestionVM(
                    ev.Id.ToString(), ev.Content, ev.PublicationDate,
                    ev.EndDate, ev.Picture.PicBase64
                ))
                .FirstOrDefaultAsync();
            
            return result;
        }
    }
}
