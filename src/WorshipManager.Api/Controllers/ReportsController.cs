using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;

namespace WorshipManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var dashboard = await _reportService.GetDashboardDataAsync();
        return Ok(dashboard);
    }

    [HttpGet("participation")]
    public async Task<ActionResult<ParticipationReportDto>> GetParticipationReport([FromQuery] ReportFilterDto filter)
    {
        var report = await _reportService.GetParticipationReportAsync(filter);
        return Ok(report);
    }

    [HttpGet("instruments")]
    public async Task<ActionResult<IEnumerable<InstrumentStatisticsDto>>> GetInstrumentStatistics([FromQuery] ReportFilterDto filter)
    {
        var stats = await _reportService.GetInstrumentStatisticsAsync(filter);
        return Ok(stats);
    }

    [HttpGet("schedules/{scheduleId}/pdf")]
    public async Task<IActionResult> ExportScheduleToPdf(int scheduleId)
    {
        var pdfBytes = await _reportService.ExportScheduleToPdfAsync(scheduleId);
        return File(pdfBytes, "application/pdf", $"escala_{scheduleId}.pdf");
    }

    [HttpGet("participation/excel")]
    public async Task<IActionResult> ExportToExcel([FromQuery] ReportFilterDto filter)
    {
        var excelBytes = await _reportService.ExportReportToExcelAsync(filter);
        return File(excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"relatorio_participacao_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpGet("participation/pdf")]
    public async Task<IActionResult> ExportParticipationToPdf([FromQuery] ReportFilterDto filter)
    {
        var pdfBytes = await _reportService.ExportParticipationReportToPdfAsync(filter);
        return File(pdfBytes, "application/pdf", $"relatorio_participacao_{DateTime.Now:yyyyMMdd}.pdf");
    }
}
