using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Hive.Backend.DataModels;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Hive.Backend.Api.FlowController
{
    /// <summary>
    /// Controller responsible for all data interactions with the FlowController
    /// </summary>
    [Route("api/v1/HiveFlowController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class FlowController : Controller
    {
        private readonly ICardService _CardService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.FlowControllerController"/> class.
        /// </summary>
        public FlowController(ICardService CardService, ILogger<FlowController> logger)
        {
            _CardService = CardService;
            _logger = logger;
        }

        [HttpGet]
        [Route("/api/flow/list")]
        [ProducesResponseType(typeof(CardVM), 200)]
        public async Task<IActionResult> List(PagingVM paging, string userProfileId)
        {
            //TODO: A tester
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!ModelState.IsValid || !canParse)
                return BadRequest();

            try
            {
                var list = new List<CardVM>();
                var result = await _CardService.GetFlowCards(paging, userProfileGuid);
                if (result == null)
                    return NoContent();

                foreach (Card item in result)
                {
                    var cardVM = new CardVM().ConvertFromModel(item);
                    cardVM.TargetGroups = await _CardService.GetTargetGroupsString(Guid.Parse(cardVM.Id));
                    cardVM.Results = await _CardService.GetResults(Guid.Parse(cardVM.Id));
                    list.Add(cardVM);
                }

                return this.Ok(list);
            }
            catch (Exception xcp)
            {
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty<CardVM>().AsQueryable());
            }
        }

        [HttpPut]
        [Route("/api/flow/addView")]
        public async Task<IActionResult> AddView([FromBody] UserViewCardVM userViewCard)
        {
            //TODO A tester

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _CardService.AddView(Guid.Parse(userViewCard.CardId), Guid.Parse(userViewCard.UserId));

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
        [Route("/api/flow/checkDeletedCards")]
        [ProducesResponseType(typeof(int[]), 200)]
        public async Task<IActionResult> CheckDeletedCards(PagingVM paging, string userProfileId)
        {
            //TODO: A tester
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!ModelState.IsValid || !canParse)
                return BadRequest();

            try
            {
                var list = new List<int>();
                var result = await _CardService.CheckDeletedCards(paging, userProfileGuid);
                if (result == null)
                    return NoContent();

                return this.Ok(result);
            }
            catch (Exception xcp)
            {
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty<CardVM>().AsQueryable());
            }
        }

        [HttpPut]
        [Route("/api/flow/like")]
        public async Task<IActionResult> Like([FromBody] UserViewCardVM userLikeCard)
        {
            //TODO A tester

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _CardService.Like(Guid.Parse(userLikeCard.CardId), Guid.Parse(userLikeCard.UserId));

                return this.Ok(true);
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Route("/api/flow/dislike")]
        public async Task<IActionResult> Dislike([FromBody] UserViewCardVM userLikeCard)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _CardService.Dislike(Guid.Parse(userLikeCard.CardId), Guid.Parse(userLikeCard.UserId));

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
        [Route("/api/flow/checkLikedCard")]
        [ProducesResponseType(typeof(int[]), 200)]
        public async Task<IActionResult> CheckLikedCard(string cardId, string userId)
        {
            //TODO: A tester
            var canParseCardId = Guid.TryParse(cardId, out var cardGuid);
            var canParseUserId = Guid.TryParse(userId, out var userGuid);
            if (!canParseCardId || !canParseUserId)
                return BadRequest();

            try
            {
                var result = await _CardService.CheckLikedCard(cardGuid, userGuid);
                return this.Ok(result);
            }
            catch (Exception xcp)
            {
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty<CardVM>().AsQueryable());
            }
        }

        [HttpPost]
        [Route("/api/flow/createMood")]
        public async Task<IActionResult> Create([FromBody] MoodVM mood)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var moodModel = mood.GetMoodFromViewModel();
                Card result = await _CardService.CreateMoodCard(moodModel);
                CardVM moodVM = new CardVM();
                moodVM.ConvertFromModel(result);
                return this.Ok(moodVM);
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}