using Microsoft.EntityFrameworkCore.Storage;
using WorshipManager.Core.Interfaces;
using WorshipManager.Infrastructure.Data;
using WorshipManager.Infrastructure.Repositories;

namespace WorshipManager.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private IDbContextTransaction? _transaction;

    private IMemberRepository? _members;
    private IRoleRepository? _roles;
    private IMemberRoleRepository? _memberRoles;
    private IAvailabilityRepository? _availabilities;
    private IScheduleRepository? _schedules;
    private IScheduleMemberRepository? _scheduleMembers;
    private IScheduleHistoryRepository? _scheduleHistories;
    private IMessageTemplateRepository? _messageTemplates;
    private ISongRepository? _songs;
    private IScheduleSongRepository? _scheduleSongs;

    public UnitOfWork(ApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public IMemberRepository Members => _members ??= new MemberRepository(_context, _tenantService);
    public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
    public IMemberRoleRepository MemberRoles => _memberRoles ??= new MemberRoleRepository(_context);
    public IAvailabilityRepository Availabilities => _availabilities ??= new AvailabilityRepository(_context, _tenantService);
    public IScheduleRepository Schedules => _schedules ??= new ScheduleRepository(_context, _tenantService);
    public IScheduleMemberRepository ScheduleMembers => _scheduleMembers ??= new ScheduleMemberRepository(_context);
    public IScheduleHistoryRepository ScheduleHistories => _scheduleHistories ??= new ScheduleHistoryRepository(_context);
    public IMessageTemplateRepository MessageTemplates => _messageTemplates ??= new MessageTemplateRepository(_context);
    public ISongRepository Songs => _songs ??= new SongRepository(_context, _tenantService);
    public IScheduleSongRepository ScheduleSongs => _scheduleSongs ??= new ScheduleSongRepository(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    public async Task BeginTransactionAsync() { _transaction = await _context.Database.BeginTransactionAsync(); }
    public async Task CommitTransactionAsync() { if (_transaction != null) { await _transaction.CommitAsync(); await _transaction.DisposeAsync(); _transaction = null; } }
    public async Task RollbackTransactionAsync() { if (_transaction != null) { await _transaction.RollbackAsync(); await _transaction.DisposeAsync(); _transaction = null; } }
    public void Dispose() { _transaction?.Dispose(); _context.Dispose(); }
}
