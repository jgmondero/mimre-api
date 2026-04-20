using FluentValidation;

namespace Mimre.Application.Features.ShareLinks.Commands.DeleteShareLink;

public class DeleteShareLinkCommandValidator : AbstractValidator<DeleteShareLinkCommand>
{
    public DeleteShareLinkCommandValidator()
    {
        RuleFor(x => x.ShareLinkId).NotEmpty();
    }
}
