using FluentValidation;
using WorshipManager.Application.DTOs;

namespace WorshipManager.Application.Validators;

public class CreateAvailabilityDtoValidator : AbstractValidator<CreateAvailabilityDto>
{
    public CreateAvailabilityDtoValidator()
    {
        RuleFor(x => x.MemberId).GreaterThan(0);
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}

public class BulkAvailabilityDtoValidator : AbstractValidator<BulkAvailabilityDto>
{
    public BulkAvailabilityDtoValidator()
    {
        RuleFor(x => x.MemberId).GreaterThan(0);
        RuleFor(x => x).Must(dto => dto.Dates.Any() || dto.AvailableDates.Any() || dto.UnavailableDates.Any()).WithMessage("Pelo menos uma data é obrigatória.");
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
