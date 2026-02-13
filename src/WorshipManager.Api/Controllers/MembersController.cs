using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;

namespace WorshipManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly ILogger<MembersController> _logger;

    public MembersController(IMemberService memberService, ILogger<MembersController> logger)
    {
        _memberService = memberService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAll()
    {
        var members = await _memberService.GetAllMembersAsync();
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetById(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound(new { message = $"Membro com ID {id} não encontrado." });
        return Ok(member);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetActive()
    {
        var members = await _memberService.GetActiveMembersAsync();
        return Ok(members);
    }

    [HttpGet("instrument/{instrument}")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetByInstrument(string instrument)
    {
        var members = await _memberService.GetMembersByInstrumentAsync(instrument);
        return Ok(members);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<IEnumerable<MemberStatisticsDto>>> GetStatistics()
    {
        var statistics = await _memberService.GetMemberStatisticsAsync();
        return Ok(statistics);
    }

    [HttpPost]
    public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var member = await _memberService.CreateMemberAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = member.Id }, member);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MemberDto>> Update(int id, [FromBody] UpdateMemberDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var member = await _memberService.UpdateMemberAsync(id, dto);
        return Ok(member);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _memberService.DeleteMemberAsync(id);
        return NoContent();
    }
}
