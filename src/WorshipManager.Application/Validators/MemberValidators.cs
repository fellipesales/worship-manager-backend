using FluentValidation;
using WorshipManager.Application.DTOs;

namespace WorshipManager.Application.Validators;

public class CreateMemberDtoValidator : AbstractValidator<CreateMemberDto>
{
    public CreateMemberDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Nome é obrigatório.").MaximumLength(100);
        RuleFor(x => x.Instrument).MaximumLength(50);
        RuleFor(x => x.Phone).MaximumLength(20).Matches(@"^[\d\s\-\+\(\)]*$").When(x => !string.IsNullOrEmpty(x.Phone)).WithMessage("Telefone inválido.");
        RuleFor(x => x.Email).MaximumLength(100).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.PrimaryRoleId).Must((dto, id) => !id.HasValue || dto.RoleIds.Contains(id.Value)).WithMessage("A função principal deve estar na lista de funções selecionadas.");
    }
}

public class UpdateMemberDtoValidator : AbstractValidator<UpdateMemberDto>
{
    public UpdateMemberDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Nome é obrigatório.").MaximumLength(100);
        RuleFor(x => x.Instrument).MaximumLength(50);
        RuleFor(x => x.Phone).MaximumLength(20).Matches(@"^[\d\s\-\+\(\)]*$").When(x => !string.IsNullOrEmpty(x.Phone)).WithMessage("Telefone inválido.");
        RuleFor(x => x.Email).MaximumLength(100).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.PrimaryRoleId).Must((dto, id) => !id.HasValue || dto.RoleIds.Contains(id.Value)).WithMessage("A função principal deve estar na lista de funções selecionadas.");
    }
}
