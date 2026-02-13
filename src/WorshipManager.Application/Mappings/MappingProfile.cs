using AutoMapper;
using WorshipManager.Application.DTOs;
using WorshipManager.Core.Entities;

namespace WorshipManager.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Member, MemberDto>();
        CreateMap<CreateMemberDto, Member>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
        CreateMap<UpdateMemberDto, Member>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Availability, AvailabilityDto>()
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name));
        CreateMap<CreateAvailabilityDto, Availability>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Schedule, ScheduleDto>()
            .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.ScheduleMembers));
        CreateMap<CreateScheduleDto, Schedule>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => ScheduleStatus.Draft));
        CreateMap<UpdateScheduleDto, Schedule>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<ScheduleMember, ScheduleMemberDto>()
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name))
            .ForMember(dest => dest.Instrument, opt => opt.MapFrom(src => src.Member.Instrument))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Member.Phone));

        CreateMap<Schedule, ScheduleCalendarDto>()
            .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.ScheduleMembers.Count));

        CreateMap<Member, MemberStatisticsDto>()
            .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TotalParticipations, opt => opt.MapFrom(src => src.ParticipationCount))
            .ForMember(dest => dest.LastParticipation, opt => opt.MapFrom(src => src.LastParticipationDate))
            .ForMember(dest => dest.DaysSinceLastParticipation, opt => opt.MapFrom(src =>
                src.LastParticipationDate.HasValue
                    ? (int)(DateTime.UtcNow - src.LastParticipationDate.Value).TotalDays
                    : -1));

        CreateMap<Role, RoleDto>();

        CreateMap<MemberRole, MemberRoleDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Role.Category))
            .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Role.Icon));

        CreateMap<Song, SongDto>();
        CreateMap<CreateSongDto, Song>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
        CreateMap<UpdateSongDto, Song>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<ScheduleSong, ScheduleSongDto>()
            .ForMember(dest => dest.SongTitle, opt => opt.MapFrom(src => src.Song.Title))
            .ForMember(dest => dest.Artist, opt => opt.MapFrom(src => src.Song.Artist))
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.CustomKey ?? src.Song.Key))
            .ForMember(dest => dest.OriginalKey, opt => opt.MapFrom(src => src.Song.Key))
            .ForMember(dest => dest.SpotifyUrl, opt => opt.MapFrom(src => src.Song.SpotifyUrl))
            .ForMember(dest => dest.YouTubeUrl, opt => opt.MapFrom(src => src.Song.YouTubeUrl))
            .ForMember(dest => dest.ChordsUrl, opt => opt.MapFrom(src => src.Song.ChordsUrl));
    }
}
