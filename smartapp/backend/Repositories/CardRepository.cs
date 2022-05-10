using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public class CardRepository : Repository<Card>, ICardRepository
    {
        private readonly Guid publicGroup = Program.PublicGroupId;
        private readonly Guid moodId = Program.MoodId;
        private readonly int maxLimit = 50;

        public CardRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Card> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Card>()
                .Include(m => m.CreatedBy)
                .Include(u => u.CreatedBy.User)
                .Include(m => m.Picture)
                .Include(m => m.Choices)
                .Include(cg => cg.CardGroup)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Card> GetCardById(Guid id)
        {
            var card = await DbContext.Set<Card>()
                .Include(m => m.CreatedBy)
                .Include(u => u.CreatedBy.User)
                .Include(m => m.Picture)
                .Include(m => m.Choices)
                .Include(cg => cg.CardGroup)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (card.Type != CardTypes.Reporting.ToString())
                return card;

            return await DbContext.Set<Card>()
                .Include(m => m.CreatedBy)
                .Include(u => u.CreatedBy.User)
                .Include(m => m.Picture)
                .Include(m => m.Choices)
                .Include(cg => cg.CardGroup)
                .FirstOrDefaultAsync(p => p.LinkedCardId == id);
        }

        #region Flow

        public async Task<IQueryable<Card>> GetFlowCards(Guid userProfileId)
        {
            //get the user's groups ids
            var groupsIds = await DbContext.Set<GroupUser>()
                .Where(gu => gu.UserId == userProfileId)
                .Select(gu => gu.GroupId)
                .ToListAsync();

            //get the ids of cards targetting the user's groups
            var groupCardsIds = await DbContext.Set<CardGroup>()
                .Where(cg => groupsIds.Contains(cg.GroupId))
                .Select(cg => cg.CardId)
                .ToListAsync();

            var answeredCardsIds = DbContext.Answers
                .Where(answer => answer.AnsweredById == userProfileId)
                .Select(a => a.CardId)
                .ToList();

            var cards = new List<Card>();

            // Verify if the user has answered to the Mood Card Today
            var answeredMood = await DbContext.Set<Answer>().Where(a => a.AnsweredById == userProfileId && a.CardId == moodId && a.AnswerDate.Date == DateTime.Now.Date).FirstOrDefaultAsync();
            var moodCard = answeredMood == null ? await DbContext.Cards.Where(c => c.Id == moodId).Include(u => u.CreatedBy.User).FirstOrDefaultAsync() : null;

            if (moodCard != null)
                cards.Add(moodCard);

            cards.AddRange(await DbContext.Cards.Where(card =>
                    //The card targets the user's groups and the user did not answer it
                    groupCardsIds.Contains(card.Id) && !answeredCardsIds.Contains(card.Id)
                    //the card is not a Reporting and is published
                    && ((card.Type != CardTypes.Reporting.ToString()
                        && card.Type != CardTypes.Mood.ToString()
                        && card.PublicationDate <= DateTime.Now
                        && card.EndDate > DateTime.Now)
                    //Or the card is a Reporting and is in its last published day
                    || (card.Type == CardTypes.Reporting.ToString()
                        && card.EndDate.Date == DateTime.Today.Date)
                        ))
                .Include(u => u.CreatedBy.User)
                .Take(maxLimit)
                .ToListAsync());

            return cards.AsQueryable();
        }

        public async Task AddView(Guid cardId, Guid userProfileId)
        {
            //If the user sees the card for the first time
            if (!DbContext.Set<UserCardViews>().Where(ucv => ucv.CardId == cardId && ucv.UserId == userProfileId).Any())
            {
                //Get the connected card's Id (could be a Reporting or any other type)
                var connectedCardId = DbContext.Cards.Where(card => card.Id == cardId).Select(card => card.LinkedCardId).FirstOrDefault();

                //Get the Report of this Card 
                var report = DbContext.Set<Reporting>()
                    .Where(r => r.LinkedCardId == cardId)
                    .FirstOrDefault();

                //Add a view count on the card and the linked one
                await DbContext.Set<UserCardViews>().AddRangeAsync(new UserCardViews()
                {
                    CardId = cardId,
                    UserId = userProfileId
                });
                report.Views++;

                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<IQueryable<Guid>> CheckDeletedCards(Guid userProfileId)
        {
            //get the user's groups ids
            var groupsIds = await DbContext.Set<GroupUser>()
                .Where(gu => gu.UserId == userProfileId)
                .Select(gu => gu.GroupId)
                .ToListAsync();

            //get the ids of cards targetting the user's groups
            var groupCardsIds = await DbContext.Set<CardGroup>()
                .Where(cg => groupsIds.Contains(cg.GroupId))
                .Select(cg => cg.CardId)
                .ToListAsync();

            var answeredCardsIds = DbContext.Answers
                .Where(answer => answer.AnsweredById == userProfileId)
                .Select(a => a.CardId)
                .ToList();

            var cardIds = new List<Guid>();
            // Verify if the user has answered to the Mood Card Today
            var answeredMood = await DbContext.Set<Answer>().Where(a => a.AnsweredById == userProfileId && a.CardId == moodId && a.AnswerDate.Date == DateTime.Now.Date).FirstOrDefaultAsync();

            if (answeredMood == null)
                cardIds.Add(moodId);

            cardIds.AddRange(await DbContext.Cards.Where(card =>
                    //The card targets the user's groups and the user did not answer it
                    groupCardsIds.Contains(card.Id) && !answeredCardsIds.Contains(card.Id)
                    //the card is not a Reporting and is published
                    && ((card.Type != CardTypes.Reporting.ToString()
                        && card.PublicationDate <= DateTime.Now
                        && card.EndDate > DateTime.Now)
                    //Or the card is a Reporting and is in its last published day
                    || (card.Type == CardTypes.Reporting.ToString()
                        && card.EndDate.Date == DateTime.Today.Date)))
                .Select(c => c.Id)
                .ToListAsync());

            return cardIds.AsQueryable();
        }

        public async Task Like(Guid cardId, Guid userProfileId)
        {
            if (!DbContext.Set<UserCardLikes>().Where(ucv => ucv.CardId == cardId && ucv.UserId == userProfileId).Any())
            {
                //Get the Report of this Card 
                var report = DbContext.Set<Reporting>()
                    .Where(r => r.LinkedCardId == cardId)
                    .FirstOrDefault();

                //Add a view count on the card and the linked one
                await DbContext.Set<UserCardLikes>().AddRangeAsync(new UserCardLikes()
                {
                    CardId = cardId,
                    UserId = userProfileId
                });
                report.Likes++;

                await DbContext.SaveChangesAsync();
            }
            
        }

        public async Task Dislike(Guid cardId, Guid userProfileId)
        {
            if (DbContext.Set<UserCardLikes>().Where(ucv => ucv.CardId == cardId && ucv.UserId == userProfileId).Any())
            {
                //Get the Report of this Card 
                var report = DbContext.Set<Reporting>()
                    .Where(r => r.LinkedCardId == cardId)
                    .FirstOrDefault();
                
                DbContext.Set<UserCardLikes>().RemoveRange(DbContext.Set<UserCardLikes>()
                    .Where(ucl => ucl.UserId == userProfileId && ucl.CardId == cardId));
                report.Likes--;

                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckLikedCard(Guid cardId, Guid userId)
        {
            if (DbContext.Set<UserCardLikes>().Where(ucl => ucl.CardId == cardId && ucl.UserId == userId).Any())
                return await Task.FromResult(true);
       
            return await Task.FromResult(false);
        }
        
        #endregion

        #region Highlights

        /// <summary>
        /// Returns the top posts reports, regardless of its type
        /// Top post stands for the most answered and the most viewed public post
        /// </summary>
        public async Task<List<Reporting>> GetTopPosts()
        {
            var members = DbContext.Users.Count();
            //retrieve 5 published public cards with the best scores
            //score = ((Likes + Answers + Views) / Total members) * 100
            var result = await DbContext.Set<Reporting>()
                .Where(report =>
                    report.PublicationDate >= DateTime.Today.AddDays(-10)
                    && report.PublicationDate <= DateTime.Now
                    && report.EndDate >= DateTime.Today)
                .Include(report => report.Picture)
                .Join(DbContext.Set<CardGroup>()
                    .Where(m =>
                        m.GroupId == publicGroup),
                        report => report.Id,
                        cardGroup => cardGroup.CardId,
                        (report, cardGroup) => report) 
                .OrderByDescending(report => ((report.Likes + report.Answers + report.Views) / members) * 100)
                .Take(5)
                .ToListAsync();
            
            return result;

        }

        public string GetTypeCardFromReportId(Guid idReport)
        {
            var result = DbContext.Set<Card>()
                .Where(card => card.LinkedCardId == idReport)
                .FirstOrDefault();

            var type = result.Type;
            return type;
        }

        /// <summary>
        /// Returns the best post report, regardless of its type
        /// Best post stands for the most answered public post
        /// </summary>
        public async Task<Reporting> GetBestPost()
        {
            var members = DbContext.Users.Count();
            //Retrieve the reportings with at least 1 answer and 1 view expired and published in the last 10 days
            var bestPost = await DbContext.Set<Reporting>()
                .Where(report =>
                  report.EndDate.Date <= DateTime.Today.Date
                  && report.EndDate.Date >= DateTime.Today.AddDays(-10).Date)
                .Join(DbContext.Set<CardGroup>()
                    .Where(m =>
                        m.GroupId == publicGroup),
                        report => report.Id,
                        cardGroup => cardGroup.CardId,
                        (report, cardGroup) => report)
                .OrderByDescending(report => ((report.Likes + report.Answers + report.Views) / members) * 100)
                .FirstOrDefaultAsync();

            //get its results if best != null 
            if(bestPost != null)
            {
                bestPost.Results = await DbContext.Set<Result>()
                    .Where(result => result.ReportingId == bestPost.Id)
                    .Select(r => new Result
                        {
                            Id = r.Id,
                            Value = r.Value,
                            ReportingId = r.ReportingId,
                            Choice = r.Choice
                        })
                    .ToListAsync();
            }
            return bestPost;
        }

        /// <summary>
        /// returns the reporting associated to the card with id = idCard
        /// </summary>
        /// /// <param name="idCard"></param>
        /// <returns></returns>
        public async Task<(Card card, Reporting report)> GetPostDetails(Guid idCard)
        {
            var card = await DbContext.Cards
                            .Where(c => c.Id == idCard)
                            .Include(c => c.Picture)
                            .FirstOrDefaultAsync();

            var report = new Reporting();
            if(card.Type == CardTypes.Reporting.ToString())
            {
                report = (Reporting)await DbContext.Cards
                    .Where(c => c.Type == CardTypes.Reporting.ToString()
                        && ((Reporting)c).Id == idCard)
                    .FirstOrDefaultAsync();
            }
            else
            {
                report = (Reporting)await DbContext.Cards
                    .Where(c => c.Type == CardTypes.Reporting.ToString()
                        && ((Reporting)c).LinkedCardId == idCard)
                    .FirstOrDefaultAsync();
            }

            var results = await DbContext.Set<Result>()
                .Where(r => r.ReportingId == report.Id)
                .Select(r => new Result
                {
                    Id = r.Id,
                    Value = r.Value,
                    ReportingId = r.ReportingId,
                    Choice = r.Choice
                }).ToListAsync();

            report.Results = results;
            
            return (card, report);
        }

        public (string MoodName, int Value) GetGeneralMood()
        {
            var now = DateTime.Now.Date;

            var answer = DbContext.Answers.Where(a => a.CardId == moodId && a.AnswerDate.Date == now)
                .Include(c => c.Choice)
                .AsEnumerable()
                .GroupBy(a => a.Choice.Name)
                .OrderByDescending(a => a.Count())
                .FirstOrDefault();

            if (answer == null)
                return (string.Empty, 0);

            var members = DbContext.Set<UserProfile>().Count();
            var value = (answer.Count() * 100) / members;

            return (answer.Key, value);
        }

        #endregion

        #region Posts

        /// <summary>
        /// Returns the top posts reports, regardless of its type
        /// Top post stands for the most answered and the most viewed public post
        /// </summary>
        public async Task<ICollection<Reporting>> GetTopPostsByUser(Guid userProfileId)
        {
            return await DbContext.Set<Reporting>()
                .Where(r => r.CreatedById == userProfileId
                    && r.PublicationDate < DateTime.Now)
                .Include(p => p.Picture)
                .OrderByDescending(r => r.Answers)
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<Card>> GetLatestPosts(Guid userProfileId)
        {

            return await DbContext.Set<Card>()
                .Where(card =>
                   card.Type != CardTypes.Reporting.ToString()
                   && card.CreatedById == userProfileId)
                .OrderBy(card => card.PublicationDate)
                .OrderBy(card => card.EndDate)
                .AsQueryable()
                .Take(maxLimit)
                .ToListAsync();
        }

        public new async Task Delete(Card post)
        {
            try
            {
                //Remove the corresponding results
                var results = DbContext.Set<Result>()
                    .Where(r => r.Reporting.LinkedCardId == post.Id)
                    .ToList();
                DbContext.Set<Result>().RemoveRange(results);

                //Remove the choices
                var choices = DbContext.Set<Card>()
                    .Where(c => c.Id == post.Id)
                    .Select(c => c.Choices)
                    .FirstOrDefault();
                DbContext.Set<Choice>().RemoveRange(choices);
                
                //Remove the corresponding rows in the UserCardViews join table
                DbContext.Set<UserCardViews>().RemoveRange(DbContext.Set<Card>().Where(c => c.Id == post.Id).Select(x => x.UserCardViews).FirstOrDefault());

                //Remove the corresponding rows in the UserCardLikes join table
                DbContext.Set<UserCardLikes>().RemoveRange(DbContext.Set<Card>().Where(c => c.Id == post.Id).Select(x => x.UserCardLikes).FirstOrDefault());

                //Remove the rows in CardGroup join table corresponding to the reporting and corresponding to the card
                var reportingCardGroup = DbContext.Set<Reporting>()
                    .Where(c => c.LinkedCardId == post.Id)
                    .Select(x => x.CardGroup)
                    .FirstOrDefault();
                DbContext.Set<CardGroup>().RemoveRange(reportingCardGroup);

                var cardGroup = DbContext.Set<Card>()
                    .Where(c => c.Id == post.Id)
                    .Select(x => x.CardGroup)
                    .FirstOrDefault();
                DbContext.Set<CardGroup>().RemoveRange(cardGroup);

                //Remove the corresponding reporting
                var report = DbContext.Set<Reporting>().Where(c => c.LinkedCardId == post.Id).FirstOrDefault();
                DbContext.Set<Card>().Remove(report);

                //Remove the corresponding answers
                DbContext.Set<Answer>().RemoveRange(DbContext.Set<Answer>().Where(a => a.CardId == report.Id || a.CardId == post.Id));

                //Remove Card
                var card = DbContext.Set<Card>().Where(c => c.Id == post.Id).FirstOrDefault();
                DbContext.Set<Card>().Remove(card);

                await DbContext.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetTypeCard(Guid linkedCardId)
        {
            return await DbContext.Cards.Where(c => c.Id == linkedCardId)
                .Select(c => c.Type)
                .FirstOrDefaultAsync();
        }

        #endregion

        #region CRUD POST
        public async Task<Card> CreatePost(Card card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            // Attach creator to DbCOntext to avoid creation of a new user
            if (card.CreatedById == Guid.Empty)
                throw new ArgumentNullException();
            var createdBy = DbContext.UserProfiles.Find(card.CreatedById);
            DbContext.UserProfiles.Attach(createdBy);

            // Attach Picture to avoid creation of a new picture
            if (card.PictureId == Guid.Empty)
                card.PictureId = GetDefaultPictureIdByType(card.Type);
            var picture = DbContext.Pictures.Find(card.PictureId);
            DbContext.Pictures.Attach(picture);
            
            // Get all groups referenced by the idea (targetGroups)
            var groupIds = card.CardGroup.Select(g => g.GroupId).ToList();
            var groups = DbContext.Groups.Where(g => groupIds.Contains(g.Id));
            DbContext.Groups.AttachRange(groups);

            card.PublicationDate = DateTime.Now < card.PublicationDate ? card.PublicationDate.Date.AddHours(1) : card.PublicationDate;
            card.EndDate = card.EndDate.Date.AddHours(23);

            // Create the associated reporting
            var report = CreatAssociatedReporting(card);

            // Add Card and Associated Reporting to the DB
            await DbContext.Cards.AddAsync(card);
            await DbContext.Cards.AddAsync(report);
            await DbContext.SaveChangesAsync();

            report.LinkedCardId = card.Id;
            card.LinkedCardId = report.Id;

            // if Card == Idea or Event Create specific Choices
            if (card.Type == CardTypes.Idea.ToString() || card.Type == CardTypes.Event.ToString())
            {
                card = CreateSpecifiChoices(card);
            }

            if (card.Choices.Any())
            {
                // Create the associated results
                report.Results = card.Choices
                    .Select(r => new Result
                    {
                        ReportingId = report.Id,
                        Value = 0,
                        Choice = r
                    })
                    .ToList();
            }
            
            // Create a row in the join table for each group referenced by the entity
            var cardGroups = groups
                .Select(gr => new CardGroup
                {
                    Card = report,
                    CardId = report.Id,
                    Group = gr,
                    GroupId = gr.Id
                })
                .ToList();

            // Add the row in the Card to keep navigation references
            report.CardGroup = cardGroups;
            // Add the row to the context before saving
            await DbContext.Set<CardGroup>().AddRangeAsync(cardGroups);

            await DbContext.SaveChangesAsync();

            return card;
        }

        public async Task UpdatePost(Card oldEntity, Card entity)
        {
            if (entity == null || oldEntity == null)
                throw new ArgumentNullException(nameof(entity));

            // If the picture changed
            if (entity.PictureId != Guid.Empty && entity.Type != CardTypes.Quote.ToString())
            {
                // If the picture changed, remove the old one from the database
                var defaultPictureId = GetDefaultPictureIdByType(oldEntity.Type);
                if (oldEntity.PictureId != defaultPictureId)
                    DbContext.Pictures.Remove(DbContext.Pictures.Find(oldEntity.PictureId));
                oldEntity.PictureId = entity.PictureId;
            }

            // Update the card
            oldEntity.Content = entity.Content;
            oldEntity.PublicationDate = oldEntity.PublicationDate.Date == entity.PublicationDate.Date ? oldEntity.PublicationDate : entity.PublicationDate.Date.AddHours(1);
            oldEntity.EndDate = oldEntity.EndDate.Date == entity.EndDate.Date ? oldEntity.EndDate : entity.EndDate.Date.AddHours(23);

            // Update the associated report
            var report = await DbContext.Set<Reporting>()
                .Where(r => r.LinkedCardId == oldEntity.Id)
                .FirstOrDefaultAsync();
            report.PictureId = oldEntity.PictureId;
            report.Content = entity.Content;
            report.PublicationDate = oldEntity.PublicationDate;
            report.EndDate = oldEntity.EndDate.AddDays(1);

            // Update Choices
            if (entity.Type != CardTypes.Idea.ToString() && entity.Type != CardTypes.Event.ToString() && entity.Choices != null)
            {
                DbContext.Set<Choice>().RemoveRange(oldEntity.Choices);
                await DbContext.SaveChangesAsync();
                oldEntity.Choices = entity.Choices;
                await DbContext.Set<Choice>().AddRangeAsync(entity.Choices);
                await DbContext.SaveChangesAsync();
                report.Results = oldEntity.Choices
                    .Select(r => new Result
                    {
                        Choice = r
                    })
                    .ToList();
            }

            // Update TargetGroups
            DbContext.Set<CardGroup>().RemoveRange(oldEntity.CardGroup);
            await DbContext.SaveChangesAsync();
            oldEntity.CardGroup = entity.CardGroup;
            report.CardGroup = entity.CardGroup;
            await DbContext.Set<CardGroup>().AddRangeAsync(entity.CardGroup);
            await DbContext.SaveChangesAsync();
        }

        public async Task<Card> GetEditablePost(Guid cardId)
        {
            return await DbContext.Cards.Where(c => c.Id == cardId)
                .Include(c => c.Choices)
                .Include(cg => cg.CardGroup)
                .Include(p => p.Picture)
                .FirstOrDefaultAsync();
        }

        public async Task<Picture> GetDefaultPictureAsync(string type)
        {
            var id = GetDefaultPictureIdByType(type);
            return await DbContext.Set<Picture>().FirstOrDefaultAsync(p => p.Id == id);
        }
        #endregion

        #region helpers

        public async Task<string> GetTargetGroupsString(Guid cardId)
        {
            var targetGroups = await DbContext.Set<CardGroup>()
                .Where(cg => cg.CardId == cardId)
                .Select(cg => cg.Group.Name)
                .ToListAsync();

            var targetGroupsString = string.Join(", ", targetGroups);

            return targetGroupsString;
        }

        public async Task<List<Result>> GetResults(Guid cardId)
        {
            //Get the results of the card
            var results = await DbContext.Results
                .Join(DbContext.Reportings
                    .Where(repo =>
                        repo.LinkedCardId == cardId
                     || repo.Id == cardId),
                     result => result.ReportingId,
                     report => report.Id,
                     (result, report) => result)
                .Select(r => new Result
                {
                    Id = r.Id,
                    Value = r.Value,
                    ReportingId = r.ReportingId,
                    Choice = r.Choice
                })
                .ToListAsync();

            return results;
        }

        private Guid GetDefaultPictureIdByType(string type)
        {
            switch (type)
            {
                case "Idea":
                    return Program.IdeaDefaultPicId;
                case "Question":
                    return Program.QuestionDefaultPicId;
                case "Event":
                    return Program.EventDefaultPicId;
                case "Mood":
                    return Program.MoodDefaultPicId;
                case "Suggestion":
                    return Program.SuggestionDefaultPicId;
                case "Quote":
                    return Program.DefaultPicId;
                default:
                    return Program.DefaultPicId;
            }
        }

        private Reporting CreatAssociatedReporting(Card card)
        {
            var report = new Reporting
            {
                Views = 0,
                Answers = 0,
                Likes = 0,
                CreatedById = card.CreatedById,
                Content = card.Content,
                Type = CardTypes.Reporting.ToString(),
                PublicationDate = card.PublicationDate,
                EndDate = card.EndDate.AddDays(1),
                PictureId = card.PictureId
            };

            return report;
        }

        private Card CreateSpecifiChoices(Card card)
        {
            switch (card.Type)
            {
                case "Idea":
                    card.Choices = CreateIdeaChoices();
                    return card;
                case "Event":
                    card.Choices = CreateEventChoices();
                    return card;
                default:
                    return card;

            }

        }

        private List<Choice> CreateIdeaChoices()
        {
            return new List<Choice>
            {
                new Choice
                {
                    Name = "Yes"
                },
                new Choice
                {
                    Name = "No"
                }
            };
        }

        private List<Choice> CreateEventChoices()
        {
            return new List<Choice>
            {
                new Choice
                {
                    Name = "Applause"
                }
            };
        }

        #endregion
    }

    public static class DateTimeExtension
    {
        /// <summary>
        /// Returns the first day of the week regarding the culture info being used by the user
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfWeek(this DateTime date)
        {
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            while (date.DayOfWeek != firstDayOfWeek)
            {
                date = date.AddDays(-1);
            }
            return date;
        }
    }
}
