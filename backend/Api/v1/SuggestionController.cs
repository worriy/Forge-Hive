using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Hive.Backend.Api.SuggestionController
{
    [Route("api/v1/HiveSuggestionController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class SuggestionController : Controller
    {
        private readonly IPictureService _PictureService;
        private readonly ICardService _CardService;
        private readonly ISuggestionService _SuggestionService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="MeteoMeter.Backend.Api.QuestionController"/> class.
        /// </summary>
        public SuggestionController(ISuggestionService SuggestionService, IPictureService PictureService, ICardService CardService, ILogger<SuggestionController> logger)
        {
            _CardService = CardService;
            _PictureService = PictureService;
            _SuggestionService = SuggestionService;
            _logger = logger;
        }


        [HttpPost]
        [Route("/api/suggestion/create")]
        public async Task<IActionResult> Create([FromBody] CreateSuggestionVM suggestion)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newSuggestion = await _SuggestionService.InsertSuggestion(ConvertTo(suggestion));

                return this.Ok(ConvertToTopPostVM(newSuggestion));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpPut]
        [Route("/api/suggestion/update")]
        public async Task<IActionResult> Update([FromBody] EditSuggestionVM suggestionVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _SuggestionService.Save(ConvertTo(suggestionVM));

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
        [Route("/api/suggestion/getDefaultPicture")]
        public async Task<IActionResult> GetDefaultPicture()
        {
            try
            {
                var picture = await _PictureService.GetById(Program.SuggestionDefaultPicId);
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
        [Route("/api/suggestion/getEditableSuggestion")]
        public async Task<IActionResult> GetEditableSuggestion(string suggestionId)
        {
            var canParse = Guid.TryParse(suggestionId, out var suggestionGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var editableSuggestionVM = await _SuggestionService.GetEditableSuggestion(suggestionGuid);
                if (editableSuggestionVM == null)
                    return NoContent();
                
                return this.Ok(await Task.FromResult(editableSuggestionVM));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        #region HELPER
        private Suggestion ConvertTo(CreateSuggestionVM suggestionVM)
        {
            return new Suggestion
            {
                Content = suggestionVM.Content,
                PublicationDate = suggestionVM.PublicationDate,
                EndDate = suggestionVM.EndDate,
                CreatedById = Guid.Parse(suggestionVM.AuthorId),
                PictureId = string.IsNullOrEmpty(suggestionVM.PictureId) ? Guid.Empty : Guid.Parse(suggestionVM.PictureId),
                Type = CardTypes.Suggestion.ToString()
            };
        }

        private Suggestion ConvertTo(EditSuggestionVM suggestionVM)
        {
            return new Suggestion
            {
                Id = Guid.Parse(suggestionVM.Id),
                Content = suggestionVM.Content,
                PublicationDate = suggestionVM.PublicationDate,
                EndDate = suggestionVM.EndDate,
                PictureId = string.IsNullOrEmpty(suggestionVM.PictureId) ? Guid.Empty : Guid.Parse(suggestionVM.PictureId)
            };
        }
        private TopPostVM ConvertToTopPostVM(Suggestion suggestion)
        {
            return new TopPostVM()
            {
                Id = suggestion.Id.ToString(),
                Content = suggestion.Content,
                PublicationDate = suggestion.PublicationDate,
                EndDate = suggestion.EndDate,
                Picture = suggestion.Picture.PicBase64,
                Status = _CardService.GetStatus(suggestion),
                Type = suggestion.Type,
                Views = 0,
                Answers = 0
            };
        }
        #endregion
    }
}
