using E_Commerce.Core.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/chat")]
    [Authorize]
    [Produces("application/json")]
    public class ChatUploadController(IWebHostEnvironment env) : ControllerBase
    {
        private static readonly Dictionary<string, (long MaxSize, string[] MimeTypes)> UploadRules = new()
        {
            ["image"] = (10 * 1024 * 1024, new[] { "image/jpeg", "image/png", "image/webp", "image/gif" }),
            ["video"] = (50 * 1024 * 1024, new[] { "video/mp4", "video/webm", "video/quicktime" }),
            ["voice"] = (20 * 1024 * 1024, new[] { "audio/mpeg", "audio/mp4", "audio/webm", "audio/wav", "audio/ogg" }),
            ["file"] = (25 * 1024 * 1024, new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "text/plain" })
        };

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<ChatUploadResult>), 200)]
        public async Task<IActionResult> Upload(
            IFormFile file,
            [FromForm] string type, // image, video, voice, file
            CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("No file provided.", 400));

            if (!UploadRules.TryGetValue(type.ToLower(), out var rule))
                return BadRequest(ApiResponse<object>.Fail("Invalid upload type.", 400));

            if (file.Length > rule.MaxSize)
                return BadRequest(ApiResponse<object>.Fail($"File exceeds max size of {rule.MaxSize / 1024 / 1024}MB.", 400));

            if (!rule.MimeTypes.Contains(file.ContentType.ToLower()))
                return BadRequest(ApiResponse<object>.Fail($"Invalid file type: {file.ContentType}.", 400));

            var uploadsDir = Path.Combine(env.WebRootPath, "uploads", "chat");
            Directory.CreateDirectory(uploadsDir);

            var safeFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, safeFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, ct);
            }

            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/chat/{safeFileName}";

            return Ok(ApiResponse<ChatUploadResult>.Ok(new ChatUploadResult(
                safeFileName, fileUrl, type.ToLower(), file.Length, file.ContentType)));
        }
    }

    public record ChatUploadResult(string FileName, string FileUrl, string Type, long FileSize, string MimeType);
}
