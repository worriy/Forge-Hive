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
    public class SurveyRepository : Repository<Survey>, ISurveyRepository
    {
        //public QuestionRepository _questionRepository;
        public SurveyRepository(ApplicationDbContext context/*, QuestionRepository questionRepository*/) : base(context)
        {
            //_questionRepository = questionRepository;
        }

        public IQueryable<Survey> GetAllWithReferences()
        {
            return DbContext.Set<Survey>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).AsNoTracking();
        }

        public async Task<Survey> GetByIdWithReferences(Guid id)
        {
            var card = await DbContext.Set<Card>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (card != null)
            {
                if (card.Type == CardTypes.Reporting.ToString())
                    return await DbContext.Set<Survey>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup)
                    .FirstOrDefaultAsync(p => p.LinkedCardId == id);
                else return await DbContext.Set<Survey>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }

            return await DbContext.Set<Survey>().Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Question>> GetSurveyWithQuestions(Guid idSurvey)
        {
            List<Question> questionList = new List<Question>();
            

            // Recuperate th equesrtion of the Survey
            questionList = await DbContext.Set<Question>().Include(q => q.SurveyId == idSurvey).AsNoTracking().ToListAsync();

            return questionList;
        }

        public async Task UpdateSurvey(Survey oldEntity, Survey entity, List<Question> questions)
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
                if (oldCardPictureId != Program.SurveyDefaultPicId)
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

            //Update the questions of the survey
            foreach(Question question in questions)
            {
                var oldQuestion = await DbContext.Set<Question>().Where(q => q.Id == question.Id)
                    .Include(m => m.CreatedBy).Include(m => m.Picture).Include(m => m.Choices).Include(m => m.CardGroup).FirstOrDefaultAsync();

                await UpdateQuestion(oldQuestion, question);
            }

        }

        public async Task<Survey> InsertSurvey(Survey survey, List<Question> questions)
        {
            if (survey == null)
            {
                throw new ArgumentNullException(nameof(survey));
            }

            //Attach creator to DbCOntext to avoid creation of a new user
            if (survey.CreatedById != Guid.Empty)
            {
                var createdBy = DbContext.UserProfiles.Find(survey.CreatedById);
                DbContext.UserProfiles.Attach(createdBy);
            }

            //Attach Picture to avoid creation of a new picture
            if (survey.PictureId == Guid.Empty)
                survey.PictureId = Program.SurveyDefaultPicId;
            var picture = DbContext.Pictures.Find(survey.PictureId);
            DbContext.Pictures.Attach(picture);

            //Create choice of Survey
            survey.Choices = new HashSet<Choice>();
            var startSurveyChoice = await DbContext.Set<Choice>().Where(c => c.Id == Program.StartSurveyChoiceId).FirstOrDefaultAsync();
            survey.Choices.Add(startSurveyChoice);

            //get all groups referenced by the idea (targetGroups)
            List<Group> groups = new List<Group>();
            foreach (CardGroup item in survey.CardGroup)
                groups.Add(DbContext.Groups.Find(item.GroupId));

            //Attach them all to avoid creation of groups
            foreach (Group gr in groups)
                DbContext.Groups.Attach(gr);

            survey.PublicationDate = DateTime.Now < survey.PublicationDate ? survey.PublicationDate.Date.AddHours(1) : survey.PublicationDate;
            survey.EndDate = survey.EndDate.Date.AddHours(23);

            //create the associated reporting
            var Reporting = new Reporting
            {
                Views = 0,
                Answers = 0,
                CreatedById = survey.CreatedById,
                Content = survey.Content,
                Type = CardTypes.Reporting.ToString(),
                PublicationDate = survey.PublicationDate,
                PictureId = survey.PictureId,
                EndDate = survey.EndDate.AddDays(1)
            };

            Reporting.Choices = new HashSet<Choice>();
            Reporting.Choices = survey.Choices;

            DbContext.Cards.Add(survey);
            DbContext.Cards.Add(Reporting);

            if (await DbContext.SaveChangesAsync() != 0)
            {
                Reporting.LinkedCardId = survey.Id;
                survey.LinkedCardId = Reporting.Id;

                //create the associated results
                var Results = new Result[survey.Choices.Count];
                for (var i = 0; i < survey.Choices.Count; i++)
                {
                    var result = new Result
                    {
                        ReportingId = Reporting.Id,
                        Value = 0,
                        Choice = survey.Choices.ElementAt(i)
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

            // Insert the question of this survey
            for (int i = 0; i < questions.Count; i++)
            {
                questions[i].SurveyId = survey.Id;
                await InsertQuestion(questions[i]);
            }

            return survey;
        }

        public async Task<EditableSurveyVM> GetEditableSurvey(Guid surveyId)
        {

            var result = await DbContext.Set<Card>()
                .Where(survey => survey.Id == surveyId)
                .Select(survey => new EditableSurveyVM(
                    survey.Id.ToString(), survey.Content, survey.PublicationDate,
                    survey.EndDate, null, survey.Picture.PicBase64, null
                ))
                .FirstOrDefaultAsync();

            // Recuperate the questions of the survey
            result.Questions = DbContext.Set<Question>().Include(q => q.Choices)
                .Where(q => q.SurveyId == surveyId)
                .ToHashSet();
                

            // Recuperate Target Groups
            result.TargetGroupsIds = new HashSet<string>();
            var groups = await DbContext.Cards.Where(c => c.Id == surveyId)
                .Select(c => c.CardGroup)
                .FirstOrDefaultAsync();

            groups.ForEach(cg =>
            {
                result.TargetGroupsIds.Add(cg.GroupId.ToString());
            });

            return result;
        }

        public async Task<IQueryable<Card>> GetSurveyquestions(Guid surveyId)
        {
            var cards = await DbContext.Set<Question>().Where(q => q.SurveyId == surveyId)
                .Include(q => q.Picture)
                .Include(q => q.CreatedBy)
                .Include(q => q.Choices)
                .ToListAsync();

            List<Card> result = new List<Card>();
            foreach(Question question in cards)
            {
                var card = new Card()
                {
                    Id = question.Id,
                    PublicationDate = question.PublicationDate,
                    EndDate = question.EndDate,
                    Content = question.Content,
                    Type = "QuestionSurvey",
                    LinkedCardId = question.LinkedCardId,
                    CreatedById = question.CreatedById,
                    CreatedBy = question.CreatedBy,
                    PictureId = question.PictureId,
                    Picture = question.Picture,
                    Choices = question.Choices,
                    CardGroup = question.CardGroup,
                    UserCardViews = question.UserCardViews,
                    UserCardLikes = question.UserCardLikes
                };
                result.Add(card);
            }

            return result.AsQueryable();
        }

        public async Task<IQueryable<Card>> GetSurveyReportsquestions(Guid surveyReportId)
        {
            // recuperate the id of the Survey
            var surveyId = await DbContext.Set<Survey>().Where(s => s.LinkedCardId == surveyReportId).Select(s => s.Id).FirstOrDefaultAsync();

            var cardIds = await DbContext.Set<Question>().Where(q => q.SurveyId == surveyId).Select(q => q.Id)
                .ToListAsync();

            List<Card> surveyReports = new List<Card>();
            foreach(var cardId in cardIds)
            {
                var surveyReport = await DbContext.Set<Card>().Where(r => r.LinkedCardId == cardId)
                    .Include(r => r.CreatedBy.User)
                    .FirstOrDefaultAsync();
                if (surveyReport != null)
                    surveyReports.Add(surveyReport);
            }

            List<Card> result = new List<Card>();
            foreach (var report in surveyReports)
            {
                var card = new Card()
                {
                    Id = report.Id,
                    PublicationDate = report.PublicationDate,
                    EndDate = report.EndDate,
                    Content = report.Content,
                    Type = "QuestionSurveyReport",
                    LinkedCardId = report.LinkedCardId,
                    CreatedById = report.CreatedById,
                    CreatedBy = report.CreatedBy,
                    PictureId = report.PictureId,
                    Picture = report.Picture,
                    Choices = report.Choices,
                    CardGroup = report.CardGroup,
                    UserCardViews = report.UserCardViews,
                    UserCardLikes = report.UserCardLikes
                };
                result.Add(card);
            }

            return result.AsQueryable();
        }

        /*public async Task<EditableQuestionVM> GetEditableQuestion(int idQuestion)
        {

            var result = await DbContext.Set<Card>().Include(question => question.Choices)
                .Where(question => question.Id == idQuestion)
                .Select(question => new EditableQuestionVM(
                    question.Id, question.Content, question.PublicationDate,
                    question.EndDate, null, question.Picture.PicBase64, question.Choices
                ))
                .FirstOrDefaultAsync();

            result.TargetGroupsIds = new HashSet<int>();
            var groups = await DbContext.Cards.Where(c => c.Id == idQuestion)
                .Select(c => c.cardGroup)
                .FirstOrDefaultAsync();

            groups.ForEach(cg =>
            {
                result.TargetGroupsIds.Add(cg.groupId);
            });

            return result;
        }*/

        #region HELPERS
        public async Task InsertQuestion(Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            //Attach creator to DbCOntext to avoid creation of a new user
            if (question.CreatedById != Guid.Empty)
            {
                var createdBy = DbContext.UserProfiles.Find(question.CreatedById);
                DbContext.UserProfiles.Attach(createdBy);
            }

            //Attach Picture to avoid creation of a new picture
            //Create the picture if not the default picture
            if (question?.Picture?.PicBase64 != null)
            {
                await DbContext.Set<Picture>().AddAsync(question.Picture);
                await DbContext.SaveChangesAsync();
                question.PictureId = question.Picture.Id;
            }
            else
                question.PictureId = Program.QuestionDefaultPicId;
            
            
            var picture = DbContext.Pictures.Find(question.PictureId);
            DbContext.Pictures.Attach(picture);

            //get all groups referenced by the idea (targetGroups)
            List<Group> groups = new List<Group>();
            foreach (CardGroup item in question.CardGroup)
                groups.Add(DbContext.Groups.Find(item.GroupId));

            //Attach them all to avoid creation of groups
            foreach (Group gr in groups)
                DbContext.Groups.Attach(gr);

            question.PublicationDate = DateTime.Now < question.PublicationDate ? question.PublicationDate.Date.AddHours(1) : question.PublicationDate;
            question.EndDate = question.EndDate.Date.AddHours(23);

            //create the associated reporting
            var Reporting = new Reporting
            {
                Views = 0,
                Answers = 0,
                CreatedById = question.CreatedById,
                Content = question.Content,
                Type = CardTypes.Reporting.ToString(),
                PublicationDate = question.PublicationDate,
                PictureId = question.PictureId,
                EndDate = question.EndDate.AddDays(1)
            };

            Reporting.Choices = new HashSet<Choice>();
            Reporting.Choices = question.Choices;

            DbContext.Cards.Add(question);
            DbContext.Cards.Add(Reporting);

            if (await DbContext.SaveChangesAsync() != 0)
            {
                Reporting.LinkedCardId = question.Id;
                question.LinkedCardId = Reporting.Id;

                //create the associated results
                var Results = new Result[question.Choices.Count];
                for (var i = 0; i < question.Choices.Count; i++)
                {
                    var result = new Result
                    {
                        ReportingId = Reporting.Id,
                        Value = 0,
                        Choice = question.Choices.ElementAt(i)
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
            }

        public async Task UpdateQuestion(Question oldQuestion, Question question)
        {
                if (question == null)
                {
                    throw new ArgumentNullException(nameof(question));
                }

                //if the picture changed
                if (question.PictureId != Guid.Empty)
                {
                    //if the picture changed, remove the old one from the database
                    var oldCardPictureId = oldQuestion.PictureId;
                    if (oldCardPictureId != Program.SurveyDefaultPicId)
                        DbContext.Pictures.Remove(DbContext.Pictures.Find(oldCardPictureId));
                    oldQuestion.PictureId = question.PictureId;
                }



                await DbContext.SaveChangesAsync();

                //get the reporting of the concerned card
                var report = DbContext.Set<Reporting>()
                    .Where(r => r.LinkedCardId == oldQuestion.Id)
                    .Include(r => r.Choices)
                    .FirstOrDefault();

                //update the report with new infos
                report.PictureId = oldQuestion.PictureId;
                report.Content = question.Content;
                report.PublicationDate = oldQuestion.PublicationDate.Date == question.PublicationDate.Date ? oldQuestion.PublicationDate : question.PublicationDate.Date.AddHours(1);
                report.EndDate = oldQuestion.EndDate.Date == question.EndDate.Date ? report.EndDate : question.EndDate.Date.AddDays(1).AddHours(23);

                //update the card itself
                oldQuestion.Content = question.Content;
                oldQuestion.PublicationDate = report.PublicationDate;
                oldQuestion.EndDate = oldQuestion.EndDate.Date == question.EndDate.Date ? oldQuestion.EndDate : question.EndDate.Date.AddHours(23);

                //keep the old targetted groups
                var cardGroups = new List<CardGroup>();
                cardGroups.AddRange(oldQuestion.CardGroup);

                //tests on those targetted groups
                foreach (CardGroup gr in cardGroups)
                {
                    //if this group is also targetted by the new entity, do not change it
                    if (question.CardGroup.Exists(g => g.GroupId == gr.GroupId))
                    {
                        var cardGroup = DbContext.Set<CardGroup>().AsNoTracking().Where(g => g.GroupId == gr.GroupId).Where(g => g.CardId == oldQuestion.Id).FirstOrDefault();
                        var reportGroup = DbContext.Set<CardGroup>().AsNoTracking().Where(g => g.GroupId == gr.GroupId).Where(g => g.CardId == report.Id).FirstOrDefault();
                    }
                    //if it's not, remove it because it's not targeted anymore 
                    else
                    {
                        var group = DbContext.Groups.Find(gr.GroupId);
                        DbContext.Groups.Attach(group);
                        var cardGroup = DbContext.Set<CardGroup>().Where(g => g.GroupId == gr.GroupId).Where(g => g.CardId == oldQuestion.Id).FirstOrDefault();
                        DbContext.Set<CardGroup>().Remove(cardGroup);
                        oldQuestion.CardGroup.Remove(gr);
                        var reportGroup = DbContext.Set<CardGroup>().Where(g => g.GroupId == gr.GroupId).Where(g => g.CardId == report.Id).FirstOrDefault();
                        report.CardGroup.Remove(reportGroup);
                    }

                }
                await DbContext.SaveChangesAsync();

                //now tests on the new entity's target groups
                foreach (CardGroup gr in question.CardGroup)
                {
                    if (!oldQuestion.CardGroup.Exists(g => g.GroupId == gr.GroupId))
                    {
                        var group = DbContext.Groups.Find(gr.GroupId);
                        DbContext.Groups.Attach(group);
                        var groupCard = new CardGroup
                        {
                            CardId = oldQuestion.Id,
                            GroupId = gr.GroupId
                        };
                        var groupReport = new CardGroup
                        {
                            CardId = report.Id,
                            GroupId = gr.GroupId
                        };
                        report.CardGroup.Add(groupReport);
                        oldQuestion.CardGroup.Add(groupCard);
                        DbContext.Set<CardGroup>().Add(groupReport);
                        DbContext.Set<CardGroup>().Add(groupCard);
                    }
                }

                //Update Choices
                //var newChoices = new HashSet<Choice>();
                foreach (Choice choice in question.Choices)
                {
                    var choiceItem = DbContext.Set<Choice>().Where(c => c.Id == choice.Id).FirstOrDefault();
                    if (choiceItem != null)
                    {
                        if (choiceItem.Name != choice.Name)
                        {
                            //newChoices.Add(choice);
                            choiceItem.Name = choice.Name;
                            DbContext.Set<Choice>().Update(choiceItem);
                        }
                        else
                        {
                            //newChoices.Add(choiceItem);
                        }
                    }
                    else
                    {
                        //newChoices.Add(choiceItem);
                        oldQuestion.Choices.Add(choice);
                        //report.Choices.Add(choice);
                        var res = new Result
                        {
                            Choice = choice
                        };
                        report.Results.Add(res);
                        DbContext.Set<Choice>().Add(choice);

                    }
                }

                // Remove the Choice that it's not targeted anymore
                foreach (Choice choice in oldQuestion.Choices)
                {
                    var list = question.Choices.ToList();
                    var item = list.Find(c => c.Id == choice.Id);
                    if (item == null)
                    {
                        //oldEntity.Choices.Remove(choice);
                        DbContext.Set<Choice>().Remove(choice);
                    }

                }


                await DbContext.SaveChangesAsync();
        }

        public async Task<string> GetContentsurvey(Guid surveyId)
        {
            return await DbContext.Set<Card>().Where(s => s.Id == surveyId).Select(s => s.Content).FirstOrDefaultAsync();
        }

        public async Task<int> GetMaxQuestions(Guid surveyId)
        {
            var card = await DbContext.Set<Card>().Where(c => c.Id == surveyId).FirstOrDefaultAsync();
            if(card.Type == CardTypes.Survey.ToString())
                return await DbContext.Set<Question>().Where(q => q.SurveyId == surveyId).CountAsync();
            else
            {
                var id = await DbContext.Set<Survey>().Where(r => r.LinkedCardId == surveyId).Select(c => c.Id).FirstOrDefaultAsync();
                return await DbContext.Set<Question>().Where(q => q.SurveyId == id).CountAsync();
            }
        }

        public async Task<string> GetPictureSurvey(Guid surveyReportId)
        {
            var picture = await DbContext.Set<Card>().Where(r => r.Id == surveyReportId).Include(r => r.Picture).Select(r => r.Picture.PicBase64).FirstOrDefaultAsync();
            return picture;
        }
        #endregion
    }
}
