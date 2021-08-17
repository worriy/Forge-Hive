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
    [Route("api/v1/HiveAnswerController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class AnswerController : Controller
    {
        private readonly ILogger _logger;
        private readonly IAnswerService _AnswerService;
        private readonly IReportingService _ReportingService;

        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.AnswerControllerController"/> class.
        /// </summary>
        public AnswerController(
            ILogger<AnswerController> logger,
            IAnswerService AnswerService, 
            IReportingService ReportingService)
        {
            _AnswerService = AnswerService;
            _ReportingService = ReportingService;
            _logger = logger;
        }

        [HttpPost]
        [Route("/api/answer/create")]
        public async Task<IActionResult> Create([FromBody] AnswerVM answer)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _AnswerService.Save(ConvertTo(answer));

                if(answer.IdCard != Program.MoodId.ToString() && answer.IdChoice != Program.StartSurveyChoiceId.ToString())
                    await _ReportingService.AddAnswer(ConvertTo(answer));

                return this.Ok(true);
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("/api/answer/answerMood")]
        public async Task<IActionResult> AnswerMood([FromBody] AnswerVM answer)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _AnswerService.Save(ConvertTo(answer));

                return this.Ok(true);
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("/api/answer/discard")]
        public async Task<IActionResult> Discard([FromBody] DiscardVM discard)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _AnswerService.Save(ConvertTo(discard));

                return this.Ok();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/api/answer/answeredCard")]
        public async Task<IActionResult> AnsweredCard(string cardId, string userProfileId)
        {
            var canParseCardId = Guid.TryParse(cardId, out var cardGuid);
            var canParseUserProfileId = Guid.TryParse(userProfileId, out var userProfileGuid);
            if(!canParseCardId || !canParseUserProfileId)
                return BadRequest();

            try
            {
                bool result = _AnswerService.AnsweredCard(cardGuid, userProfileGuid);

                return Ok(await Task.FromResult(result));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/api/answer/getChoiceMoodId")]
        public async Task<IActionResult> GetChoiceMoodId(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            try
            {
                var result = _AnswerService.GetChoiceMoodId(name);

                return Ok(await Task.FromResult(result));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Route("/api/answer/delete")]
        public async Task<IActionResult> Delete(string cardId, string userProfileId)
        {
            var canParseCardId = Guid.TryParse(cardId, out var cardGuid);
            var canParseUserProfileId = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParseCardId || !canParseUserProfileId)
                return BadRequest();

            try
            {
                await this._AnswerService.DeleteAnswer(cardGuid, userProfileGuid);
                return Ok(true);
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
                ChoiceId = Guid.Parse(answer.IdChoice),
                AnswerDate = DateTime.Now
            };
        }

        private Answer ConvertTo(DiscardVM discard)
        {
            return new Answer
            {
                CardId = Guid.Parse(discard.CardId),
                AnsweredById = Guid.Parse(discard.UserProfileId),
                ChoiceId = Program.DiscardChoiceId,
                AnswerDate = new DateTime()
            };
        }
        #endregion
    }
}