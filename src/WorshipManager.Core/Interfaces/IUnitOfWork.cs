namespace WorshipManager.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IMemberRepository Members { get; }
    IRoleRepository Roles { get; }
    IMemberRoleRepository MemberRoles { get; }
    IAvailabilityRepository Availabilities { get; }
    IScheduleRepository Schedules { get; }
    IScheduleMemberRepository ScheduleMembers { get; }
    IScheduleHistoryRepository ScheduleHistories { get; }
    IMessageTemplateRepository MessageTemplates { get; }
    ISongRepository Songs { get; }
    IScheduleSongRepository ScheduleSongs { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
