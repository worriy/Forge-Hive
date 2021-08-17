using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hive.Backend.Api.SurveyController
{
    [Route("api/v1/HiveSurveyController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class SurveyController : Controller
    {
        private readonly IPictureService _PictureService;
        private readonly ICardService _CardService;
        private readonly ISurveyService _SurveyService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="MeteoMeter.Backend.Api.QuestionController"/> class.
        /// </summary>
        public SurveyController(ISurveyService SurveyService, ICardService CardService, IPictureService PictureService, ILogger<SurveyController> logger)
        {
            _PictureService = PictureService;
            _SurveyService = SurveyService;
            _CardService = CardService;
            _logger = logger;
        }


        [HttpPost]
        [Route("/api/survey/create")]
        public async Task<IActionResult> Create([FromBody] CreateSurveyVM survey)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newSurvey = await _SurveyService.InsertSurvey(ConvertTo(survey), ConvertToQuestions(survey));
                
                return this.Ok(ConvertToTopPostVM(newSurvey));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Route("/api/survey/update")]
        public async Task<IActionResult> Update([FromBody] EditSurveyVM surveyVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _SurveyService.Save(ConvertTo(surveyVM), ConvertToQuestions(surveyVM));

                return this.Ok(true);
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/api/survey/getDefaultPicture")]
        public async Task<IActionResult> GetDefaultPicture()
        {
            try
            {
                var picture = await _PictureService.GetById(Program.SurveyDefaultPicId);
                var pictureVM = new PictureVM()
                {
                    Picture = picture.PicBase64
                };
                return this.Ok(pictureVM);
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/api/survey/getEditableSurvey")]
        public async Task<IActionResult> GetEditableSurvey(string surveyId)
        {
            var canParse = Guid.TryParse(surveyId, out var surveyGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var editableSurveyVM = await _SurveyService.GetEditableSurvey(surveyGuid);
                if (editableSurveyVM == null)
                    return NoContent();
                
                return this.Ok(await Task.FromResult(editableSurveyVM));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/api/survey/getSurveyquestions")]
        public async Task<IActionResult> GetSurveyquestions(string surveyId)
        {
            var canParse = Guid.TryParse(surveyId, out var surveyGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var list = new List<SurveyVM>();
                var questionsSurvey = await _SurveyService.GetSurveyquestions(surveyGuid);
                var surveyContent = await _SurveyService.GetContentsurvey(surveyGuid);
                var maxQuestions = await _SurveyService.GetMaxQuestions(surveyGuid);
                if (questionsSurvey != null)
                {
                    int index = 1;
                    foreach (Card item in questionsSurvey)
                    {
                        var cardVM = new SurveyVM().ConvertFromModel(item);
                        cardVM.TargetGroups = await _CardService.GetTargetGroupsString(Guid.Parse(cardVM.Id));
                        cardVM.Results = await _CardService.GetResults(Guid.Parse(cardVM.Id));
                        cardVM.ContentSurvey = surveyContent;
                        cardVM.MaxQuestions = maxQuestions;
                        cardVM.QuestionNumber = index;
                        list.Add(cardVM);
                        index++;
                    }

                    return this.Ok(list);
                }
                    

                return NoContent();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/api/survey/getSurveyReportsQuestions")]
        public async Task<IActionResult> GetSurveyReportsQuestions(string surveyReportId)
        {
            var canParse = Guid.TryParse(surveyReportId, out var surveyReportGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var list = new List<SurveyVM>();
                var questionsSurvey = await _SurveyService.GetSurveyReportsquestions(surveyReportGuid);
                var surveyContent = await _SurveyService.GetContentsurvey(surveyReportGuid);
                var maxQuestions = await _SurveyService.GetMaxQuestions(surveyReportGuid);
                //var pictureSurvey = await _SurveyService.GetPictureSurvey(surveyReportId);
                if (questionsSurvey != null)
                {
                    int index = 1;
                    foreach (var item in questionsSurvey)
                    {
                        var cardVM = new SurveyVM().ConvertFromModel(item);
                        cardVM.TargetGroups = await _CardService.GetTargetGroupsString(Guid.Parse(cardVM.Id));
                        cardVM.Results = await _CardService.GetResults(Guid.Parse(cardVM.Id));
                        cardVM.ContentSurvey = surveyContent;
                        cardVM.MaxQuestions = maxQuestions;
                        //cardVM.PictureSurvey = pictureSurvey;
                        cardVM.QuestionNumber = index;
                        cardVM.Answers = await _CardService.GetAnswersNumber(Guid.Parse(cardVM.LinkedCardId));
                        cardVM.Views = await _CardService.GetViewsNumber(Guid.Parse(cardVM.LinkedCardId));
                        list.Add(cardVM);
                        index++;
                    }

                    return this.Ok(list);
                }


                return NoContent();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        #region HELPER
        private Survey ConvertTo(CreateSurveyVM survey)
        {
            var model = new Survey()
            {
                Content = survey.Content,
                PublicationDate = survey.PublicationDate,
                EndDate = survey.EndDate,
                CreatedById = Guid.Parse(survey.AuthorId),
                PictureId = string.IsNullOrEmpty(survey.PictureId) ? Guid.Empty : Guid.Parse(survey.PictureId),
                Type = CardTypes.Survey.ToString()
            };

            model.CardGroup = new List<CardGroup>();
            foreach (var groupId in survey.TargetGroupsIds)
            {
                model.CardGroup.Add(new CardGroup(Guid.Empty, Guid.Parse(groupId)));
            }

            return model;
        }

        private Survey ConvertTo(EditSurveyVM survey)
        {
            var model = new Survey()
            {
                Id = Guid.Parse(survey.Id),
                Content = survey.Content,
                PublicationDate = survey.PublicationDate,
                EndDate = survey.EndDate,
                CardGroup = new List<CardGroup>(),
                PictureId = string.IsNullOrEmpty(survey.PictureId) ? Guid.Empty : Guid.Parse(survey.PictureId)
            };
            if (survey.TargetGroupsIds != null)
            {
                foreach (var item in survey.TargetGroupsIds)
                {
                    CardGroup cardGroup = new CardGroup();
                    cardGroup.GroupId = Guid.Parse(item);
                    cardGroup.CardId = Guid.Parse(survey.Id);

                    model.CardGroup.Add(cardGroup);
                }
            }
            return model;
        }

        private List<Question> ConvertToQuestions(CreateSurveyVM survey)
        {
            List<Question> model = new List<Question>();
            //model = survey.Questions.ToList();
            foreach (var questionVM in survey.Questions.ToList())
            {
                var question = new Question
                {
                    Content = questionVM.Content,
                    PublicationDate = survey.PublicationDate,
                    EndDate = survey.EndDate,
                    CreatedById = Guid.Parse(survey.AuthorId),
                    PictureId = string.IsNullOrEmpty(questionVM.PictureId) ? Guid.Empty : Guid.Parse(questionVM.PictureId),
                    Type = CardTypes.Question.ToString()
                };
                question.CardGroup = new List<CardGroup>();
                foreach (var groupId in survey.TargetGroupsIds)
                {
                    question.CardGroup.Add(new CardGroup(Guid.Empty, Guid.Parse(groupId)));
                }
                question.Choices = new HashSet<Choice>();
                foreach (string name in questionVM.Choices)
                {
                    var choice = new Choice
                    {
                        Name = name
                    };
                    question.Choices.Add(choice);
                }
                /*foreach (var groupId in survey.TargetGroupsIds)
                {
                    question.CardGroup = new List<CardGroup>();
                    question.CardGroup.Add(new CardGroup(Guid.Empty, Guid.Parse(groupId)));
                    question.Type = CardTypes.Question.ToString();
                    question.CreatedById = Guid.Parse(survey.AuthorId);
                    question.PublicationDate = survey.PublicationDate;
                    question.EndDate = survey.EndDate;

                }*/
                model.Add(question);
            }

            return model;
        }

        private List<Question> ConvertToQuestions(EditSurveyVM survey)
        {
            var model = new List<Question>();
            model = survey.Questions.ToList();

            if (survey.TargetGroupsIds != null)
            {
                foreach (Question question in model)
                {
                    foreach (var groupId in survey.TargetGroupsIds)
                    {
                        question.CardGroup = new List<CardGroup>();
                        CardGroup cardGroup = new CardGroup();
                        cardGroup.GroupId = Guid.Parse(groupId);
                        cardGroup.CardId = Guid.Parse(survey.Id);

                        question.CardGroup.Add(cardGroup);
                    }
                }
            }

            return model;
        }

        private TopPostVM ConvertToTopPostVM(Survey survey)
        {
            return new TopPostVM()
            {
                Id = survey.Id.ToString(),
                Content = survey.Content,
                PublicationDate = survey.PublicationDate,
                EndDate = survey.EndDate,
                Picture = survey.Picture.PicBase64,
                Status = _CardService.GetStatus(survey),
                Type = survey.Type,
                Views = 0,
                Answers = 0
            };
        }
        #endregion
    }
}
