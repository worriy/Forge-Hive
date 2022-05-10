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

namespace Hive.Backend.Api.HighlightsController
{
	/// <summary>
	/// Controller responsible for all data interactions with the HighlightsController
	/// </summary>
	[Route("api/highlights")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class HighlightsController : Controller
	{
        private readonly ICardService _CardService;
        private readonly IUserProfileService _UserProfileService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.HighlightsControllerController"/> class.
        /// </summary>
        public HighlightsController(ICardService CardService, IUserProfileService UserProfileService, ILogger<HighlightsController> logger)
		{
			_CardService = CardService;
			_UserProfileService = UserProfileService;
            _logger = logger;
		}

		[HttpGet("getTopPosts")]
		public async Task<IActionResult> GetTopPosts()
		{
			try
			{
                List<CardVM> topPosts = new List<CardVM>();
                var result = await _CardService.GetTopPosts();

                if (result == null)
                    return NoContent();
				
                foreach (var report in result)
                {
                    report.CreatedBy = await _UserProfileService
                        .GetById(report.CreatedById);
                    report.Type = _CardService.GetTypeCardFromReportId(report.Id);
                    topPosts.Add(await ConvertToCardVM(report));
                }
                return this.Ok(topPosts);

			}
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <TopPostVM> ().AsQueryable());
			}
		}

		[HttpGet("getBestContributor")]
		[ProducesResponseType(typeof(BestContributorVM), 200)]
		public async Task<IActionResult> GetBestContributor()
		{
			try
			{
                var user = await _UserProfileService.GetBestContributor();

                if (user == null)
                    return NoContent();

                return this.Ok(ConvertToBestContributor(user));
            }
			catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

		[HttpGet("getBestPost")]
		[ProducesResponseType(typeof(ReportVM), 200)]
		public async Task<IActionResult> GetBestPost()
		{
			try
			{
                var reporting = await _CardService.GetBestPost();

                if (reporting == null)
                    return NoContent();

                reporting.CreatedBy = await _UserProfileService
                    .GetById(reporting.CreatedById);

                return this.Ok(await ConvertToReportVM(reporting));
            }
			catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

        [HttpGet("getGeneralMood")]
        [ProducesResponseType(typeof(GeneralMoodVM), 200)]
        public async Task<IActionResult> GetGeneralMood()
        {
            try
            {
                var (moodName, value) = _CardService.GetGeneralMood();
                var result = ConvertToGeneralMood(moodName, value);

                return this.Ok(await Task.FromResult(result));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        #region HELPER
        private BestContributorVM ConvertToBestContributor(UserProfile userProfile)
        {
            return new BestContributorVM
            {
                Department = userProfile.Department,
                City = userProfile.City,
                Country = userProfile.Country,
                Firstname = userProfile.User.FirstName,
                Lastname = userProfile.User.LastName,
                PictureUrl = userProfile.User.PictureUrl,
                Posts = _UserProfileService.GetBestContributorPostsNumber(userProfile.Id),
                Answers = _UserProfileService.GetAnswersNumber(userProfile.Id),
                Likes = _UserProfileService.GetLikesNumber(userProfile.Id)
            };
        }

        private async Task<ReportVM> ConvertToReportVM(Reporting report)
        {
            return new ReportVM
            {
                Id = report.Id.ToString(),
                Content = report.Content,
                Author = report.CreatedBy != null && report.CreatedBy.User != null ? report.CreatedBy.User.FirstName + ' ' + report.CreatedBy.User.LastName : null,
                Views = report.Views,
                Likes = report.Likes,
                Answers = report.Answers,
                Results = await _CardService.GetResults(report.Id)
            };
        }

        private async Task<CardVM> ConvertToCardVM(Card card)
        {
            return new CardVM
            {
                Id = card.Id.ToString(),
                LinkedCardId = card.LinkedCardId.ToString(),
                Content = card.Content,
                PublicationDate = card.PublicationDate,
                EndDate = card.EndDate,
                Type = card.Type,
                Answers = ((Reporting)card).Answers,
                Views = ((Reporting)card).Views,
                Likes = ((Reporting)card).Likes,
                Author = (card.CreatedBy != null && card.CreatedBy.User != null) ? card.CreatedBy.User.FirstName + ' ' + card.CreatedBy.User.LastName : null,
                Picture = card.Picture.PicBase64,

                Results = await _CardService.GetResults(card.Id),
                TargetGroups = await _CardService.GetTargetGroupsString(card.Id)
            };
        }

        private GeneralMoodVM ConvertToGeneralMood(string moodName, int value)
        {
            return new GeneralMoodVM
            {
                MoodName = moodName,
                Value = value
            };
        }
        #endregion

    }
}