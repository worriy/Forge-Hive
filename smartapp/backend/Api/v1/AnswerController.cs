using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Hive.Backend.DataModels;

namespace Hive.Backend.Api.AnswerController
{
    /// <summary>
    /// Controller responsible for all data interactions with the AnswerController
    /// </summary>
    [Route("api/answer")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class AnswerController : Controller
    {
        private readonly ILogger _logger;
        private readonly IAnswerService _answerService;

        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.AnswerControllerController"/> class.
        /// </summary>
        public AnswerController(
            ILogger<AnswerController> logger,
            IAnswerService answerService)
        {
            _answerService = answerService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AnswerVM answer)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _answerService.Save(ConvertTo(answer));

                if(answer.IdCard != Program.MoodId.ToString() && answer.IdChoice != Program.DiscardChoiceId.ToString())
                    await _answerService.AddAnswer(ConvertTo(answer));

                return this.Ok();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("answeredCard/{cardId}/{userProfileId}")]
        public async Task<IActionResult> AnsweredCard(string cardId, string userProfileId)
        {
            var canParseCardId = Guid.TryParse(cardId, out var cardGuid);
            var canParseUserProfileId = Guid.TryParse(userProfileId, out var userProfileGuid);
            if(!canParseCardId || !canParseUserProfileId)
                return BadRequest();

            try
            {
                var result = _answerService.AnsweredCard(cardGuid, userProfileGuid);

                return Ok(await Task.FromResult(result));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("getChoiceMoodId/{name}")]
        public async Task<IActionResult> GetChoiceMoodId(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            try
            {
                var result = _answerService.GetChoiceMoodId(name);

                return Ok(await Task.FromResult(result));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{cardId}/{userProfileId}")]
        public async Task<IActionResult> Delete(string cardId, string userProfileId)
        {
            var canParseCardId = Guid.TryParse(cardId, out var cardGuid);
            var canParseUserProfileId = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParseCardId || !canParseUserProfileId)
                return BadRequest();

            try
            {
                await this._answerService.DeleteAnswer(cardGuid, userProfileGuid);
                return Ok();
            }
            catch (Exception xcp)
            {
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        #region HELPER
        private Answer ConvertTo(AnswerVM answer)
        {
            return new Answer
            {
                CardId = Guid.Parse(answer.IdCard),
                AnsweredById = Guid.Parse(answer.IdUser),
                ChoiceId = answer.IdChoice != null ? Guid.Parse(answer.IdChoice) : Program.DiscardChoiceId,
                AnswerDate = DateTime.Now
            };
        }

        #endregion
    }
}