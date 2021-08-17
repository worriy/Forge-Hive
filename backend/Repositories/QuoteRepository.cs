using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public class QuoteRepository : Repository<Quote>, IQuoteRepository
    {
        public QuoteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Quote> GetAllWithReferences()
        {
            return DbContext.Set<Quote>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).AsNoTracking();
        }

        public async Task<Quote> GetByIdWithReferences(Guid id)
        {
            var card = await DbContext.Set<Card>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup).Include(m => m.CardGroup)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (card != null)
            {
                if (card.Type == CardTypes.Reporting.ToString())
                    return await DbContext.Set<Quote>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup).Include(m => m.CardGroup)
                    .FirstOrDefaultAsync(p => p.LinkedCardId == id);
                else return await DbContext.Set<Quote>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup).Include(m => m.CardGroup)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }

            return await DbContext.Set<Quote>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup).Include(m => m.CardGroup)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateQuote(Quote oldEntity, Quote entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            //if the picture changed
            /*if (entity.PictureId != 0)
            {
                //if the picture changed, remove the old one from the database
                var oldCardPictureId = oldEntity.PictureId;
                if (oldCardPictureId != Program.eventDefaultPicId)
                    DbContext.Pictures.Remove(DbContext.Pictures.Find(oldCardPictureId));
                oldEntity.PictureId = entity.PictureId;
            }*/



            //await DbContext.SaveChangesAsync();

            //get the reporting of the concerned card
            var report = DbContext.Set<Reporting>().Include(r => r.Choices)
                .Where(r => r.LinkedCardId == oldEntity.Id)
                .FirstOrDefault();

            //update the report with new infos
            //report.PictureId = oldEntity.PictureId;
            report.Content = entity.Content;
            report.PublicationDate = oldEntity.PublicationDate.Date == entity.PublicationDate.Date ? oldEntity.PublicationDate : entity.PublicationDate.Date.AddHours(1);
            report.EndDate = oldEntity.EndDate.Date == entity.EndDate.Date ? report.EndDate : entity.EndDate.Date.AddDays(1).AddHours(23);

            //update the card itself
            oldEntity.Content = entity.Content;
            oldEntity.PublicationDate = oldEntity.PublicationDate.Date == entity.PublicationDate.Date ? oldEntity.PublicationDate : entity.PublicationDate.Date.AddHours(1);
            oldEntity.EndDate = oldEntity.EndDate.Date == entity.EndDate.Date ? oldEntity.EndDate : entity.EndDate.Date.AddHours(23);

            //keep the old targetted groups
            /*var cardGroups = new List<CardGroup>();
            cardGroups.AddRange(oldEntity.cardGroup);*/

            //tests on those targetted groups
            /*foreach (CardGroup gr in cardGroups)
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
            await DbContext.SaveChangesAsync();*/

            //now tests on the new entity's target groups
            /*foreach (CardGroup gr in entity.cardGroup)
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

        public async Task<Quote> InsertQuote(Quote quote)
        {
            if (quote == null)
            {
                throw new ArgumentNullException(nameof(quote));
            }

            //Attach creator to DbCOntext to avoid creation of a new user
            if (quote.CreatedById != Guid.Empty)
            {
                var createdBy = DbContext.UserProfiles.Find(quote.CreatedById);
                DbContext.UserProfiles.Attach(createdBy);
            }

            quote.PictureId = Program.QuoteDefaultPicId;
            var picture = DbContext.Pictures.Find(Program.QuoteDefaultPicId);
            DbContext.Pictures.Attach(picture);

            //get all groups referenced by the event (targetGroups)
            List<Group> groups = new List<Group>
            {
                DbContext.Groups.Find(Program.PublicGroupId)
            };

            //Attach them all to avoid creation of groups
            foreach (Group gr in groups)
                DbContext.Groups.Attach(gr);

            quote.PublicationDate = DateTime.Now < quote.PublicationDate ? quote.PublicationDate.Date.AddHours(1) : quote.PublicationDate;
            quote.EndDate = quote.EndDate.Date.AddHours(23);
            
            //create the associated reporting
            Reporting Reporting = new Reporting
            {
                Views = 0,
                Answers = 0,
                CreatedById = quote.CreatedById,
                Content = quote.Content,
                Type = CardTypes.Reporting.ToString(),
                PublicationDate = quote.PublicationDate,
                PictureId = quote.PictureId,
                EndDate = quote.EndDate.AddDays(1)
            };

            DbContext.Cards.Add(quote);
            DbContext.Cards.Add(Reporting);
            await DbContext.SaveChangesAsync();
            Reporting.LinkedCardId = quote.Id;
            quote.LinkedCardId = Reporting.Id;
            /*quote.cardGroup = new List<CardGroup>();
            quote.cardGroup.Add(new CardGroup
            {
                card = quote,
                cardId = quote.Id,
                group = DbContext.Set<Group>().Where(g => g.Id == Program.publicGroupId).FirstOrDefault(),
                groupId = Program.publicGroupId
            });*/
            await DbContext.SaveChangesAsync();
            return quote;
        }

        public async Task<EditableQuoteVM> GetEditableQuote(Guid idQuote)
        {

            var result = await DbContext.Set<Card>()
                .Where(quote => quote.Id == idQuote)
                .Select(quote => new EditableQuoteVM(
                    quote.Id.ToString(), quote.Content, quote.PublicationDate,
                    quote.EndDate))
                .FirstOrDefaultAsync();
            
            return result;
        }
    }
}
