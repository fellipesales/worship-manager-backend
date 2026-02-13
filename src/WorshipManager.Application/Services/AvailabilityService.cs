using AutoMapper;
using Microsoft.Extensions.Logging;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AvailabilityService> _logger;
    private readonly ITenantService _tenantService;

    public AvailabilityService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AvailabilityService> logger, ITenantService tenantService)
    {
        _unitOfWork = unitOfWork; _mapper = mapper; _logger = logger; _tenantService = tenantService;
    }

    public async Task<IEnumerable<AvailabilityDto>> GetMemberAvailabilitiesAsync(int memberId)
    {
        var availabilities = await _unitOfWork.Availabilities.GetByMemberIdAsync(memberId);
        return _mapper.Map<IEnumerable<AvailabilityDto>>(availabilities);
    }

    public async Task<IEnumerable<AvailabilityDto>> GetMemberAvailabilityAsync(int memberId, DateTime startDate, DateTime endDate)
    {
        var all = await _unitOfWork.Availabilities.GetByDateRangeAsync(startDate, endDate);
        var filtered = all.Where(a => a.MemberId == memberId);
        return _mapper.Map<IEnumerable<AvailabilityDto>>(filtered);
    }

    public async Task<AvailabilityCalendarDto> GetAvailabilityForDateAsync(DateTime date)
    {
        var availabilities = await _unitOfWork.Availabilities.GetByDateAsync(date);
        var activeMembers = await _unitOfWork.Members.GetActiveMembersAsync();

        var unavailableIds = availabilities.Where(a => !a.IsAvailable).Select(a => a.MemberId).ToHashSet();
        var result = new AvailabilityCalendarDto { Date = date };

        foreach (var member in activeMembers)
        {
            var info = new MemberAvailabilityInfo { MemberId = member.Id, Name = member.Name, Instrument = member.Instrument ?? "" };
            if (unavailableIds.Contains(member.Id))
                result.UnavailableMembers.Add(info);
            else
                result.AvailableMembers.Add(info);
        }
        return result;
    }

    public async Task<IEnumerable<AvailabilityCalendarDto>> GetAvailabilityForRangeAsync(DateTime startDate, DateTime endDate)
    {
        var results = new List<AvailabilityCalendarDto>();
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            results.Add(await GetAvailabilityForDateAsync(date));
        return results;
    }

    public async Task<AvailabilityDto> SetAvailabilityAsync(CreateAvailabilityDto dto)
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        if (tenantId == null) throw new InvalidOperationException("Nenhuma organização selecionada.");

        var existing = await _unitOfWork.Availabilities.GetByMemberAndDateAsync(dto.MemberId, dto.Date);
        if (existing != null)
        {
            existing.IsAvailable = dto.IsAvailable;
            existing.Notes = dto.Notes;
            existing.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Availabilities.Update(existing);
        }
        else
        {
            var availability = _mapper.Map<Availability>(dto);
            availability.OrganizationId = tenantId.Value;
            await _unitOfWork.Availabilities.AddAsync(availability);
        }
        await _unitOfWork.SaveChangesAsync();

        var result = await _unitOfWork.Availabilities.GetByMemberAndDateAsync(dto.MemberId, dto.Date);
        return _mapper.Map<AvailabilityDto>(result);
    }

    public async Task<IEnumerable<AvailabilityDto>> SetBulkAvailabilityAsync(BulkAvailabilityDto dto)
    {
        var results = new List<AvailabilityDto>();

        if (dto.AvailableDates.Any() || dto.UnavailableDates.Any())
        {
            foreach (var date in dto.AvailableDates)
                results.Add(await SetAvailabilityAsync(new CreateAvailabilityDto { MemberId = dto.MemberId, Date = date, IsAvailable = true, Notes = dto.Notes }));
            foreach (var date in dto.UnavailableDates)
                results.Add(await SetAvailabilityAsync(new CreateAvailabilityDto { MemberId = dto.MemberId, Date = date, IsAvailable = false, Notes = dto.Notes }));
        }
        else
        {
            foreach (var date in dto.Dates)
                results.Add(await SetAvailabilityAsync(new CreateAvailabilityDto { MemberId = dto.MemberId, Date = date, IsAvailable = dto.IsAvailable, Notes = dto.Notes }));
        }
        return results;
    }

    public async Task DeleteAvailabilityAsync(int id)
    {
        var availability = await _unitOfWork.Availabilities.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Disponibilidade com ID {id} não encontrada.");
        _unitOfWork.Availabilities.Delete(availability);
        await _unitOfWork.SaveChangesAsync();
    }
}
