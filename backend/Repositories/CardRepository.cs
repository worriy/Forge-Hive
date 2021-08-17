using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace Hive.Backend.Repositories
{
    public class CardRepository : Repository<Card>, ICardRepository
    {
        private readonly Guid publicGroup = Program.PublicGroupId;
        private readonly Guid AdminId = Program.AdminId;

        public CardRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Card> GetAllWithReferences()
        {
            return DbContext.Set<Card>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).AsNoTracking();
        }

        public async Task<Card> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Card>().Include(m => m.CreatedBy).Include(u => u.CreatedBy.User).Include(m => m.Picture).Include(m => m.Choices).FirstOrDefaultAsync(p => p.Id == id);
        }
        #region Flow

        public async Task<IQueryable<Card>> GetFlowCards(PagingVM paging, Guid userProfileId)
        {
            //TODO: A tester

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

            var _answerRepository = new AnswerRepository(DbContext);

            List<Guid> answeredCardsIds = DbContext.Answers
                .Where(answer => answer.AnsweredById == userProfileId)
                .Select(a => a.CardId)
                .ToList();

            //Get the cards targetting the user's groups and of which publication and end date are good.
            var now = DateTime.Now;
            
            // List of the questions of Survey
            var questionIds = await DbContext.Set<Question>()
                .Where(card => card.SurveyId != Guid.Empty
                    && (groupCardsIds.Contains(card.Id) && !answeredCardsIds.Contains(card.Id))
                    //the card is not a Reporting and is published
                    && ((card.Type != CardTypes.Reporting.ToString()
                        && card.PublicationDate <= now
                        && card.EndDate > now)
                    //Or the card is a Reporting and is in its last published day
                    || (card.Type == CardTypes.Reporting.ToString()
                        && card.EndDate.Date == DateTime.Today.Date)))
                .Select(q => q.Id)
                .ToListAsync();

            // List of report of questions of the Survey
            var reportsIds = await DbContext.Set<Reporting>()
                .Where(card => (groupCardsIds.Contains(card.Id) 
                    && !answeredCardsIds.Contains(card.Id))
                    && card.EndDate.Date == DateTime.Today.Date)
                .Select(card => card.Id)
                .ToListAsync();

            List<Guid> SurveyReportIds = new List<Guid>();
            foreach(var reportId in reportsIds)
            {
                var card = await DbContext.Set<Question>()
                    .Where(c => c.LinkedCardId == reportId)
                    .FirstOrDefaultAsync();
                if(card != null && card.SurveyId != Guid.Empty)
                {
                    SurveyReportIds.Add(DbContext.Set<Card>()
                        .Where(c => c.Id == reportId)
                        .Select(s => s.Id)
                        .FirstOrDefault());
                    //SurveyReports.Add(card);
                }
            }

            //var suggestion = DbContext.Set<Suggestion>().Where(s => s.Id > paging.LastId).FirstOrDefaultAsync();

            
            var cards = await DbContext.Cards.Where(card =>
                    //card.Id > (paging.LastId) &&
                    card.Type != CardTypes.Mood.ToString()
                    //The card targets the user's groups and the user did not answer it
                    && (groupCardsIds.Contains(card.Id) && !answeredCardsIds.Contains(card.Id))
                    //the card is not a Reporting and is published
                    && ((card.Type != CardTypes.Reporting.ToString()
                        && card.PublicationDate <= now
                        && card.EndDate > now)
                    //Or the card is a Reporting and is in its last published day
                    || (card.Type == CardTypes.Reporting.ToString()
                        && card.EndDate.Date == DateTime.Today.Date))
                    && !questionIds.Contains(card.Id)
                    && !SurveyReportIds.Contains(card.Id))
                .ToListAsync();

            //List<Card> cards = new List<Card>();
            //Card mood = new Card();
            //List<Card> newCards = new List<Card>();

            // Verify if the user has answered to the Mood Card Today
            var answeredMood = await DbContext.Set<Answer>().Where(a => a.AnsweredById == userProfileId && a.CardId == Program.MoodId && a.AnswerDate.Date == now.Date).FirstOrDefaultAsync();
            if (answeredMood == null)
            {
                Card mood = await DbContext.Set<Card>().Where(m => m.Id == Program.MoodId /*&& m.Id > paging.LastId*/).FirstOrDefaultAsync();
                if(mood != null)
                {
                    cards.Insert(0, mood);
                }
            }
            
            
            for (int i = 0; i < cards.Count; i++)
            {
                // If the report is Survey Report
                var surveyReport = await DbContext.Set<Survey>().Where(c => c.LinkedCardId == cards[i].Id).FirstOrDefaultAsync();
                if (surveyReport != null)
                    cards[i].Type = "SurveyReport";

                cards[i].Picture = await DbContext.Pictures.FirstOrDefaultAsync(pic => pic.Id == cards[i].PictureId);
                cards[i].CreatedBy = await DbContext.UserProfiles
                    .Where(user => user.Id == cards[i].CreatedById)
                    .Include(user => user.User)
                    .FirstOrDefaultAsync();
            }

            var result = cards.AsQueryable();
            // Pagination
            if (paging.Page > 0)
            {
                if (paging.Step > 0)
                {
                    var skip = paging.Step * (paging.Page - 1);
                    result = result.Skip(skip);
                }
                else
                {
                    result = result.Skip(paging.Page);
                }
            }

            if (paging.Step > 0)
            {
                result = result.Take(paging.Step);
            }

            return result;
    }

    public async Task AddView(Guid cardId, Guid userProfileId)
    {
        //TODO: A tester

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

    public async Task<IQueryable<Guid>> CheckDeletedCards(PagingVM paging, Guid userProfileId)
    {

        //get the user's groups ids
        var groupsIds = await DbContext.Set<GroupUser>().Where(gu => gu.UserId == userProfileId).Select(gu => gu.GroupId).ToListAsync();

        //get the ids of cards targetting the user's groups
        var groupCardsIds = await DbContext.Set<CardGroup>().Where(cg => groupsIds.Contains(cg.GroupId)).Select(cg => cg.CardId).ToListAsync();

        var _answerRepository = new AnswerRepository(DbContext);

        var answeredCardsIds = DbContext.Answers.Where(answer => answer.AnsweredById == userProfileId).Select(a => a.CardId).ToList();

        //Get the cards targetting the user's groups ids and of which publication and end date are good.
        var now = DateTime.Now;
        var ids = await DbContext.Cards.Where(card =>
                //card.Id <= paging.LastId &&
                card.Type != CardTypes.Mood.ToString()
                //The card targets the user's groups and the user did not answer it
                && (groupCardsIds.Contains(card.Id) && !answeredCardsIds.Contains(card.Id))
                //the card is not a Reporting and is published
                && ((card.Type != CardTypes.Reporting.ToString()
                    && card.PublicationDate <= now
                    && card.EndDate > now)
                //Or the card is a Reporting and is in its last published day
                || (card.Type == CardTypes.Reporting.ToString()
                    && card.EndDate.Date == DateTime.Today.Date)))
            .Select(c => c.Id)
            .ToListAsync();

        List<Guid> cardIds = new List<Guid>();

        var answeredMood = await DbContext.Set<Answer>().Where(a => a.AnsweredById == userProfileId && a.CardId == Program.MoodId && a.AnswerDate.Date == now.Date).FirstOrDefaultAsync();
        if (answeredMood == null)
        {
            var mood = await DbContext.Set<Card>().Where(m => m.Id == Program.MoodId /*&& m.Id <= paging.LastId*/).Select(m => m.Id).FirstOrDefaultAsync();
            if (mood != Guid.Empty)
            {
                cardIds = ids.Append(mood)
                .OrderBy(id => id)
                .ToList();
            }
            else
            {
                cardIds = ids.OrderBy(id => id)
                .ToList();
            }

        }
        else
        {
            cardIds = ids.OrderBy(id => id)
                .ToList();
        }

        return cardIds.AsQueryable<Guid>();
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

    public async Task<Mood> CreateMoodCard(Mood mood)
    {
        try
        {
            var now = DateTime.Now.Date;

            var test = await DbContext.Set<Mood>().Where(m => m.PublicationDate.Date == now && mood.EndDate.Date == now).Include(m=> m.Picture).Include(c => c.CreatedBy.User).FirstOrDefaultAsync();
            if(test != null)
            {
                // Display the Mood Card
                return test;
            }
            else
            {
            // Create the Mood Card

            // Create the content 
            mood.Content = "What's your mood today?";
                    mood.Choices = new HashSet<Choice>
                    {
                        new Choice()
                        {
                            Name = "Happy"
                        },
                        new Choice()
                        {
                            Name = "Indifferent"
                        },
                        new Choice()
                        {
                            Name = "Muted"
                        },
                        new Choice()
                        {
                            Name = "Sad"
                        },
                        new Choice()
                        {
                            Name = "Sunglasses"
                        }
                    };

            // CreatedBy is the Admin
            mood.CreatedById = AdminId;
            var createdBy = DbContext.UserProfiles.Find(mood.CreatedById);
            DbContext.UserProfiles.Attach(createdBy);

            // Picture of the Mood Card
            mood.PictureId = Program.IdeaDefaultPicId;
            var picture = DbContext.Pictures.Find(mood.PictureId);
            DbContext.Pictures.Attach(picture);

                    // Publish this Mood Card in the Public Group
                    mood.CardGroup = new List<CardGroup>
                    {
                        new CardGroup(Guid.Empty, Program.PublicGroupId)
                    };
                    var group = DbContext.Groups.Find(Program.PublicGroupId);
            DbContext.Groups.Attach(group);

            // Publication Date and End Date of Mood Card
            mood.PublicationDate = now.Date;
            mood.EndDate = now.Date.AddHours(23);

            // Create the associated reporting
            var Reporting = new Reporting
            {
                Views = 0,
                Answers = 0,
                CreatedById = mood.CreatedById,
                Content = mood.Content,
                Type = CardTypes.Reporting.ToString(),
                PublicationDate = mood.PublicationDate,
                PictureId = mood.PictureId,
                EndDate = mood.EndDate
            };

            Reporting.Choices = new HashSet<Choice>();
            Reporting.Choices = mood.Choices;

            DbContext.Cards.Add(mood);
            DbContext.Cards.Add(Reporting);

                //await DbContext.SaveChangesAsync();
            if(await DbContext.SaveChangesAsync() != 0)
            {
                Reporting.LinkedCardId = mood.Id;
                mood.LinkedCardId = Reporting.Id;

                //Create the associated results
                var Results = new Result[]
                {
                    new Result
                    {
                        ReportingId = Reporting.Id,
                        Value = 0,
                        Choice = mood.Choices.ElementAt(0)
                    },
                    new Result
                    {
                        ReportingId = Reporting.Id,
                        Value = 0,
                        Choice = mood.Choices.ElementAt(1)
                    },
                    new Result
                    {
                        ReportingId = Reporting.Id,
                        Value = 0,
                        Choice = mood.Choices.ElementAt(2)
                    },
                    new Result
                    {
                        ReportingId = Reporting.Id,
                        Value = 0,
                        Choice = mood.Choices.ElementAt(3)
                    },
                    new Result
                    {
                        ReportingId = Reporting.Id,
                        Value = 0,
                        Choice = mood.Choices.ElementAt(4)
                    }
                };

                Reporting.Results = new HashSet<Result>();
                foreach (Result r in Results)
                    Reporting.Results.Add(r);

                Reporting.CardGroup = new List<CardGroup>();
                //create a row in the join table for each group referenced by the entity
                    var cardGroupReporting = new CardGroup
                    {
                        Card = Reporting,
                        CardId = Reporting.Id,
                        Group = group,
                        GroupId = group.Id
                    };
                    //Add the row in the Card to keep navigation references
                    Reporting.CardGroup.Add(cardGroupReporting);
                    //Add the row to the context before saving
                    DbContext.Set<CardGroup>().Add(cardGroupReporting);
                    
                await DbContext.SaveChangesAsync();
            }
            return mood;
        }
        }
        catch(Exception xcp)
        {
            //log exception
            throw new Exception("Exception " + xcp);
        }
    }
        
    #endregion

        #region Highlights

        /// <summary>
        /// Returns the top posts reports, regardless of its type
        /// Top post stands for the most answered and the most viewed public post
        /// </summary>
        public async Task<List<Reporting>> GetTopPosts()
        {
            // Exclude the questions of Survey
            var questionIds = await DbContext.Set<Question>()
                .Where(q => q.SurveyId != Guid.Empty)
                .Select(q => q.Id)
                //.Select(DbContext.Set<Reporting>().Where(r => r.LinkedCardId ==))
                .ToListAsync();
            var reports = new List<Guid>();
            if (questionIds != null)
            {
                //List<Reporting> reports = new List<Reporting>();
                foreach (var question in questionIds)
                {
                    reports.Add(DbContext.Set<Reporting>()
                        .Where(report => report.LinkedCardId == question)
                        .Select(r => r.Id)
                        .FirstOrDefault());
                }
            }
            

            /*var test = await DbContext.Set<Reporting>()
                .Where(r => r.LinkedCardId.Equals((DbContext.Set<Question>().Where(q => q.SurveyId != 0 ).Select(q => q.Id))))
                .ToListAsync();*/

            //retrieve 5 published public cards with the best scores
            //score = ((number of answers) / (number of views) * (number of answers))
            var result = await DbContext.Set<Reporting>()
                .Where(report => report.PublicationDate >= DateTime.Today.AddDays(-10)
                    && report.PublicationDate <= DateTime.Now
                    && report.EndDate >= DateTime.Today
                    && !reports.Contains(report.Id))
                .Include(report => report.Picture)
                .Join(DbContext.Set<CardGroup>()
                    .Where(m =>
                        m.GroupId == publicGroup),
                        report => report.Id,
                        cardGroup => cardGroup.CardId,
                        (report, cardGroup) => report) 
                .OrderByDescending(report => report.Views == 0 ? 0 : (((report.Answers * 4) + (report.Likes * 3) + (report.Views * 2))/9))
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

            // Exclude the questions of Survey
            var questionIds = await DbContext.Set<Question>()
                .Where(q => q.SurveyId != Guid.Empty && q.EndDate.Date <= DateTime.Today.Date)
                .Select(q => q.Id)
                .ToListAsync();

            var reportsQuestion = new List<Guid>();
            if (questionIds != null)
            {
                //List<Reporting> reports = new List<Reporting>();
                foreach (var question in questionIds)
                {
                    reportsQuestion.Add(DbContext.Set<Reporting>()
                        .Where(report => report.LinkedCardId == question)
                        .Select(r => r.Id)
                        .FirstOrDefault());
                }
            }

            //Retrieve the reportings with at least 1 answer and 1 view expired and published in the last 10 days
            var reports = DbContext.Set<Reporting>()
                .Where(rep =>
                     rep.Answers > 0
                  && rep.Views > 0
                  && rep.Likes > 0
                  && rep.EndDate.Date <= DateTime.Today.Date
                  && rep.EndDate.Date >= DateTime.Today.AddDays(-10).Date
                  && !reportsQuestion.Contains(rep.Id))
                .Join(DbContext.Set<CardGroup>()
                    .Where(m =>
                        m.GroupId == publicGroup),
                        report => report.Id,
                        cardGroup => cardGroup.CardId,
                        (report, cardGroup) => report);

            //take the one with the best score 
            //score = ((number of answers) / (number of views) * (number of answers) 
            var best = await reports
                .Where(rep =>
                    (((rep.Answers * 4) + (rep.Likes * 3) + (rep.Views * 2))/9) == reports.Max(reportTest => ((reportTest.Answers * 4) + (reportTest.Likes * 3) + (reportTest.Views * 2))/9))
                    //(rep.Answers / rep.Views) * rep.Answers == reports.Max(reportTest => (reportTest.Answers / reportTest.Views) * reportTest.Answers))
                .FirstOrDefaultAsync();

            //get its results if best != null 
            if(best != null)
            {
                best.Results = DbContext.Set<Result>()
                .Where(result => result.ReportingId == best.Id)
                .Include(result => result.Choice)
                .ToHashSet();
            }
            return best;

        }

        /// <summary>
        /// returns the reporting associated to the card with id = idCard
        /// </summary>
        /// /// <param name="idCard"></param>
        /// <returns></returns>
        public async Task<PostDetailsVM> GetPostDetails(Guid idCard)
        {
            var card = await DbContext.Cards
                            .Where(c => c.Id == idCard)
                            .Include(c => c.Picture)
                            .FirstOrDefaultAsync();
            Reporting report = new Reporting();
            
            List<Question> reportQuestions = new List<Question>();

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
            
            var results = DbContext.Set<Result>()
                .Where(r => r.ReportingId == report.Id)
                .Select(r => new { r.Id, r.ReportingId, r.Choice, r.Value })
                .ToList();

            report.Results = new HashSet<Result>();
            foreach (var r in results)
            {
                report.Results.Add(new Result
                {
                    Id = r.Id,
                    Value = r.Value,
                    ReportingId = r.ReportingId,
                    Choice = r.Choice
                });
            }

            
            //Create the string to display
            string groupNames = await this.GetTargetGroupsString(idCard);

            var postDetails = new PostDetailsVM()
            {
                Id = idCard.ToString(),
                Content = report.Content,
                PublicationDate = report.PublicationDate,
                EndDate = card.EndDate,
                Views = report.Views,
                Likes = report.Likes,
                Answers = report.Answers,
                Picture = card.Picture.PicBase64,
                TargetGroups = groupNames,
                Results = report.Results,
                Questions = reportQuestions.ToHashSet()

            };
            if (DateTime.Now > card.EndDate)
                postDetails.Type = report.Type;
            else postDetails.Type = card.Type;

            return postDetails;
        }

        /// <summary>
        /// Gets the number of views in the report of a card
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public async Task<int> GetViewsNumber(Guid idCard)
        {
            var report = await DbContext.Set<Reporting>()
                .FirstOrDefaultAsync(m => m.LinkedCardId == idCard
                    || m.Id == idCard);
            return ((Reporting)report).Views;
        }

        /// <summary>
        /// Gets the number of answers in the report of a card
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public async Task<int> GetAnswersNumber(Guid idCard)
        {
            var report = await DbContext.Set<Reporting>()
                .FirstOrDefaultAsync(m => m.LinkedCardId == idCard
                    || m.Id == idCard);
            return ((Reporting)report).Answers;
        }


        /// <summary>
        /// Fills CreatedBy property of the card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public async Task<Card> FillUserInfos(Card card)
        {
            UserProfileRepository userRepository = new UserProfileRepository(DbContext);
            UserVM userVM = new UserVM();
            userVM = Convert(await userRepository.GetByIdWithReferences(card.CreatedById));
            card.CreatedBy = ConvertToUserProfile(userVM);
            return card;
        }

        public GeneralMoodVM GetGeneralMood()
        {
            var now = DateTime.Now.Date;

            var answers = DbContext.Set<Answer>().Where(a => a.CardId == Program.MoodId && a.AnswerDate.Date == now)
                .Include(c => c.Choice)
                .AsEnumerable()
                .GroupBy(a => a.Choice.Name).ToList();

            var members = DbContext.Set<UserProfile>().Count() - 1;

            GeneralMoodVM generalMood = new GeneralMoodVM();

            if (answers.Count != 0)
            {
                IGrouping<string, Answer> result = null;
                var number = 0;
                foreach (var answer in answers)
                {

                    if (answer.Count() > number)
                    {
                        number = answer.Count();
                        result = answer;
                    }

                }

                generalMood = new GeneralMoodVM
                {
                    MoodName = result.Key,
                    Value = (number * 100) / members
                };

            }
            

            return generalMood;
        
        }

        #endregion

        #region Posts

        /// <summary>
        /// Returns the top posts reports, regardless of its type
        /// Top post stands for the most answered and the most viewed public post
        /// </summary>
        public async Task<ICollection<Reporting>> GetTopPostsByUser(Guid userProfileId)
        {
            // Exclude the questions of Survey
            var questionIds = await DbContext.Set<Question>()
                .Where(q => q.SurveyId != Guid.Empty && q.CreatedById == userProfileId && q.PublicationDate < DateTime.Now)
                .Select(q => q.Id)
                .ToListAsync();

            var reportIds = new List<Guid>();
            if (questionIds != null)
            {
                //List<Reporting> reports = new List<Reporting>();
                foreach (var question in questionIds)
                {
                    reportIds.Add(DbContext.Set<Reporting>().Where(report => report.LinkedCardId == question)
                        .Select(r => r.Id)
                        .FirstOrDefault());
                }
            }

            var result = await DbContext.Set<Reporting>()
                .Where(r => r.CreatedById == userProfileId
                    && r.PublicationDate < DateTime.Now
                    && !reportIds.Contains(r.Id))
                .Include(p => p.Picture)
                .OrderByDescending(r => r.Answers)
                .Take(5)
                .ToListAsync();

            return result; 
           

        }

        public async Task<List<Card>> GetLatestPosts(PagingVM paging, Guid userProfileId)
        {
            // Exclude the questions of Survey
            var questionIds = await DbContext.Set<Question>()
                .Where(q => q.SurveyId != Guid.Empty && q.CreatedById == userProfileId)
                .Select(q => q.Id)
                .ToListAsync();

            var cards = DbContext.Set<Card>()
                .Where(card =>
                   //card.Id > paging.LastId &&
                   card.Type != CardTypes.Reporting.ToString()
                   && card.CreatedById == userProfileId
                   && !questionIds.Contains(card.Id))
                //.Except(questions)
                //.OrderByDescending(card => card.Id)
                .OrderBy(card => card.PublicationDate)
                .OrderBy(card => card.EndDate)
                .AsQueryable();
                /*.Take(paging.Step)
                .ToListAsync();*/

            if (paging.Page > 0)
            {
                if (paging.Step > 0)
                {
                    var skip = paging.Step * (paging.Page - 1);
                    cards = cards.Skip(skip);
                }
                else
                {
                    cards = cards.Skip(paging.Page);
                }
            }

            if (paging.Step > 0)
            {
                cards = cards.Take(paging.Step);
            }

            return await cards.ToListAsync();
        }

        new public async Task Delete(Card post)
        {
            try
            {
                //Remove the corresponding results
                var results = DbContext.Set<Result>()
                    .Where(r => r.Reporting.LinkedCardId == post.Id)
                    .ToList();
                foreach (Result r in results)
                {
                    DbContext.Set<Result>().Remove(r);
                }

                if (post.Type != CardTypes.Survey.ToString())
                {
                    //Remove the choices
                    var choices = DbContext.Set<Card>()
                    .Where(c => c.Id == post.Id)
                    .Select(c => c.Choices)
                    .FirstOrDefault();
                
                    foreach (Choice c in choices)
                    {
                        var choice = DbContext.Set<Choice>()
                            .Where(ch => ch.Id == c.Id)
                            .FirstOrDefault();
                        DbContext.Set<Choice>().Remove(choice);
                    }
                }
                
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

        #endregion

        #region helpers

        public async Task<string> GetTargetGroupsString(Guid cardId)
        {
            //TODO: A tester

            string targetGroupsString = "";

            //Get the Ids of the groups targetted by this card
            var targetGroupsIds = await DbContext.Set<CardGroup>().Where(cg => cg.CardId == cardId).Select(cg => cg.GroupId).ToListAsync();

            var lastId = targetGroupsIds.Last();
            foreach(var id in targetGroupsIds)
            {
                //Add the name to the string 
                targetGroupsString += await DbContext.Groups.Where(group => group.Id == id).Select(group => group.Name).FirstOrDefaultAsync();

                //if it is not the last target group, add a coma and space
                if (id != lastId)
                    targetGroupsString += ", ";
            }

            return targetGroupsString;
        }

        public async Task<HashSet<Result>> GetResults(Guid cardId)
        {
            //TODO: A tester

            //Get the results of the card
            var results = DbContext.Results
                .Join(DbContext.Reportings
                    .Where(repo =>
                        repo.LinkedCardId == cardId
                     || repo.Id == cardId),
                     result => result.ReportingId,
                     report => report.Id,
                     (result, report) => result)
                .Include(r => r.Choice)
                .ToHashSet();

            foreach (Result item in results)
            {
                item.Reporting = null;
            };

            return await Task.FromResult(results);

        }

        private UserVM Convert(UserProfile userProfile)
        {
            return new UserVM
            {
                Firstname = userProfile.User.FirstName,
                Lastname = userProfile.User.LastName,
                Picture = userProfile.User.PictureUrl,
                Email = userProfile.User.Email,
                Job = userProfile.Job,
                UserProfileId = userProfile.Id.ToString()
            };
        }

        private UserProfile ConvertToUserProfile(UserVM user)
        {
            return new UserProfile
            {
                Job = user.Job,
                Id = Guid.Parse(user.UserProfileId)
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
