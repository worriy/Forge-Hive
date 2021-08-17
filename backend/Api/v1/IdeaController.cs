using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hive.Backend.DataModels;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using System.Collections.Generic;

namespace Hive.Backend.Api.IdeaController
{
    /// <summary>
    /// Controller responsible for all data interactions with the IdeaController
    /// </summary>
    [Route("api/v1/HiveIdeaController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class IdeaController : Controller
    {
        private readonly IIdeaService _IdeaService;
        private readonly IPictureService _PictureService;
        private readonly ICardService _CardService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.IdeaControllerController"/> class.
        /// </summary>
        public IdeaController(IIdeaService IdeaService, IPictureService PictureService, ICardService CardService, ILogger<IdeaController> logger)
        {
            _CardService = CardService;
            _IdeaService = IdeaService;
            _PictureService = PictureService;
            _logger = logger;
        }

        [HttpPost]
        [Route("/api/idea/create")]
        public async Task<IActionResult> Create([FromBody] CreateIdeaVM ideaVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newIdea = await _IdeaService.InsertIdea(ConvertToIdea(ideaVM));

                return this.Ok(ConvertToTopPostVM(newIdea));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/api/idea/getEditableIdea")]
        public async Task<IActionResult> GetEditableIdea(string ideaId)
        {
            bool canParse = Guid.TryParse(ideaId, out var ideaGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var editableIdeaVM = await _IdeaService.GetEditableIdea(ideaGuid);

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

        [HttpPut]
        [Route("/api/idea/update")]
        public async Task<IActionResult> Update([FromBody] EditIdeaVM ideaVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _IdeaService.UpdateIdea(ConvertToIdea(ideaVM));

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
        [Route("/api/idea/getDefaultPicture")]
        [ProducesResponseType(typeof(PictureVM), 200)]
        public async Task<IActionResult> GetDefaultPicture()
        {
            try
            {
                var picture = await _PictureService.GetById(Program.IdeaDefaultPicId);
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

        #region HELPER
        private Idea ConvertToIdea(CreateIdeaVM ideaVM)
        {
            var idea = new Idea
            {
                Content = ideaVM.Content,
                PublicationDate = ideaVM.PublicationDate,
                EndDate = ideaVM.EndDate,
                CreatedById = Guid.Parse(ideaVM.AuthorId),
                PictureId = string.IsNullOrEmpty(ideaVM.PictureId) ? Guid.Empty : Guid.Parse(ideaVM.PictureId),
                Type = CardTypes.Idea.ToString()
            };
            idea.Choices.Add(new Choice
            {
                Name = "Yes"
            });
            idea.Choices.Add(new Choice
            {
                Name = "No"
            });
            idea.CardGroup = new List<CardGroup>();
            foreach (var groupId in ideaVM.TargetGroupsIds)
            {
                idea.CardGroup.Add(new CardGroup(Guid.Empty, Guid.Parse(groupId)));
            };
            return idea;
        }

        private Idea ConvertToIdea(EditIdeaVM ideaVM)
        {
            var model = new Idea()
            {
                Id = Guid.Parse(ideaVM.Id),
                Content = ideaVM.Content,
                PublicationDate = ideaVM.PublicationDate,
                EndDate = ideaVM.EndDate,
                CardGroup = new List<CardGroup>(),
                PictureId = string.IsNullOrEmpty(ideaVM.PictureId) ? Guid.Empty : Guid.Parse(ideaVM.PictureId)
            };

            if (ideaVM.TargetGroupsIds != null)
            {
                foreach (var item in ideaVM.TargetGroupsIds)
                {
                    CardGroup cardGroup = new CardGroup();
                    cardGroup.GroupId = Guid.Parse(item);
                    cardGroup.CardId = Guid.Parse(ideaVM.Id);

                    model.CardGroup.Add(cardGroup);
                }
            }
            return model;
        }

        private TopPostVM ConvertToTopPostVM(Idea idea)
        {
            return new TopPostVM()
            {
                Id = idea.Id.ToString(),
                Content = idea.Content,
                PublicationDate = idea.PublicationDate,
                EndDate = idea.EndDate,
                Picture = idea.Picture.PicBase64,
                Status = _CardService.GetStatus(idea),
                Type = idea.Type,
                Views = 0,
                Answers = 0
            };
        }
        #endregion
    }
}