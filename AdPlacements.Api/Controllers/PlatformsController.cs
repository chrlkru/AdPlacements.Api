using AdPlacements.Api.DTO;
using AdPlacements.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdPlacements.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly IAdPlatformStore _store;

    public PlatformsController(IAdPlatformStore store) => _store = store;

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UploadResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadResultDto>> Upload([FromForm] UploadFileRequest req, CancellationToken ct)
    {
        var file = req.File;
        if (file == null || file.Length == 0)
            return BadRequest("Файл не передан или пуст");

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        ms.Position = 0;

        var (loaded, skipped) = _store.Reload(ms);
        return Ok(new UploadResultDto(loaded, skipped));
    }



    [HttpGet]
    public ActionResult<IEnumerable<PlatformDto>> Get([FromQuery] string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return BadRequest("Укажите параметр location, например /ru/svrd/revda");

        var names = _store.FindByLocation(location).OrderBy(n => n, StringComparer.OrdinalIgnoreCase);
        return Ok(names.Select(n => new PlatformDto(n)));
    }
}
