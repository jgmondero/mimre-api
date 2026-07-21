using FluentValidation;

namespace Mimre.Application.Features.Galleries.Commands.SetGalleryPassword;

public class SetGalleryPasswordCommandValidator : AbstractValidator<SetGalleryPasswordCommand>
{
    public SetGalleryPasswordCommandValidator()
    {
        RuleFor(x => x.Password)
            .MinimumLength(4)
            .WithMessage("Gallery password must be at least 4 characters.")
            .MaximumLength(100)
            .WithMessage("Gallery password cannot exceed 100 characters.")
            .When(x => x.Password is not null);
    }
}
