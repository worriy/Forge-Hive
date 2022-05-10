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
using Hive.Backend.Models.JoinTables;

namespace Hive.Backend.Api.PostsController
{
	/// <summary>
	/// Controller responsible for all data interactions with the PostsController
	/// </summary>
	[Route("api/posts")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class PostsController : Controller
	{
		private readonly ICardService _cardService;
        private readonly ILogger _logger;

        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.PostsControllerController"/> class.
        /// </summary>
        public PostsController(ICardService cardService, ILogger<PostsController> logger)
		{
			_cardService = cardService;
            _logger = logger;
		}

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostVM cardVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var card = await _cardService.CreatePost(ConvertToCard(cardVM));

                return this.Ok(ConvertToTopPostVM(card));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePostVM cardVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _cardService.UpdatePost(ConvertToCard(cardVM));
                return this.Ok();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("getEditableCard/{cardId}")]
        public async Task<IActionResult> GetEditableCard(string cardId)
        {
            var canParse = Guid.TryParse(cardId, out var cardGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var editablePostVM = await _cardService.GetEditablePost(cardGuid);

                if (editablePostVM == null)
                    return NoContent();

                return this.Ok(ConvertTo(editablePostVM));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("getLatestPosts/{userProfileId}")]
        public async Task<IActionResult> GetLatestPosts(string userProfileId)
		{
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!ModelState.IsValid || !canParse)
                return BadRequest();

			try
			{
                var posts = await _cardService.GetLatestPosts(userProfileGuid);

                if (posts == null)
                    return NoContent();

                var result = posts.Select(p => ConvertToPostVM(p))
                    .ToList();

				return this.Ok(result);
			}
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <PostVM> ().AsQueryable());
			}
		}

        [HttpGet("getPostDetails/{postId}")]
        public async Task<IActionResult> GetPostDetails(string postId)
		{
            var canParse = Guid.TryParse(postId, out var postGuid);
            if (!canParse)
                return BadRequest();
			
			try
			{
                var (card, report) = await _cardService.GetPostDetails(postGuid);
                var result = await ConvertTo(card, report);

                if (result == null)
                    return NoContent();

                return this.Ok(result);
            }
			catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

        [HttpGet("getTopPosts/{userProfileId}")]
        public async Task<IActionResult> GetTopPostsByUser(string userProfileId)
		{
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParse)
                return BadRequest();
			try
			{
                var posts = await _cardService.GetTopPostsByUser(userProfileGuid);

                if (posts == null)
                    return NoContent();

                var result = posts.Select(p => ConvertToTopPostVM(p))
                    .ToList();

                return Ok(result);
            }
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <TopPostVM> ().AsQueryable());
			}
		}

        [HttpGet("{postId}")]
        public async Task<IActionResult> Get(string postId)
		{
            var canParse = Guid.TryParse(postId, out var postGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var card = await this._cardService.GetById(postGuid);

                if (card == null)
                    return NoContent();
                
                return this.Ok(ConvertToCardVM(card));
                
            }
            catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

        [HttpDelete("{postId}")]
        public async Task<IActionResult> Delete(string postId)
        {
            var canParse = Guid.TryParse(postId, out var postGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                await this._cardService.Delete(postGuid);
                return Ok();
            }
            catch (Exception xcp)
            {
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("getDefaultPicture")]
        [ProducesResponseType(typeof(PictureVM), 200)]
        public async Task<IActionResult> GetDefaultPicture(CardTypes type)
        {
            if (string.IsNullOrEmpty(type.ToString()))
                return BadRequest();

            try
            {
                var picture = await _cardService.GetDefaultPictureAsync(type.ToString());
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

        #region HELPERS
        private Card ConvertToCard(CreatePostVM cardVM)
        {
            var choices = new List<Choice>();
            if (cardVM.Choices != null)
            {
                choices = cardVM.Choices
                    .Select(c => new Choice
                    {
                        Name = c
                    })
                    .ToList();
            }
            
            var cardGroups = cardVM.TargetGroupsIds
                .Select(g => new CardGroup
                {
                    GroupId = Guid.Parse(g)
                })
                .ToList();

            return new Card
            {
                Content = cardVM.Content,
                PublicationDate = cardVM.PublicationDate,
                EndDate = cardVM.EndDate,
                CreatedById = Guid.Parse(cardVM.AuthorId),
                PictureId = string.IsNullOrEmpty(cardVM.PictureId) ? Guid.Empty : Guid.Parse(cardVM.PictureId),
                Type = cardVM.Type.ToString(),
                Choices = choices,
                CardGroup = cardGroups
            };
        }
        private Card ConvertToCard(UpdatePostVM cardVM)
        {
            var cardGroups = cardVM.TargetGroupsIds
                .Select(g => new CardGroup
                {
                    GroupId = Guid.Parse(g),
                    CardId = Guid.Parse(cardVM.Id)
                })
                .ToList();

            return new Card
            {
                Id = Guid.Parse(cardVM.Id),
                Content = cardVM.Content,
                PublicationDate = cardVM.PublicationDate,
                EndDate = cardVM.EndDate,
                PictureId = string.IsNullOrEmpty(cardVM.PictureId) ? Guid.Empty : Guid.Parse(cardVM.PictureId),
                Choices = cardVM.Choices ?? null,
                CardGroup = cardGroups
            };
        }
        private TopPostVM ConvertToTopPostVM(Card card)
        {
            return new TopPostVM()
            {
                Id = card.Id.ToString(),
                Content = card.Content,
                PublicationDate = card.PublicationDate,
                EndDate = card.EndDate,
                Picture = card.Picture.PicBase64,
                Status = _cardService.GetStatus(card),
                Type = card.Type,
                Views = 0,
                Answers = 0
            };
        }
        private TopPostVM ConvertToTopPostVM(Reporting reporting)
        {
            return new TopPostVM
            {
                Id = reporting.LinkedCardId.ToString(),
                Content = reporting.Content,
                PublicationDate = reporting.PublicationDate,
                EndDate = reporting.EndDate.AddDays(-1),
                Picture = reporting.Picture.PicBase64,
                Views = reporting.Views,
                Answers = reporting.Answers,
                Type = _cardService.GetTypeCard(reporting.LinkedCardId).GetAwaiter().GetResult()
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
                Status = _cardService.GetStatus(card),
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
        private async Task<PostDetailsVM> ConvertTo(Card card, Reporting report)
        {
            return new PostDetailsVM
            {
                Id = card.Id.ToString(),
                Content = report.Content,
                PublicationDate = report.PublicationDate,
                EndDate = card.EndDate,
                Views = report.Views,
                Likes = report.Likes,
                Answers = report.Answers,
                TargetGroups = await _cardService.GetTargetGroupsString(card.Id),
                Results = report.Results,
                Type = DateTime.Now > card.EndDate ? report.Type : card.Type,
                Status = _cardService.GetStatus(card),
                Picture = card.Picture.PicBase64
            };
        }
        private EditableCardVM ConvertTo(Card card)
        {
            return new EditableCardVM
            {
                Id = card.Id.ToString(),
                Content = card.Content,
                PublicationDate = card.PublicationDate,
                EndDate = card.EndDate,
                Picture = card.Picture.PicBase64,
                Choices = card.Type == CardTypes.Question.ToString() ? card.Choices : null,
                TargetGroupsIds = card.CardGroup.Select(g => g.GroupId.ToString()).ToList()
            };
        }
        #endregion

    }
}