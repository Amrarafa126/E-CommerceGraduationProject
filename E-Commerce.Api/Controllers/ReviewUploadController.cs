using E_Commerce.Core.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/reviews")]
    [Authorize]
    [Produces("application/json")]
    public class ReviewUploadController(IWebHostEnvironment env) : ControllerBase
    {
        private static readonly (long MaxSize, string[] MimeTypes) ImageRule = (
            10 * 1024 * 1024,
            new[] { "image/jpeg", "image/png", "image/webp", "image/gif" }
        );

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<ReviewUploadResult>), 200)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("No file provided.", 400));

            if (file.Length > ImageRule.MaxSize)
                return BadRequest(ApiResponse<object>.Fail($"Image exceeds max size of {ImageRule.MaxSize / 1024 / 1024}MB.", 400));

            if (!ImageRule.MimeTypes.Contains(file.ContentType.ToLower()))
                return BadRequest(ApiResponse<object>.Fail($"Invalid image type: {file.ContentType}.", 400));

            var uploadsDir = Path.Combine(env.WebRootPath, "uploads", "reviews");
            Directory.CreateDirectory(uploadsDir);

            var safeFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, safeFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, ct);
            }

            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/reviews/{safeFileName}";

            return Ok(ApiResponse<ReviewUploadResult>.Ok(new ReviewUploadResult(
                safeFileName, fileUrl, file.Length, file.ContentType)));
        }
    }

    public record ReviewUploadResult(string FileName, string FileUrl, long FileSize, string MimeType);
}
