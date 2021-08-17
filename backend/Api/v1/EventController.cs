using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hive.Backend.Api.EventController
{
    [Route("api/v1/HiveEventController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public partial class EventController : Controller
    {
        private readonly IPictureService _PictureService;
        private readonly ICardService _CardService;
        private readonly IEventService _EventService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="MeteoMeter.Backend.Api.QuestionController"/> class.
        /// </summary>
        public EventController(IEventService EventService, IPictureService PictureService, ICardService CardService, ILogger<EventController> logger)
        {
            _CardService = CardService;
            _PictureService = PictureService;
            _EventService = EventService;
            _logger = logger;
        }


        [HttpPost]
        [Route("/api/event/create")]
        public async Task<IActionResult> Create([FromBody] CreateEventVM eventVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newEvent = await _EventService.InsertEvent(ConvertTo(eventVM));
                return this.Ok(ConvertToTopPostVM(newEvent));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpPut]
        [Route("/api/event/update")]
        public async Task<IActionResult> Update([FromBody] EditEventVM eventVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _EventService.Save(ConvertTo(eventVM));

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
        [Route("/api/event/getDefaultPicture")]
        public async Task<IActionResult> GetDefaultPicture()
        {
            try
            {
                var picture = await _PictureService.GetById(Program.EventDefaultPicId);
                var pictureVM = new PictureVM
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
        [Route("/api/event/getEditableEvent")]
        public async Task<IActionResult> GetEditableEvent(string eventId)
        {
            var canParse = Guid.TryParse(eventId, out var eventGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var editableEventVM = await _EventService.GetEditableEvent(eventGuid);

                if (editableEventVM == null)
                    return NoContent();

                return this.Ok(await Task.FromResult(editableEventVM));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        #region HELPER
        private Event ConvertTo(CreateEventVM ev)
        {
            var model = new Event
            {
                Content = ev.Content,
                PublicationDate = ev.PublicationDate,
                EndDate = ev.EndDate,
                CreatedById = Guid.Parse(ev.AuthorId),
                PictureId = string.IsNullOrEmpty(ev.PictureId) ? Guid.Empty : Guid.Parse(ev.PictureId),
                Type = CardTypes.Event.ToString()
            };
            model.Choices.Add(new Choice
            {
                Name = "Applause"
            });
            model.CardGroup = new List<CardGroup>();
            foreach (var groupId in ev.TargetGroupsIds)
            {
                model.CardGroup.Add(new CardGroup(Guid.Empty, Guid.Parse(groupId)));
            }

            return model;
        }

        private Event ConvertTo(EditEventVM ev)
        {
            var model = new Event()
            {
                Id = Guid.Parse(ev.Id),
                Content = ev.Content,
                PublicationDate = ev.PublicationDate,
                EndDate = ev.EndDate,
                CardGroup = new List<CardGroup>(),
                PictureId = string.IsNullOrEmpty(ev.PictureId) ? Guid.Empty : Guid.Parse(ev.PictureId)
            };
            if (ev.TargetGroupsIds != null)
            {
                foreach (var item in ev.TargetGroupsIds)
                {
                    CardGroup cardGroup = new CardGroup();
                    cardGroup.GroupId = Guid.Parse(item);
                    cardGroup.CardId = Guid.Parse(ev.Id);

                    model.CardGroup.Add(cardGroup);
                }
            }


            return model;
        }

        private TopPostVM ConvertToTopPostVM(Event ev)
        {
            return new TopPostVM()
            {
                Id = ev.Id.ToString(),
                Content = ev.Content,
                PublicationDate = ev.PublicationDate,
                EndDate = ev.EndDate,
                Picture = ev.Picture.PicBase64,
                Status = _CardService.GetStatus(ev),
                Type = ev.Type,
                Views = 0,
                Answers = 0
            };
        }
        #endregion
    }
}
