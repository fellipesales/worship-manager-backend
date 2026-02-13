using AutoMapper;
using Microsoft.Extensions.Logging;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Application.Services;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MemberService> _logger;
    private readonly ITenantService _tenantService;

    public MemberService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MemberService> logger, ITenantService tenantService)
    {
        _unitOfWork = unitOfWork; _mapper = mapper; _logger = logger; _tenantService = tenantService;
    }

    public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
    {
        var members = await _unitOfWork.Members.GetActiveMembersAsync();
        return _mapper.Map<IEnumerable<MemberDto>>(members);
    }

    public async Task<MemberDto?> GetMemberByIdAsync(int id)
    {
        var member = await _unitOfWork.Members.GetMemberWithDetailsAsync(id);
        return member == null ? null : _mapper.Map<MemberDto>(member);
    }

    public async Task<MemberDto?> GetMemberWithRolesAsync(int id)
    {
        var member = await _unitOfWork.Members.GetMemberWithDetailsAsync(id);
        if (member == null) return null;
        var dto = _mapper.Map<MemberDto>(member);
        dto.Roles = _mapper.Map<List<MemberRoleDto>>(member.MemberRoles);
        return dto;
    }

    public async Task<MemberDto> CreateMemberAsync(CreateMemberDto dto)
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        if (tenantId == null) throw new InvalidOperationException("Nenhuma organização selecionada.");

        var member = _mapper.Map<Member>(dto);
        member.OrganizationId = tenantId.Value;
        await _unitOfWork.Members.AddAsync(member);
        await _unitOfWork.SaveChangesAsync();

        if (dto.RoleIds.Any())
        {
            foreach (var roleId in dto.RoleIds)
            {
                var memberRole = new MemberRole
                {
                    MemberId = member.Id,
                    RoleId = roleId,
                    IsPrimary = roleId == dto.PrimaryRoleId
                };
                await _unitOfWork.MemberRoles.AddAsync(memberRole);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        return _mapper.Map<MemberDto>(await _unitOfWork.Members.GetMemberWithDetailsAsync(member.Id));
    }

    public async Task<MemberDto> UpdateMemberAsync(int id, UpdateMemberDto dto)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Membro com ID {id} não encontrado.");
        _mapper.Map(dto, member);
        _unitOfWork.Members.Update(member);

        await _unitOfWork.MemberRoles.DeleteByMemberIdAsync(id);
        if (dto.RoleIds.Any())
        {
            foreach (var roleId in dto.RoleIds)
            {
                var memberRole = new MemberRole { MemberId = id, RoleId = roleId, IsPrimary = roleId == dto.PrimaryRoleId };
                await _unitOfWork.MemberRoles.AddAsync(memberRole);
            }
        }
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<MemberDto>(await _unitOfWork.Members.GetMemberWithDetailsAsync(id));
    }

    public async Task DeleteMemberAsync(int id)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Membro com ID {id} não encontrado.");
        _unitOfWork.Members.Delete(member);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<MemberDto>> GetActiveMembersAsync()
    {
        var members = await _unitOfWork.Members.GetActiveMembersAsync();
        return _mapper.Map<IEnumerable<MemberDto>>(members);
    }

    public async Task<IEnumerable<MemberDto>> GetMembersByInstrumentAsync(string instrument)
    {
        var members = await _unitOfWork.Members.GetMembersByInstrumentAsync(instrument);
        return _mapper.Map<IEnumerable<MemberDto>>(members);
    }

    public async Task<IEnumerable<MemberStatisticsDto>> GetMemberStatisticsAsync()
    {
        var members = await _unitOfWork.Members.GetActiveMembersAsync();
        return _mapper.Map<IEnumerable<MemberStatisticsDto>>(members);
    }
}
