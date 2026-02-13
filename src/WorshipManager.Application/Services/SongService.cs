using AutoMapper;
using Microsoft.Extensions.Logging;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Application.Services;

public class SongService : ISongService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<SongService> _logger;
    private readonly ITenantService _tenantService;

    public SongService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SongService> logger, ITenantService tenantService)
    {
        _unitOfWork = unitOfWork; _mapper = mapper; _logger = logger; _tenantService = tenantService;
    }

    public async Task<IEnumerable<SongDto>> GetAllSongsAsync()
    {
        var tenantId = _tenantService.GetCurrentTenantId() ?? throw new InvalidOperationException("Nenhuma organização selecionada.");
        return _mapper.Map<IEnumerable<SongDto>>(await _unitOfWork.Songs.GetAllByOrganizationAsync(tenantId));
    }

    public async Task<IEnumerable<SongDto>> GetActiveSongsAsync()
    {
        var tenantId = _tenantService.GetCurrentTenantId() ?? throw new InvalidOperationException("Nenhuma organização selecionada.");
        return _mapper.Map<IEnumerable<SongDto>>(await _unitOfWork.Songs.GetActiveSongsAsync(tenantId));
    }

    public async Task<SongDto?> GetSongByIdAsync(int id)
    {
        var song = await _unitOfWork.Songs.GetByIdAsync(id);
        return song == null ? null : _mapper.Map<SongDto>(song);
    }

    public async Task<IEnumerable<SongDto>> GetSongsByCategoryAsync(string category)
    {
        var tenantId = _tenantService.GetCurrentTenantId() ?? throw new InvalidOperationException("Nenhuma organização selecionada.");
        return _mapper.Map<IEnumerable<SongDto>>(await _unitOfWork.Songs.GetByCategoryAsync(tenantId, category));
    }

    public async Task<IEnumerable<SongDto>> SearchSongsAsync(string searchTerm)
    {
        var tenantId = _tenantService.GetCurrentTenantId() ?? throw new InvalidOperationException("Nenhuma organização selecionada.");
        return _mapper.Map<IEnumerable<SongDto>>(await _unitOfWork.Songs.SearchSongsAsync(tenantId, searchTerm));
    }

    public async Task<SongDto> CreateSongAsync(CreateSongDto dto)
    {
        var tenantId = _tenantService.GetCurrentTenantId() ?? throw new InvalidOperationException("Nenhuma organização selecionada.");
        var song = _mapper.Map<Song>(dto);
        song.OrganizationId = tenantId;
        await _unitOfWork.Songs.AddAsync(song);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SongDto>(song);
    }

    public async Task<SongDto> UpdateSongAsync(int id, UpdateSongDto dto)
    {
        var song = await _unitOfWork.Songs.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Música com ID {id} não encontrada.");
        _mapper.Map(dto, song);
        _unitOfWork.Songs.Update(song);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SongDto>(song);
    }

    public async Task DeleteSongAsync(int id)
    {
        var song = await _unitOfWork.Songs.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Música com ID {id} não encontrada.");
        _unitOfWork.Songs.Delete(song);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<ScheduleSongDto>> GetScheduleSongsAsync(int scheduleId)
    {
        var songs = await _unitOfWork.ScheduleSongs.GetByScheduleIdAsync(scheduleId);
        return _mapper.Map<IEnumerable<ScheduleSongDto>>(songs);
    }

    public async Task<ScheduleSongDto> AddSongToScheduleAsync(int scheduleId, AddScheduleSongDto dto)
    {
        var scheduleSong = new ScheduleSong { ScheduleId = scheduleId, SongId = dto.SongId, Order = dto.Order, CustomKey = dto.CustomKey, Notes = dto.Notes, DurationMinutes = dto.DurationMinutes };
        await _unitOfWork.ScheduleSongs.AddAsync(scheduleSong);
        await _unitOfWork.Songs.IncrementUsageCountAsync(dto.SongId, DateTime.UtcNow);
        await _unitOfWork.SaveChangesAsync();
        var result = (await _unitOfWork.ScheduleSongs.GetByScheduleIdAsync(scheduleId)).FirstOrDefault(ss => ss.Id == scheduleSong.Id);
        return _mapper.Map<ScheduleSongDto>(result);
    }

    public async Task RemoveSongFromScheduleAsync(int scheduleId, int songId)
    {
        var songs = await _unitOfWork.ScheduleSongs.GetByScheduleIdAsync(scheduleId);
        var ss = songs.FirstOrDefault(s => s.SongId == songId);
        if (ss != null) { _unitOfWork.ScheduleSongs.Delete(ss); await _unitOfWork.SaveChangesAsync(); }
    }

    public async Task ReorderScheduleSongsAsync(int scheduleId, List<int> songIdsInOrder)
    {
        for (int i = 0; i < songIdsInOrder.Count; i++)
        {
            var songs = await _unitOfWork.ScheduleSongs.GetByScheduleIdAsync(scheduleId);
            var ss = songs.FirstOrDefault(s => s.SongId == songIdsInOrder[i]);
            if (ss != null) await _unitOfWork.ScheduleSongs.UpdateOrderAsync(ss.Id, i + 1);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateScheduleSongAsync(int scheduleSongId, AddScheduleSongDto dto)
    {
        var ss = await _unitOfWork.ScheduleSongs.GetByIdAsync(scheduleSongId) ?? throw new KeyNotFoundException("Música na escala não encontrada.");
        ss.Order = dto.Order; ss.CustomKey = dto.CustomKey; ss.Notes = dto.Notes; ss.DurationMinutes = dto.DurationMinutes;
        _unitOfWork.ScheduleSongs.Update(ss);
        await _unitOfWork.SaveChangesAsync();
    }
}
