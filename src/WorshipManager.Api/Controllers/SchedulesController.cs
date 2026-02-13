using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;

namespace WorshipManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly IScheduleGeneratorService _generatorService;
    private readonly IWhatsAppService _whatsAppService;
    private readonly ILogger<SchedulesController> _logger;

    public SchedulesController(
        IScheduleService scheduleService,
        IScheduleGeneratorService generatorService,
        IWhatsAppService whatsAppService,
        ILogger<SchedulesController> logger)
    {
        _scheduleService = scheduleService;
        _generatorService = generatorService;
        _whatsAppService = whatsAppService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAll()
    {
        var schedules = await _scheduleService.GetAllSchedulesAsync();
        return Ok(schedules);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScheduleDto>> GetById(int id)
    {
        var schedule = await _scheduleService.GetScheduleByIdAsync(id);
        if (schedule == null)
            return NotFound(new { message = $"Escala com ID {id} não encontrada." });
        return Ok(schedule);
    }

    [HttpGet("calendar")]
    public async Task<ActionResult<IEnumerable<ScheduleCalendarDto>>> GetCalendar(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var schedules = await _scheduleService.GetScheduleCalendarAsync(startDate, endDate);
        return Ok(schedules);
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetUpcoming([FromQuery] int count = 5)
    {
        var schedules = await _scheduleService.GetUpcomingSchedulesAsync(count);
        return Ok(schedules);
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleDto>> Create([FromBody] CreateScheduleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var schedule = await _scheduleService.CreateScheduleAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = schedule.Id }, schedule);
    }

    [HttpPost("generate")]
    public async Task<ActionResult<ScheduleDto>> Generate([FromBody] GenerateScheduleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var schedule = await _generatorService.GenerateOptimalScheduleAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = schedule.Id }, schedule);
    }

    [HttpGet("suggestions")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetSuggestions(
        [FromQuery] DateTime date, [FromQuery] int count = 5)
    {
        var members = await _generatorService.GetSuggestedMembersAsync(date, count);
        return Ok(members);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ScheduleDto>> Update(int id, [FromBody] UpdateScheduleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var schedule = await _scheduleService.UpdateScheduleAsync(id, dto);
        return Ok(schedule);
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult<ScheduleDto>> Confirm(int id)
    {
        var schedule = await _scheduleService.ConfirmScheduleAsync(id);
        return Ok(schedule);
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<ScheduleDto>> Complete(int id)
    {
        var schedule = await _scheduleService.CompleteScheduleAsync(id);
        return Ok(schedule);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<ScheduleDto>> Cancel(int id)
    {
        var schedule = await _scheduleService.CancelScheduleAsync(id);
        return Ok(schedule);
    }

    [HttpPost("{id}/notify")]
    public async Task<IActionResult> SendNotifications(int id)
    {
        var schedule = await _scheduleService.GetScheduleByIdAsync(id);
        if (schedule == null)
            return NotFound(new { message = $"Escala com ID {id} não encontrada." });

        var success = await _whatsAppService.SendScheduleNotificationAsync(id);
        return Ok(new { success, message = success ? "Notificações enviadas." : "Algumas notificações falharam." });
    }

    [HttpPost("confirm-presence")]
    public async Task<ActionResult<ScheduleMemberDto>> ConfirmPresence([FromBody] ConfirmPresenceDto dto)
    {
        var result = await _scheduleService.ConfirmPresenceAsync(dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _scheduleService.DeleteScheduleAsync(id);
        return NoContent();
    }
}
