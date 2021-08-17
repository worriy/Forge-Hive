using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Hive.Backend.Api.QuoteController
{
    [Route("api/v1/HiveQuoteController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class QuoteController : Controller
    {
        private readonly ICardService _CardService;
        private readonly IQuoteService _QuoteService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="MeteoMeter.Backend.Api.QuestionController"/> class.
        /// </summary>
        public QuoteController(IQuoteService QuoteService, ICardService CardService, ILogger<QuoteController> logger)
        {
            _CardService = CardService;
            _QuoteService = QuoteService;
            _logger = logger;
        }


        [HttpPost]
        [Route("/api/quote/create")]
        public async Task<IActionResult> Create([FromBody] CreateQuoteVM quote)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newQuote = await _QuoteService.InsertQuote(ConvertTo(quote));

                return this.Ok(ConvertToTopPostVM(newQuote));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpPut]
        [Route("/api/quote/update")]
        public async Task<IActionResult> Update([FromBody] EditQuoteVM quoteVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _QuoteService.Save(ConvertTo(quoteVM));

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
        [Route("/api/quote/getEditableQuote")]
        public async Task<IActionResult> GetEditableQuote(string quoteId)
        {
            var canParse = Guid.TryParse(quoteId, out var quoteGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var editableQuoteVM = await _QuoteService.GetEditableQuote(quoteGuid);
                if (editableQuoteVM == null)
                    return NoContent();
                
                return this.Ok(await Task.FromResult(editableQuoteVM));
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        #region HELPER
        private Quote ConvertTo(CreateQuoteVM quote)
        {
            var model = new Quote
            {
                Content = quote.Content,
                PublicationDate = quote.PublicationDate,
                EndDate = quote.EndDate,
                CreatedById = Guid.Parse(quote.AuthorId),
                Type = CardTypes.Quote.ToString()
            };

            model.CardGroup = new List<CardGroup>();
            foreach (var groupId in quote.TargetGroupsIds)
            {
                model.CardGroup.Add(new CardGroup(Guid.Empty, Guid.Parse(groupId)));
            }
            return model;
        }

        private Quote ConvertTo(EditQuoteVM quote)
        {
            return new Quote
            {
                Id = Guid.Parse(quote.Id),
                Content = quote.Content,
                PublicationDate = quote.PublicationDate,
                EndDate = quote.EndDate
            };
        }
        private TopPostVM ConvertToTopPostVM(Quote quote)
        {
            return new TopPostVM()
            {
                Id = quote.Id.ToString(),
                Content = quote.Content,
                PublicationDate = quote.PublicationDate,
                EndDate = quote.EndDate,
                Picture = quote.Picture.PicBase64,
                Status = _CardService.GetStatus(quote),
                Type = quote.Type,
                Views = 0,
                Answers = 0
            };
        }
        #endregion
    }
}

