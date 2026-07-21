using FluentValidation;

namespace Mimre.Application.Features.ShareLinks.Commands.VerifyGalleryPassword;

public class VerifyGalleryPasswordCommandValidator : AbstractValidator<VerifyGalleryPasswordCommand>
{
    public VerifyGalleryPasswordCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
