using AutoMapper;
using Microsoft.Extensions.Logging;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Application.Services;

public class ScheduleGeneratorService : IScheduleGeneratorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ScheduleGeneratorService> _logger;
    private readonly ITenantService _tenantService;

    public ScheduleGeneratorService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScheduleGeneratorService> logger, ITenantService tenantService)
    {
        _unitOfWork = unitOfWork; _mapper = mapper; _logger = logger; _tenantService = tenantService;
    }

    public async Task<ScheduleDto> GenerateOptimalScheduleAsync(GenerateScheduleDto dto)
    {
        var tenantId = _tenantService.GetCurrentTenantId() ?? throw new InvalidOperationException("Nenhuma organização selecionada.");
        var dates = dto.GetAllDates();
        if (!dates.Any()) throw new InvalidOperationException("Nenhuma data informada.");

        var date = dates.First();
        var availableMembers = await _unitOfWork.Members.GetAvailableMembersForDateAsync(date);
        var memberList = availableMembers.ToList();
        if (memberList.Count < dto.MinimumMembers)
            _logger.LogWarning("Apenas {Count} membros disponíveis para {Date}", memberList.Count, date);

        var selectedMembers = memberList.OrderBy(m => m.ParticipationCount).ThenBy(m => m.LastParticipationDate ?? DateTime.MinValue).Take(Math.Max(dto.MinimumMembers, 5)).ToList();

        var schedule = new Schedule
        {
            OrganizationId = tenantId, Date = date, ServiceType = dto.ServiceType,
            Description = dto.Description, Status = ScheduleStatus.Draft,
            CreatedBy = _tenantService.GetCurrentUserId()
        };
        await _unitOfWork.Schedules.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        foreach (var member in selectedMembers)
        {
            await _unitOfWork.ScheduleMembers.AddAsync(new ScheduleMember { ScheduleId = schedule.Id, MemberId = member.Id });
            await _unitOfWork.Members.UpdateParticipationCountAsync(member.Id);
        }
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ScheduleDto>(await _unitOfWork.Schedules.GetScheduleWithMembersAsync(schedule.Id));
    }

    public async Task<IEnumerable<ScheduleDto>> GenerateMultipleSchedulesAsync(GenerateScheduleDto dto)
    {
        var results = new List<ScheduleDto>();
        foreach (var date in dto.GetAllDates())
        {
            var singleDto = new GenerateScheduleDto
            {
                Date = date, ServiceType = dto.ServiceType, Description = dto.Description,
                MinimumMembers = dto.MinimumMembers, RequiredInstruments = dto.RequiredInstruments,
                UseBalancing = dto.UseBalancing, MinDaysBetweenParticipations = dto.MinDaysBetweenParticipations
            };
            results.Add(await GenerateOptimalScheduleAsync(singleDto));
        }
        return results;
    }

    public async Task<IEnumerable<MemberDto>> GetSuggestedMembersAsync(DateTime date, int count = 5)
    {
        var availableMembers = await _unitOfWork.Members.GetAvailableMembersForDateAsync(date);
        var suggested = availableMembers.OrderBy(m => m.ParticipationCount).Take(count);
        return _mapper.Map<IEnumerable<MemberDto>>(suggested);
    }
}
