using System;
using System.Collections.Generic;
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

namespace Hive.Backend.Api.QuestionController
{
    /// <summary>
    /// Controller responsible for all data interactions with the IdeaController
    /// </summary>
    [Route("api/v1/HiveQuestionController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class QuestionController : Controller
    {
        private readonly IPictureService _PictureService;
        private readonly ICardService _CardService;
        private readonly IQuestionService _QuestionService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="MeteoMeter.Backend.Api.QuestionController"/> class.
        /// </summary>
        public QuestionController(IQuestionService QuestionService, IPictureService PictureService, ICardService CardService, ILogger<QuestionController> logger)
        {
            _CardService = CardService;
            _PictureService = PictureService;
            _QuestionService = QuestionService;
            _logger = logger;
        }


        [HttpPost]
        [Route("/api/question/create")]
        public async Task<IActionResult> Create([FromBody] CreateQuestionVM question)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newQuestion = await _QuestionService.InsertQuestion(ConvertTo(question));
                return this.Ok(ConvertToTopPostVM(newQuestion));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpPut]
        [Route("/api/question/update")]
        public async Task<IActionResult> Update([FromBody] EditQuestionVM questionVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _QuestionService.Save(ConvertTo(questionVM));
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
        [Route("/api/question/getDefaultPicture")]
        public async Task<IActionResult> GetDefaultPicture()
        {
            try
            {
                var picture = await _PictureService.GetById(Program.QuestionDefaultPicId);
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
        [Route("/api/question/getEditableQuestion")]
        public async Task<IActionResult> GetEditableQuestion(string questionId)
        {
            var canParse = Guid.TryParse(questionId, out var questionGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var editableIdeaVM = await _QuestionService.GetEditableQuestion(questionGuid);
                if (editableIdeaVM == null)
                    return NoContent();

                return this.Ok(await Task.FromResult(editableIdeaVM));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        #region
        private Question ConvertTo(CreateQuestionVM questionVM)
        {
            var question = new Question
            {
                Content = questionVM.Content,
                PublicationDate = questionVM.PublicationDate,
                EndDate = questionVM.EndDate,
                CreatedById = Guid.Parse(questionVM.AuthorId),
                PictureId = string.IsNullOrEmpty(questionVM.PictureId) ? Guid.Empty : Guid.Parse(questionVM.PictureId),
                Type = CardTypes.Question.ToString()
            };
            question.CardGroup = new List<CardGroup>();
            foreach (var groupId in questionVM.TargetGroupsIds)
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

            return question;
        }

        private Question ConvertTo(EditQuestionVM questionVM)
        {
            var question = new Question()
            {
                Id = Guid.Parse(questionVM.Id),
                Content = questionVM.Content,
                PublicationDate = questionVM.PublicationDate,
                EndDate = questionVM.EndDate,
                CardGroup = new List<CardGroup>(),
                PictureId = string.IsNullOrEmpty(questionVM.PictureId) ? Guid.Empty : Guid.Parse(questionVM.PictureId),
                //Choices = questionVM.Choices
            };
            if (questionVM.Choices != null)
            {
                foreach(var choice in questionVM.Choices)
                {
                    question.Choices.Add(choice);
                }
                
            }
            if (questionVM.TargetGroupsIds != null)
            {
                foreach (var item in questionVM.TargetGroupsIds)
                {
                    CardGroup cardGroup = new CardGroup();
                    cardGroup.GroupId = Guid.Parse(item);
                    cardGroup.CardId = Guid.Parse(questionVM.Id);

                    question.CardGroup.Add(cardGroup);
                }
            }


            return question;
        }

        private TopPostVM ConvertToTopPostVM(Question question)
        {
            return new TopPostVM()
            {
                Id = question.Id.ToString(),
                Content = question.Content,
                PublicationDate = question.PublicationDate,
                EndDate = question.EndDate,
                Picture = question.Picture.PicBase64,
                Status = _CardService.GetStatus(question),
                Type = question.Type,
                Views = 0,
                Answers = 0
            };
        }
        #endregion
    }
}
