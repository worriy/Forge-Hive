using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Hive.Backend.Api.NotificationsController
{
    /// <summary>
	/// Controller responsible for all data interactions with the NotificationsController
	/// </summary>
	[Route("api/v1/HiveNotificationsController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public partial class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ICardService _cardService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="MeteoMeter.Backend.Api.NotificationsControllerController"/> class.
        /// </summary>
        public NotificationsController(ICardService cardService, ILogger<NotificationsController> logger)
        {
            _cardService = cardService;
            _notificationService = new NotificationService();
            _logger = logger;
        }

        [HttpPost]
        [Route("/api/notifications/registerTags")]
        public async Task<IActionResult> RegisterTags([FromBody] TagRegisterVM tagRegister)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _notificationService.RegisterTags(tagRegister.InstallationId, tagRegister.RegistrationId, tagRegister.Tags, tagRegister.Platform);

                return Ok();
            }
            catch (Exception xcp)
            {
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Route("/api/notifications/unsubscribe")]
        public async Task<IActionResult> Unsubscribe(string installationId)
        {
            if (string.IsNullOrEmpty(installationId))
                return BadRequest();

            try
            {
                await _notificationService.Unsubscribe(installationId);
                return Ok();
            }
            catch (Exception xcp)
            {
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/api/notifications/registerForResults")]
        public async Task<IActionResult> NotifyForResult(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();
            try
            {
                await _notificationService.NotifyForResults(userId, this._cardService);
                return Ok();
            }
            catch (Exception xcp)
            {
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
