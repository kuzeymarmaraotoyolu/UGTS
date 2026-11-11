using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UGTS.Models;

[Route("api/v1/objection-updates")]
[ApiController]
public class UgtsObjectionUpdateController : ControllerBase
{
    [HttpPost("{objectionId}")]
    [Authorize]
    public async Task<IActionResult> UpdateObjection(string objectionId, [FromForm] ObjectionUpdateFormModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.Request))
        {
            return BadRequest(new { message = "Metadata (request) alanı zorunludur." });
        }

        ObjectionUpdateModel? metadata;
        try
        {
            metadata = JsonSerializer.Deserialize<ObjectionUpdateModel>(model.Request, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            return BadRequest(new { message = "Geçersiz JSON formatı." });
        }

        if (metadata == null || string.IsNullOrEmpty(metadata.ObjectionStatus) || string.IsNullOrEmpty(metadata.Answer))
        {
            return BadRequest(new { message = "metadata bölümünde zorunlu alanlar eksik." });
        }

        if (metadata.ObjectionId != objectionId)
        {
            metadata.ObjectionId = objectionId;
        }

        var attachment = model.Attachment;
        if (attachment != null && attachment.Length > 0)
        {
            if (attachment.Length > 10 * 1024 * 1024)
            {
                return BadRequest(new { message = "Dosya boyutu aşıldı." });
            }

            var permittedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
            var extension = Path.GetExtension(attachment.FileName).ToLowerInvariant();

            if (!permittedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Desteklenmeyen dosya formatı." });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "SecureStorage");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await attachment.CopyToAsync(stream);
            }
        }

        return Ok(new
        {
            objectionStatus = metadata.ObjectionStatus,
            message = "Objection updated successfully."
        });
    }
}