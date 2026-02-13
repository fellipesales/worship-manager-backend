using FluentValidation;
using WorshipManager.Application.DTOs;

namespace WorshipManager.Application.Validators;

public class CreateSongDtoValidator : AbstractValidator<CreateSongDto>
{
    public CreateSongDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Artist).MaximumLength(200);
        RuleFor(x => x.Key).MaximumLength(50).Must(key => string.IsNullOrEmpty(key) || MusicalKeys.Keys.Contains(key)).WithMessage("Tom musical inválido.");
        RuleFor(x => x.Category).MaximumLength(50);
        RuleFor(x => x.SpotifyUrl).MaximumLength(500);
        RuleFor(x => x.YouTubeUrl).MaximumLength(500);
        RuleFor(x => x.ChordsUrl).MaximumLength(500);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.Lyrics).MaximumLength(2000);
    }
}

public class UpdateSongDtoValidator : AbstractValidator<UpdateSongDto>
{
    public UpdateSongDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Artist).MaximumLength(200);
        RuleFor(x => x.Key).MaximumLength(50).Must(key => string.IsNullOrEmpty(key) || MusicalKeys.Keys.Contains(key)).WithMessage("Tom musical inválido.");
        RuleFor(x => x.Category).MaximumLength(50);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.Lyrics).MaximumLength(2000);
    }
}

public class AddScheduleSongDtoValidator : AbstractValidator<AddScheduleSongDto>
{
    public AddScheduleSongDtoValidator()
    {
        RuleFor(x => x.SongId).GreaterThan(0);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CustomKey).MaximumLength(50);
        RuleFor(x => x.Notes).MaximumLength(500);
        RuleFor(x => x.DurationMinutes).GreaterThan(0).When(x => x.DurationMinutes.HasValue);
    }
}
