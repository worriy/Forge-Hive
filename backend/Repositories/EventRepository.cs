using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public partial class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Event> GetAllWithReferences()
        {
            return DbContext.Set<Event>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).AsNoTracking();
        }

        public async Task<Event> GetByIdWithReferences(Guid id)
        {
            var card = await DbContext.Set<Card>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup).Include(m => m.CardGroup)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (card != null)
            {
                if (card.Type == CardTypes.Reporting.ToString())
                    return await DbContext.Set<Event>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup).Include(m => m.CardGroup)
                    .FirstOrDefaultAsync(p => p.LinkedCardId == id);
                else return await DbContext.Set<Event>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup).Include(m => m.CardGroup)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }

            return await DbContext.Set<Event>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup).Include(m => m.CardGroup)
                .FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task UpdateEvent(Event oldEntity, Event entity)
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
            oldEntity.PublicationDate = report.PublicationDate;
            oldEntity.EndDate = oldEntity.EndDate.Date == entity.EndDate.Date ? oldEntity.EndDate : entity.EndDate.Date.AddHours(23);

            //keep the old targetted groups
            var cardGroups = new List<CardGroup>();
            cardGroups.AddRange(oldEntity.CardGroup);

            //tests on those targetted groups
            foreach (CardGroup gr in cardGroups)
            {
                //if this group is also targetted by the new entity, do not change it
                if (entity.CardGroup.Exists(g => g.GroupId == gr.GroupId))
                {
                    var cardGroup = DbContext.Set<CardGroup>().AsNoTracking().Where(g => g.GroupId == gr.GroupId).Where(g => g.CardId == oldEntity.Id).FirstOrDefault();
                    var reportGroup = DbContext.Set<CardGroup>().AsNoTracking().Where(g => g.GroupId == gr.GroupId).Where(g => g.CardId == report.Id).FirstOrDefault();
                }
                //if it's not, remove it because it's not targeted anymore 
                else
                {
                    var group = DbContext.Groups.Find(gr.GroupId);
                    DbContext.Groups.Attach(group);
                    var cardGroup = DbContext.Set<CardGroup>().Where(g => g.GroupId == gr.GroupId).Where(g => g.CardId == oldEntity.Id).FirstOrDefault();
                    DbContext.Set<CardGroup>().Remove(cardGroup);
                    oldEntity.CardGroup.Remove(gr);
                    var reportGroup = DbContext.Set<CardGroup>().Where(g => g.GroupId == gr.GroupId).Where(g => g.CardId == report.Id).FirstOrDefault();
                    report.CardGroup.Remove(reportGroup);
                }

            }
            await DbContext.SaveChangesAsync();

            //now tests on the new entity's target groups
            foreach (CardGroup gr in entity.CardGroup)
            {
                if (!oldEntity.CardGroup.Exists(g => g.GroupId == gr.GroupId))
                {
                    var group = DbContext.Groups.Find(gr.GroupId);
                    DbContext.Groups.Attach(group);
                    var groupCard = new CardGroup
                    {
                        CardId = oldEntity.Id,
                        GroupId = gr.GroupId
                    };
                    var groupReport = new CardGroup
                    {
                        CardId = report.Id,
                        GroupId = gr.GroupId
                    };
                    report.CardGroup.Add(groupReport);
                    oldEntity.CardGroup.Add(groupCard);
                    DbContext.Set<CardGroup>().Add(groupReport);
                    DbContext.Set<CardGroup>().Add(groupCard);
                }
            }
            


            await DbContext.SaveChangesAsync();

        }

        public async Task<Event> InsertEvent(Event ev)
        {
            if (ev == null)
            {
                throw new ArgumentNullException(nameof(ev));
            }

            //Attach creator to DbCOntext to avoid creation of a new user
            if (ev.CreatedById != Guid.Empty)
            {
                var createdBy = DbContext.UserProfiles.Find(ev.CreatedById);
                DbContext.UserProfiles.Attach(createdBy);
            }

            //Attach Picture to avoid creation of a new picture
            if (ev.PictureId == Guid.Empty)
                ev.PictureId = Program.EventDefaultPicId;
            var picture = DbContext.Pictures.Find(ev.PictureId);
            DbContext.Pictures.Attach(picture);

            //get all groups referenced by the event (targetGroups)
            List<Group> groups = new List<Group>();
            foreach (CardGroup item in ev.CardGroup)
                groups.Add(DbContext.Groups.Find(item.GroupId));

            //Attach them all to avoid creation of groups
            foreach (Group gr in groups)
                DbContext.Groups.Attach(gr);

            ev.PublicationDate = DateTime.Now < ev.PublicationDate ? ev.PublicationDate.Date.AddHours(1) : ev.PublicationDate;
            ev.EndDate = ev.EndDate.Date.AddHours(23);

            //create the associated reporting
            var Reporting = new Reporting
            {
                Views = 0,
                Answers = 0,
                CreatedById = ev.CreatedById,
                Content = ev.Content,
                Type = CardTypes.Reporting.ToString(),
                PublicationDate = ev.PublicationDate,
                PictureId = ev.PictureId,
                EndDate = ev.EndDate.AddDays(1)
            };

            Reporting.Choices = new HashSet<Choice>();
            Reporting.Choices = ev.Choices;

            DbContext.Cards.Add(ev);
            DbContext.Cards.Add(Reporting);

            if (await DbContext.SaveChangesAsync() != 0)
            {
                Reporting.LinkedCardId = ev.Id;
                ev.LinkedCardId = Reporting.Id;

                //create the associated results
                var Results = new Result[ev.Choices.Count];
                for (var i = 0; i < ev.Choices.Count; i++)
                {
                    var result = new Result
                    {
                        ReportingId = Reporting.Id,
                        Value = 0,
                        Choice = ev.Choices.ElementAt(i)
                    };

                    Results[i] = result;

                }

                Reporting.Results = new HashSet<Result>();
                foreach (Result r in Results)
                    Reporting.Results.Add(r);

                Reporting.CardGroup = new List<CardGroup>();
                //create a row in the join table for each group referenced by the entity
                foreach (Group gr in groups)
                {
                    var cardGroupReporting = new CardGroup
                    {
                        Card = Reporting,
                        CardId = Reporting.Id,
                        Group = gr,
                        GroupId = gr.Id
                    };
                    //Add the row in the Card to keep navigation references
                    Reporting.CardGroup.Add(cardGroupReporting);
                    //Add the row to the context before saving
                    DbContext.Set<CardGroup>().Add(cardGroupReporting);
                }
                await DbContext.SaveChangesAsync();

            }
            return ev;
        }

        public async Task<EditableEventVM> GetEditableEvent(Guid idEvent)
        {

            var result = await DbContext.Set<Card>().Include(ev => ev.Choices)
                .Where(ev => ev.Id == idEvent)
                .Select(ev => new EditableEventVM(
                    ev.Id.ToString(), ev.Content, ev.PublicationDate,
                    ev.EndDate, null, ev.Picture.PicBase64, ev.Choices
                ))
                .FirstOrDefaultAsync();

            result.TargetGroupsIds = new HashSet<string>();
            var groups = await DbContext.Cards.Where(c => c.Id == idEvent)
                .Select(c => c.CardGroup)
                .FirstOrDefaultAsync();

            groups.ForEach(cg =>
            {
                result.TargetGroupsIds.Add(cg.GroupId.ToString());
            });

            return result;
        }
    }
}
