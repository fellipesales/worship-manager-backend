using AutoMapper;
using Microsoft.Extensions.Logging;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Application.Services;

public class ScheduleService : IScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ScheduleService> _logger;
    private readonly ITenantService _tenantService;

    public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScheduleService> logger, ITenantService tenantService)
    {
        _unitOfWork = unitOfWork; _mapper = mapper; _logger = logger; _tenantService = tenantService;
    }

    public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
    {
        var schedules = await _unitOfWork.Schedules.GetUpcomingSchedulesAsync(100);
        return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
    }

    public async Task<ScheduleDto?> GetScheduleByIdAsync(int id)
    {
        var schedule = await _unitOfWork.Schedules.GetScheduleWithMembersAsync(id);
        return schedule == null ? null : _mapper.Map<ScheduleDto>(schedule);
    }

    public async Task<IEnumerable<ScheduleCalendarDto>> GetScheduleCalendarAsync(DateTime startDate, DateTime endDate)
    {
        var schedules = await _unitOfWork.Schedules.GetSchedulesByDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<ScheduleCalendarDto>>(schedules);
    }

    public async Task<IEnumerable<ScheduleDto>> GetUpcomingSchedulesAsync(int count = 5)
    {
        var schedules = await _unitOfWork.Schedules.GetUpcomingSchedulesAsync(count);
        return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
    }

    public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto dto)
    {
        var tenantId = _tenantService.GetCurrentTenantId() ?? throw new InvalidOperationException("Nenhuma organização selecionada.");
        var schedule = _mapper.Map<Schedule>(dto);
        schedule.OrganizationId = tenantId;
        schedule.CreatedBy = _tenantService.GetCurrentUserId();
        await _unitOfWork.Schedules.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        foreach (var memberId in dto.MemberIds)
        {
            await _unitOfWork.ScheduleMembers.AddAsync(new ScheduleMember { ScheduleId = schedule.Id, MemberId = memberId });
            await _unitOfWork.Members.UpdateParticipationCountAsync(memberId);
        }
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ScheduleDto>(await _unitOfWork.Schedules.GetScheduleWithMembersAsync(schedule.Id));
    }

    public async Task<ScheduleDto> UpdateScheduleAsync(int id, UpdateScheduleDto dto)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Escala com ID {id} não encontrada.");
        _mapper.Map(dto, schedule);
        _unitOfWork.Schedules.Update(schedule);

        await _unitOfWork.ScheduleMembers.DeleteByScheduleIdAsync(id);
        foreach (var memberId in dto.MemberIds)
        {
            await _unitOfWork.ScheduleMembers.AddAsync(new ScheduleMember { ScheduleId = id, MemberId = memberId });
            await _unitOfWork.Members.UpdateParticipationCountAsync(memberId);
        }
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ScheduleDto>(await _unitOfWork.Schedules.GetScheduleWithMembersAsync(id));
    }

    public async Task DeleteScheduleAsync(int id) => await DeleteScheduleAsync(id, false);

    public async Task DeleteScheduleAsync(int id, bool forceDelete)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Escala com ID {id} não encontrada.");
        if (!forceDelete && schedule.Status == ScheduleStatus.Confirmed)
            throw new InvalidOperationException("Não é possível excluir uma escala confirmada.");
        await _unitOfWork.ScheduleMembers.DeleteByScheduleIdAsync(id);
        _unitOfWork.Schedules.Delete(schedule);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ScheduleDto> ConfirmScheduleAsync(int id)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Escala com ID {id} não encontrada.");
        schedule.Status = ScheduleStatus.Confirmed;
        schedule.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Schedules.Update(schedule);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ScheduleDto>(await _unitOfWork.Schedules.GetScheduleWithMembersAsync(id));
    }

    public async Task<ScheduleDto> CompleteScheduleAsync(int id)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Escala com ID {id} não encontrada.");
        schedule.Status = ScheduleStatus.Completed;
        schedule.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Schedules.Update(schedule);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ScheduleDto>(await _unitOfWork.Schedules.GetScheduleWithMembersAsync(id));
    }

    public async Task<ScheduleDto> CancelScheduleAsync(int id)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Escala com ID {id} não encontrada.");
        schedule.Status = ScheduleStatus.Cancelled;
        schedule.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Schedules.Update(schedule);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ScheduleDto>(await _unitOfWork.Schedules.GetScheduleWithMembersAsync(id));
    }

    public async Task<ScheduleMemberDto> ConfirmPresenceAsync(ConfirmPresenceDto dto)
    {
        var sm = await _unitOfWork.ScheduleMembers.GetByIdAsync(dto.ScheduleMemberId) ?? throw new KeyNotFoundException("Membro na escala não encontrado.");
        sm.ConfirmedPresence = dto.Confirmed;
        sm.ConfirmedAt = DateTime.UtcNow;
        sm.Notes = dto.Notes;
        _unitOfWork.ScheduleMembers.Update(sm);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ScheduleMemberDto>(sm);
    }
}
