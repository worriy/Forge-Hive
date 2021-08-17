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

namespace Hive.Backend.Api.PostsController
{
	/// <summary>
	/// Controller responsible for all data interactions with the PostsController
	/// </summary>
	[Route("api/v1/HivePostsController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class PostsController : Controller
	{
		private readonly ICardService _CardService;
        private readonly ILogger _logger;

        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.PostsControllerController"/> class.
        /// </summary>
        public PostsController(ICardService CardService, ILogger<PostsController> logger)
		{
			_CardService = CardService;
            _logger = logger;
		}

		[HttpGet]
		[Route("/api/posts/getLatestPosts")]
		public async Task<IActionResult> GetLatestPosts(PagingVM paging,string userProfileId)
		{
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!ModelState.IsValid || !canParse)
                return BadRequest();

			try
			{
                var result = await _CardService.GetLatestPosts(paging, userProfileGuid);
                var list = new List<PostVM>();

                foreach(Card card in result)
                {
                    list.Add(ConvertToPostVM(card));
                }

				return this.Ok(await Task.FromResult(list));
			}
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <PostVM> ().AsQueryable());
			}
		}

		[HttpGet]
		[Route("/api/posts/getPostDetails")]
		public async Task<IActionResult> GetPostDetails(string postId)
		{
            var canParse = Guid.TryParse(postId, out var postGuid);
            if (!canParse)
                return BadRequest();
			
			try
			{
                PostDetailsVM result = await _CardService.GetPostDetails(postGuid);

                if (result == null)
                    return NoContent();

                //result = await _ReportingService.fillInfos(result);
                return this.Ok(await Task.FromResult(result));
            }
			catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

		[HttpGet]
		[Route("/api/posts/getTopPosts")]
		public async Task<IActionResult> GetTopPostsByUser(string userProfileId)
		{
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParse)
                return BadRequest();
			try
			{
                var result = await _CardService.GetTopPostsByUser(userProfileGuid);

                if (result == null)
                    return NoContent();

                var list = new HashSet<TopPostVM>();
                foreach (var item in result)
                {
                    list.Add(ConvertToTopPostVM(item));
                }

                return Ok(list);
            }
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <TopPostVM> ().AsQueryable());
			}
		}

		[HttpGet]
		[Route("/api/posts/get")]
		public async Task<IActionResult> Get(string postId)
		{
            var canParse = Guid.TryParse(postId, out var postGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var card = await this._CardService.GetById(postGuid);

                if (card == null)
                    return NoContent();
                
                //var post = new CardVM().ConvertFromModel(card);
                return this.Ok(ConvertToCardVM(card));
                
            }
            catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

        [HttpDelete]
        [Route("/api/posts/delete")]
        public async Task<IActionResult> Delete(string postId)
        {
            var canParse = Guid.TryParse(postId, out var postGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                await this._CardService.Delete(postGuid);
                return Ok(true);
            }
            catch (Exception xcp)
            {
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        #region HELPERS
        private TopPostVM ConvertToTopPostVM(Reporting reporting)
        {
            return new TopPostVM
            {
                Id = reporting.Id.ToString(),
                Content = reporting.Content,
                PublicationDate = reporting.PublicationDate,
                EndDate = reporting.EndDate.AddDays(-1),
                Picture = reporting.Picture.PicBase64,
                Views = reporting.Views,
                Answers = reporting.Answers
            };

        }
        private PostVM ConvertToPostVM(Card card)
        {
            return new PostVM()
            {
                Id = card.Id.ToString(),
                Content = card.Content,
                PublicationDate = card.PublicationDate,
                EndDate = card.EndDate,
                Status = _CardService.GetStatus(card),
                Type = card.Type
            };
        }

        private CardVM ConvertToCardVM(Card card)
        {
            return new CardVM()
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
                Picture = card.Picture.PicBase64
            };
        }
        #endregion

    }
}