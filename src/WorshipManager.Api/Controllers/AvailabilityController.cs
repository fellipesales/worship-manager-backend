using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;

namespace WorshipManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<AvailabilityController> _logger;

    public AvailabilityController(IAvailabilityService availabilityService, ILogger<AvailabilityController> logger)
    {
        _availabilityService = availabilityService;
        _logger = logger;
    }

    [HttpGet("member/{memberId}")]
    public async Task<ActionResult<IEnumerable<AvailabilityDto>>> GetByMember(int memberId)
    {
        var availabilities = await _availabilityService.GetMemberAvailabilitiesAsync(memberId);
        return Ok(availabilities);
    }

    [HttpGet("date/{date}")]
    public async Task<ActionResult<AvailabilityCalendarDto>> GetByDate(DateTime date)
    {
        var availability = await _availabilityService.GetAvailabilityForDateAsync(date);
        return Ok(availability);
    }

    [HttpGet("range")]
    public async Task<ActionResult<IEnumerable<AvailabilityCalendarDto>>> GetByRange(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var availabilities = await _availabilityService.GetAvailabilityForRangeAsync(startDate, endDate);
        return Ok(availabilities);
    }

    [HttpPost]
    public async Task<ActionResult<AvailabilityDto>> SetAvailability([FromBody] CreateAvailabilityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var availability = await _availabilityService.SetAvailabilityAsync(dto);
        return CreatedAtAction(nameof(GetByMember), new { memberId = dto.MemberId }, availability);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<AvailabilityDto>>> SetBulkAvailability([FromBody] BulkAvailabilityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var availabilities = await _availabilityService.SetBulkAvailabilityAsync(dto);
        return CreatedAtAction(nameof(GetByMember), new { memberId = dto.MemberId }, availabilities);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _availabilityService.DeleteAvailabilityAsync(id);
        return NoContent();
    }
}
