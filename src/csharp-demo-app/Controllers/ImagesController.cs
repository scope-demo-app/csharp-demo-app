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
            _logger.LogInformation($"Getting images for RestaurantId: {restaurantId}");
            return new ImagesContext().ImagesData
                .Where(i => i.RestaurantId == restaurantId)
                .Select(i => i.Id)
                .AsAsyncEnumerable();
        }

        [HttpPost("restaurant/{restaurantId}")]
        public async Task<Guid> PostImage(Guid restaurantId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[RestaurantId: {restaurantId}] Storing a new image", restaurantId);

            string contentType;
            Stream fileStream;

            if (Request.HasFormContentType)
            {
                if (Request.Form?.Files == null || Request.Form.Files.Count == 0)
                    throw new ArgumentException("The request form files is null or empty");

                var file = Request.Form.Files[0];
                contentType = file.ContentType;
                fileStream = file.OpenReadStream();
            }
            else if (Request.ContentType.StartsWith("image/"))
            {
                contentType = Request.ContentType;
                fileStream = Request.Body;
            }
            else
            {
                _logger.LogCritical("No images were found in the request");
                throw new Exception("No images were found in the request");
            }

            _logger.LogInformation("[RestaurantId: {restaurantId}] ContentType: {contentType}", restaurantId, contentType);

            await using var imagesCtx = new ImagesContext();
            await using var memStream = new MemoryStream();

            await fileStream.CopyToAsync(memStream, cancellationToken);

            var imgData = new ImagesEntity
            {
                RestaurantId = restaurantId,
                ContentType = contentType,
                ContentData = memStream.ToArray()
            };
            imagesCtx.ImagesData.Add(imgData);

            await imagesCtx.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("[RestaurantId: {restaurantId}] Images saved with Id: {Id}", restaurantId, imgData.Id);
            return imgData.Id;
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetImage(Guid imageId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting image: {imageId}");
            await using var imagesCtx = new ImagesContext();

            var image = await imagesCtx.ImagesData
                .FirstOrDefaultAsync(i => i.Id == imageId, cancellationToken);
            if (image == null)
                return NotFound();

            return File(image.ContentData, image.ContentType);
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid imageId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deleting image: {imageId}");
            await using var imagesCtx = new ImagesContext();
            
            var image = await imagesCtx.ImagesData
                .FirstOrDefaultAsync(i => i.Id == imageId, cancellationToken);
            if (image == null)
                return NotFound();
            
            imagesCtx.ImagesData.Remove(image);
            if (await imagesCtx.SaveChangesAsync(cancellationToken) == 0)
                return Problem("The image can't be deleted.");
            
            return Ok();
        }
    }
}