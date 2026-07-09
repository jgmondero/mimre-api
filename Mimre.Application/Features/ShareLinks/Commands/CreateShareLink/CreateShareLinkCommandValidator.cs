using FluentValidation;

namespace Mimre.Application.Features.ShareLinks.Commands.CreateShareLink;

public class CreateShareLinkCommandValidator : AbstractValidator<CreateShareLinkCommand>
{
    public CreateShareLinkCommandValidator()
    {
        RuleFor(x => x.GalleryId)
            .NotEmpty();

        RuleFor(x => x.ClientEmail)
            .EmailAddress()
            .WithMessage("Client email must be a valid email address.")
            .When(x => x.ClientEmail is not null);

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiry date must be in the future.")
            .When(x => x.ExpiresAt.HasValue);
    }
}
