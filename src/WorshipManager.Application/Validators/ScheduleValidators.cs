using FluentValidation;
using WorshipManager.Application.DTOs;

namespace WorshipManager.Application.Validators;

public class CreateScheduleDtoValidator : AbstractValidator<CreateScheduleDto>
{
    public CreateScheduleDtoValidator()
    {
        RuleFor(x => x.Date).NotEmpty().GreaterThanOrEqualTo(DateTime.Today);
        RuleFor(x => x.ServiceType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class UpdateScheduleDtoValidator : AbstractValidator<UpdateScheduleDto>
{
    public UpdateScheduleDtoValidator()
    {
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.ServiceType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class GenerateScheduleDtoValidator : AbstractValidator<GenerateScheduleDto>
{
    public GenerateScheduleDtoValidator()
    {
        RuleFor(x => x).Must(dto => dto.Date.HasValue || dto.Dates.Any()).WithMessage("Pelo menos uma data é obrigatória.");
        RuleFor(x => x.Date).GreaterThanOrEqualTo(DateTime.Today).When(x => x.Date.HasValue);
        RuleForEach(x => x.Dates).GreaterThanOrEqualTo(DateTime.Today);
        RuleFor(x => x.ServiceType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MinimumMembers).GreaterThan(0);
    }
}

public class ConfirmPresenceDtoValidator : AbstractValidator<ConfirmPresenceDto>
{
    public ConfirmPresenceDtoValidator()
    {
        RuleFor(x => x.ScheduleMemberId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
