using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Entities;
using WorshipManager.Core.Interfaces;

namespace WorshipManager.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ITenantService? _tenantService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    // Multi-tenant entities
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationSettings> OrganizationSettings => Set<OrganizationSettings>();
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();
    public new DbSet<Role> Roles => Set<Role>();
    public DbSet<MemberRole> MemberRoles => Set<MemberRole>();

    // Existing entities
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<ScheduleMember> ScheduleMembers => Set<ScheduleMember>();
    public DbSet<ScheduleHistory> ScheduleHistories => Set<ScheduleHistory>();
    public DbSet<MessageTemplate> MessageTemplates => Set<MessageTemplate>();
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<ScheduleSong> ScheduleSongs => Set<ScheduleSong>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========== MULTI-TENANT ENTITIES ==========

        // Organization configuration
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.Property(e => e.InviteCode).IsRequired().HasMaxLength(10);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.InviteCode).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // OrganizationSettings configuration
        modelBuilder.Entity<OrganizationSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Organization)
                .WithOne(o => o.Settings)
                .HasForeignKey<OrganizationSettings>(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OrganizationMember configuration
        modelBuilder.Entity<OrganizationMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.OrganizationId, e.UserId }).IsUnique();
            entity.HasIndex(e => e.UserId);

            entity.HasOne(e => e.Organization)
                .WithMany(o => o.OrganizationMembers)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.OrganizationMemberships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Member)
                .WithMany(m => m.OrganizationMembers)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.HasIndex(e => new { e.OrganizationId, e.Name });

            entity.HasOne(e => e.Organization)
                .WithMany(o => o.Roles)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // MemberRole configuration
        modelBuilder.Entity<MemberRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.MemberId, e.RoleId }).IsUnique();

            entity.HasOne(e => e.Member)
                .WithMany(m => m.MemberRoles)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.MemberRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ========== EXISTING ENTITIES (UPDATED FOR MULTI-TENANT) ==========

        // Member configuration
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Instrument).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.OrganizationId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.OrganizationId, e.UserId });

            entity.HasOne(e => e.Organization)
                .WithMany(o => o.Members)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Availability configuration
        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.OrganizationId);
            entity.HasIndex(e => new { e.MemberId, e.Date }).IsUnique();

            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Member)
                .WithMany(m => m.Availabilities)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Schedule configuration
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.OrganizationId);
            entity.HasIndex(e => new { e.Date, e.ServiceType });

            entity.HasOne(e => e.Organization)
                .WithMany(o => o.Schedules)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ScheduleMember configuration
        modelBuilder.Entity<ScheduleMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ScheduleId, e.MemberId }).IsUnique();

            entity.HasOne(e => e.Schedule)
                .WithMany(s => s.ScheduleMembers)
                .HasForeignKey(e => e.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Member)
                .WithMany(m => m.ScheduleMembers)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ScheduleHistory configuration
        modelBuilder.Entity<ScheduleHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ChangedBy).HasMaxLength(100);
            entity.HasIndex(e => e.ScheduleId);
            entity.HasIndex(e => e.ChangedAt);

            entity.HasOne(e => e.Schedule)
                .WithMany()
                .HasForeignKey(e => e.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // MessageTemplate configuration
        modelBuilder.Entity<MessageTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Template).IsRequired().HasMaxLength(2000);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.IsActive);
        });

        // Song configuration
        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Artist).HasMaxLength(200);
            entity.Property(e => e.Key).HasMaxLength(50);
            entity.Property(e => e.Tempo).HasMaxLength(20);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.SpotifyUrl).HasMaxLength(500);
            entity.Property(e => e.YouTubeUrl).HasMaxLength(500);
            entity.Property(e => e.ChordsUrl).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.Lyrics).HasMaxLength(2000);
            entity.HasIndex(e => e.OrganizationId);
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.OrganizationId, e.Title });

            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ScheduleSong configuration
        modelBuilder.Entity<ScheduleSong>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomKey).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasIndex(e => e.ScheduleId);
            entity.HasIndex(e => e.SongId);
            entity.HasIndex(e => new { e.ScheduleId, e.Order });

            entity.HasOne(e => e.Schedule)
                .WithMany(s => s.ScheduleSongs)
                .HasForeignKey(e => e.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Song)
                .WithMany(s => s.ScheduleSongs)
                .HasForeignKey(e => e.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }
}
