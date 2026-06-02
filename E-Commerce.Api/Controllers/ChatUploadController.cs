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
        private static readonly Dictionary<string, (long MaxSize, string[] MimeTypes, string[] Prefixes)> UploadRules = new()
        {
            ["image"] = (10 * 1024 * 1024,
                new[] { "image/jpeg", "image/png", "image/webp", "image/gif", "image/heic", "image/heif", "image/avif" },
                new[] { "image/" }),
            ["video"] = (50 * 1024 * 1024,
                new[] { "video/mp4", "video/webm", "video/quicktime", "video/x-msvideo", "video/mpeg" },
                new[] { "video/" }),
            ["voice"] = (20 * 1024 * 1024,
                new[] { "audio/mpeg", "audio/mp4", "audio/webm", "audio/wav", "audio/wave", "audio/ogg", "audio/aac", "audio/x-m4a", "audio/opus", "audio/flac", "audio/x-wav" },
                new[] { "audio/" }),
            ["file"] = (25 * 1024 * 1024,
                new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "text/plain" },
                Array.Empty<string>())
        };

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<ChatUploadResult>), 200)]
        public async Task<IActionResult> Upload(
            IFormFile file,
            [FromForm] string type,
            CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("لم يتم اختيار ملف.", 400));

            if (string.IsNullOrWhiteSpace(type))
                return BadRequest(ApiResponse<object>.Fail("نوع الملف مطلوب.", 400));

            if (!UploadRules.TryGetValue(type.ToLower(), out var rule))
                return BadRequest(ApiResponse<object>.Fail("نوع الملف غير صالح. المسموح: image، video، voice، file.", 400));

            if (file.Length > rule.MaxSize)
                return BadRequest(ApiResponse<object>.Fail($"حجم الملف يتجاوز الحد الأقصى المسموح به {rule.MaxSize / 1024 / 1024} ميجابايت.", 400));

            var contentType = file.ContentType?.ToLower() ?? "";

            // Check exact MIME or prefix match
            bool mimeOk = rule.MimeTypes.Any(mt => contentType.StartsWith(mt)) ||
                          rule.Prefixes.Any(p => contentType.StartsWith(p));

            if (!mimeOk)
                return BadRequest(ApiResponse<object>.Fail($"نوع الملف غير صالح: {file.ContentType}. المسموح: {string.Join(", ", rule.MimeTypes)}.", 400));

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
                safeFileName, fileUrl, type.ToLower(), file.Length, file.ContentType ?? "application/octet-stream")));
        }
    }

    public record ChatUploadResult(string FileName, string FileUrl, string Type, long FileSize, string MimeType);
}
