using System.ComponentModel.DataAnnotations;

namespace WorshipManager.Application.DTOs;

public class SongDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Artist { get; set; }
    public string? Key { get; set; }
    public string? Tempo { get; set; }
    public string? Category { get; set; }
    public string? SpotifyUrl { get; set; }
    public string? YouTubeUrl { get; set; }
    public string? ChordsUrl { get; set; }
    public string? Notes { get; set; }
    public string? Lyrics { get; set; }
    public int TimesUsed { get; set; }
    public DateTime? LastUsedDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool HasSpotifyLink => !string.IsNullOrEmpty(SpotifyUrl);
    public bool HasYouTubeLink => !string.IsNullOrEmpty(YouTubeUrl);
    public bool HasChordsLink => !string.IsNullOrEmpty(ChordsUrl);
}

public class CreateSongDto
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [MaxLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Artist { get; set; }

    [MaxLength(50)]
    public string? Key { get; set; }

    [MaxLength(20)]
    public string? Tempo { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    [MaxLength(500)]
    [Url(ErrorMessage = "URL do Spotify inválida")]
    public string? SpotifyUrl { get; set; }

    [MaxLength(500)]
    [Url(ErrorMessage = "URL do YouTube inválida")]
    public string? YouTubeUrl { get; set; }

    [MaxLength(500)]
    [Url(ErrorMessage = "URL da cifra inválida")]
    public string? ChordsUrl { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    [MaxLength(2000)]
    public string? Lyrics { get; set; }
}

public class UpdateSongDto
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Artist { get; set; }

    [MaxLength(50)]
    public string? Key { get; set; }

    [MaxLength(20)]
    public string? Tempo { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    [MaxLength(500)]
    public string? SpotifyUrl { get; set; }

    [MaxLength(500)]
    public string? YouTubeUrl { get; set; }

    [MaxLength(500)]
    public string? ChordsUrl { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    [MaxLength(2000)]
    public string? Lyrics { get; set; }

    public bool IsActive { get; set; } = true;
}

public class ScheduleSongDto
{
    public int Id { get; set; }
    public int SongId { get; set; }
    public string SongTitle { get; set; } = string.Empty;
    public string? Artist { get; set; }
    public int Order { get; set; }
    public string? Key { get; set; }
    public string? OriginalKey { get; set; }
    public string? Notes { get; set; }
    public int? DurationMinutes { get; set; }
    public string? SpotifyUrl { get; set; }
    public string? YouTubeUrl { get; set; }
    public string? ChordsUrl { get; set; }
}

public class AddScheduleSongDto
{
    [Required]
    public int SongId { get; set; }
    public int Order { get; set; }
    [MaxLength(50)]
    public string? CustomKey { get; set; }
    [MaxLength(500)]
    public string? Notes { get; set; }
    public int? DurationMinutes { get; set; }
}

public class SongFilterDto
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public string? Key { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public static class SongCategories
{
    public static readonly string[] Categories = new[]
    {
        "Adoração", "Celebração", "Comunhão", "Ofertório",
        "Entrada", "Encerramento", "Louvor", "Momento Profético",
        "Ministração", "Hino", "Coral", "Instrumental"
    };
}

public static class MusicalKeys
{
    public static readonly string[] Keys = new[]
    {
        "C", "C#", "Db", "D", "D#", "Eb", "E", "F", "F#", "Gb", "G", "G#", "Ab", "A", "A#", "Bb", "B",
        "Cm", "C#m", "Dm", "D#m", "Ebm", "Em", "Fm", "F#m", "Gm", "G#m", "Abm", "Am", "A#m", "Bbm", "Bm"
    };
}
