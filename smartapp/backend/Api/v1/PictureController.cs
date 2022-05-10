using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hive.Backend.DataModels;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Hive.Backend.Api.PictureController
{
	/// <summary>
	/// Controller responsible for all data interactions with the PictureController
	/// </summary>
	[Route("api/picture")]
	[Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
	public class PictureController : Controller
	{
		private readonly IPictureService _PictureService;
		private readonly ILogger _logger;
		/// <summary>
		/// Initialize a new instance of <see cref="Hive.Backend.Api.PictureControllerController"/> class.
		/// </summary>
		public PictureController(IPictureService PictureService, ILogger<PictureController> logger)
		{
			_PictureService = PictureService;
			_logger = logger;
		}

		[HttpGet("{pictureId}")]
		[ProducesResponseType(typeof(PictureVM), 200)]
		public async Task<IActionResult> Get(string pictureId)
		{
			var canParse = Guid.TryParse(pictureId, out var pictureGuid);
			if (!canParse)
                return BadRequest();
			
			try
			{
                var picture = await _PictureService.GetById(pictureGuid);
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

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PictureVM picture)
        {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
                Picture photo = new Picture
                {
                    PicBase64 = picture.Picture,
                };
                await _PictureService.Save(photo);
                var result = await _PictureService.GetById(photo.Id);

                return Ok(result.Id);
			}
			catch (Exception xcp) {
				//log exception
				_logger.LogError(xcp.ToString());
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
        }
    
	}
}