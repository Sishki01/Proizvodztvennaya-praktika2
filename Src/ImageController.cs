using ImageApi.Models;
using ImageApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageApi.Controllers;

[ApiController]
[Route("api/image")]
[Produces("application/json")]
public class ImageController : ControllerBase
{
    private readonly ImageService _imageService;

    public ImageController(ImageService imageService)
    {
        _imageService = imageService;
    }

    /// <summary>
    /// Добавление изображения — сохраняет запись об изображении в базу данных.
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ImageRecord), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddImage([FromBody] AddImageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FilePath))
            return BadRequest(new { error = "FilePath is required." });

        try
        {
            var record = await _imageService.AddImageAsync(request.FilePath);
            return CreatedAtAction(nameof(GetAllImages), new { id = record.Id }, record);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Выдает информацию об изображении (Размер, Разрешение, Дата создания).
    /// Пользователь задаёт путь к изображению.
    /// </summary>
    [HttpGet("info")]
    [ProducesResponseType(typeof(ImageInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImageInfo([FromQuery] string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return BadRequest(new { error = "filePath query parameter is required." });

        try
        {
            var info = await _imageService.GetInfoAsync(filePath);
            return Ok(info);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Переименование названия изображения.
    /// </summary>
    [HttpPut("change/name")]
    [ProducesResponseType(typeof(ImageRecord), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RenameImage([FromBody] RenameRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FilePath))
            return BadRequest(new { error = "FilePath is required." });

        if (string.IsNullOrWhiteSpace(request.NewName))
            return BadRequest(new { error = "NewName is required." });

        // Reject names with invalid path characters
        if (request.NewName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            return BadRequest(new { error = "NewName contains invalid characters." });

        try
        {
            var record = await _imageService.RenameImageAsync(request.FilePath, request.NewName);
            return Ok(record);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Получить все изображения — выдаёт все записи об изображениях из базы данных.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ImageRecord>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllImages()
    {
        var images = await _imageService.GetAllAsync();
        return Ok(images);
    }
}
