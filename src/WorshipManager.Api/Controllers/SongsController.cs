using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;

namespace WorshipManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SongsController : ControllerBase
{
    private readonly ISongService _songService;
    private readonly ILogger<SongsController> _logger;

    public SongsController(ISongService songService, ILogger<SongsController> logger)
    {
        _songService = songService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SongDto>>> GetAll()
    {
        var songs = await _songService.GetAllSongsAsync();
        return Ok(songs);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<SongDto>>> GetActive()
    {
        var songs = await _songService.GetActiveSongsAsync();
        return Ok(songs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SongDto>> GetById(int id)
    {
        var song = await _songService.GetSongByIdAsync(id);
        if (song == null)
            return NotFound(new { message = $"Música com ID {id} não encontrada." });
        return Ok(song);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<SongDto>>> GetByCategory(string category)
    {
        var songs = await _songService.GetSongsByCategoryAsync(category);
        return Ok(songs);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<SongDto>>> Search([FromQuery] string term)
    {
        var songs = await _songService.SearchSongsAsync(term);
        return Ok(songs);
    }

    [HttpPost]
    public async Task<ActionResult<SongDto>> Create([FromBody] CreateSongDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var song = await _songService.CreateSongAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = song.Id }, song);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SongDto>> Update(int id, [FromBody] UpdateSongDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var song = await _songService.UpdateSongAsync(id, dto);
        return Ok(song);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _songService.DeleteSongAsync(id);
        return NoContent();
    }

    // Schedule Songs
    [HttpGet("schedule/{scheduleId}")]
    public async Task<ActionResult<IEnumerable<ScheduleSongDto>>> GetScheduleSongs(int scheduleId)
    {
        var songs = await _songService.GetScheduleSongsAsync(scheduleId);
        return Ok(songs);
    }

    [HttpPost("schedule/{scheduleId}")]
    public async Task<ActionResult<ScheduleSongDto>> AddToSchedule(int scheduleId, [FromBody] AddScheduleSongDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var scheduleSong = await _songService.AddSongToScheduleAsync(scheduleId, dto);
        return CreatedAtAction(nameof(GetScheduleSongs), new { scheduleId }, scheduleSong);
    }

    [HttpDelete("schedule/{scheduleId}/{songId}")]
    public async Task<IActionResult> RemoveFromSchedule(int scheduleId, int songId)
    {
        await _songService.RemoveSongFromScheduleAsync(scheduleId, songId);
        return NoContent();
    }

    [HttpPut("schedule/{scheduleId}/reorder")]
    public async Task<IActionResult> ReorderScheduleSongs(int scheduleId, [FromBody] List<int> songIdsInOrder)
    {
        await _songService.ReorderScheduleSongsAsync(scheduleId, songIdsInOrder);
        return Ok();
    }
}
