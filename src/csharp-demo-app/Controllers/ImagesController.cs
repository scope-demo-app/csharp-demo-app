using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScopeAgent;

namespace csharp_demo_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
      
        public ImagesController(ILogger<ImagesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("restaurant/{restaurantId}")]
        public IAsyncEnumerable<Guid> GetRestaurantImages(Guid restaurantId)
        {
            _logger.LogInformation($"Getting all images for RestaurantId: {restaurantId}");
            var ctx = new ImagesContext();
            Response.RegisterForDisposeAsync(ctx);
            return ctx.ImagesData
                .Where(i => i.RestaurantId == restaurantId)
                .Select(i => i.Id)
                .AsAsyncEnumerable();
        }

        [HttpPost("restaurant/{restaurantId}")]
        public async Task<IActionResult> PostImage(Guid restaurantId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[RestaurantId: {restaurantId}] Storing a new image", restaurantId);

            string contentType;
            Stream fileStream;

            if (Request.ContentType.StartsWith("image/"))
            {
                contentType = Request.ContentType;
                fileStream = Request.Body;
            }
            else
            {
                _logger.LogCritical("No images were found in the request. Invalid ContentType = " + Request.ContentType);
                return BadRequest("No images were found in the request. Invalid ContentType = " + Request.ContentType);
            }

            _logger.LogInformation("[RestaurantId: {restaurantId}] ContentType: {contentType}", restaurantId, contentType);

            await using var imagesCtx = new ImagesContext();
            
            var bytesData = await Utils.GetBytesFromStreamAsync(fileStream, cancellationToken).ConfigureAwait(false);
            if (bytesData.Length == 0)
                throw new Exception("Image data length must be greater than 0");
            
            var imgData = new ImagesEntity
            {
                RestaurantId = restaurantId,
                ContentType = contentType,
                ContentData = bytesData
            };
            imagesCtx.ImagesData.Add(imgData);

            await imagesCtx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("[RestaurantId: {restaurantId}] Images saved with Id: {Id}", restaurantId, imgData.Id);
            return Ok(imgData.Id);
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetImage(Guid imageId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting image: {imageId}");
            await using var imagesCtx = ImagesContext.GetBalancedContext();
            
            var image = await imagesCtx.ImagesData
                .FirstOrDefaultAsync(i => i.Id == imageId, cancellationToken).ConfigureAwait(false);
            if (image == null)
            {
                _logger.LogError($"Image: {imageId}, can't be found.");
                return NotFound();
            }

            return File(image.ContentData, image.ContentType);
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid imageId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deleting image: {imageId}");
            await using var imagesCtx = new ImagesContext();
            
            var image = await imagesCtx.ImagesData
                .FirstOrDefaultAsync(i => i.Id == imageId, cancellationToken).ConfigureAwait(false);
            if (image == null)
            {
                _logger.LogError($"Image: {imageId}, can't be found.");
                return NotFound();
            }

            imagesCtx.ImagesData.Remove(image);
            if (await imagesCtx.SaveChangesAsync(cancellationToken) == 0)
            {
                _logger.LogError($"Image: {imageId}, cannot be deleted.");
                return Problem("The image can't be deleted.");
            }

            return Ok();
        }
    }
}