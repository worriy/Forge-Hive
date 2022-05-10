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
using Hive.Backend.Models;

namespace Hive.Backend.Api.FlowController
{
    /// <summary>
    /// Controller responsible for all data interactions with the FlowController
    /// </summary>
    [Route("api/flow")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class FlowController : Controller
    {
        private readonly ICardService _cardService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.FlowControllerController"/> class.
        /// </summary>
        public FlowController(ICardService cardService, ILogger<FlowController> logger)
        {
            _cardService = cardService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CardVM), 200)]
        public async Task<IActionResult> List(string userProfileId)
        {
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!ModelState.IsValid || !canParse)
                return BadRequest();

            try
            {
                var result = await _cardService.GetFlowCards(userProfileGuid);
                if (result == null)
                    return NoContent();

                var list = result.Select(c => ConvertTo(c, userProfileId, _cardService))
                    .ToList();

                return this.Ok(list);
            }
            catch (Exception xcp)
            {
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty<CardVM>().AsQueryable());
            }
        }

        [HttpPut("addView")]
        public async Task<IActionResult> AddView([FromBody] UserViewCardVM userViewCard)
        {
            var canParseCardId = Guid.TryParse(userViewCard.CardId, out var cardGuid);
            var canParseUserId = Guid.TryParse(userViewCard.UserId, out var userGuid);
            if (!ModelState.IsValid || !canParseCardId || !canParseUserId)
                return BadRequest(ModelState);

            try
            {
                await _cardService.AddView(cardGuid, userGuid);

                return this.Ok();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("checkDeletedCards/{userProfileId}")]
        public async Task<IActionResult> CheckDeletedCards(string userProfileId)
        {
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!ModelState.IsValid || !canParse)
                return BadRequest();

            try
            {
                var result = await _cardService.CheckDeletedCards(userProfileGuid);
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

        [HttpPut("like")]
        public async Task<IActionResult> Like([FromBody] UserViewCardVM userLikeCard)
        {
            var canParseCardId = Guid.TryParse(userLikeCard.CardId, out var cardGuid);
            var canParseUserId = Guid.TryParse(userLikeCard.UserId, out var userGuid);
            if (!ModelState.IsValid || !canParseCardId || !canParseUserId)
                return BadRequest(ModelState);

            try
            {
                await _cardService.Like(cardGuid, userGuid);

                return this.Ok();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("dislike")]
        public async Task<IActionResult> Dislike([FromBody] UserViewCardVM userLikeCard)
        {
            var canParseCardId = Guid.TryParse(userLikeCard.CardId, out var cardGuid);
            var canParseUserId = Guid.TryParse(userLikeCard.UserId, out var userGuid);
            if (!ModelState.IsValid || !canParseCardId || !canParseUserId)
                return BadRequest(ModelState);

            try
            {
                await _cardService.Dislike(cardGuid, userGuid);

                return this.Ok();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        #region HELPER
        private static CardVM ConvertTo(Card card, string userProfileId, ICardService cardService)
        {
            return new CardVM
            {
                Id = card.Id.ToString(),
                LinkedCardId = card.LinkedCardId.ToString(),
                Content = card.Content,
                PublicationDate = card.PublicationDate,
                EndDate = card.EndDate,
                Type = card.Type,
                Answers = card.Type == CardTypes.Reporting.ToString() ? ((Reporting)card).Answers : 0,
                Views = card.Type == CardTypes.Reporting.ToString() ? ((Reporting)card).Views : 0,
                Likes = card.Type == CardTypes.Reporting.ToString() ? ((Reporting)card).Likes : 0,
                Author = (card.CreatedBy != null && card.CreatedBy.User != null) ? card.CreatedBy.User.FirstName + ' ' + card.CreatedBy.User.LastName : null,
                PictureId = card.PictureId.ToString(),
                TargetGroups = cardService.GetTargetGroupsString(card.Id).GetAwaiter().GetResult(),
                Results = cardService.GetResults(card.Id).GetAwaiter().GetResult(),
                IsLiked = cardService.CheckLikedCard(card.Id, Guid.Parse(userProfileId)).GetAwaiter().GetResult()
            };
        }
        #endregion
    }
}