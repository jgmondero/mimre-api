using FluentValidation;

namespace Mimre.Application.Features.Galleries.Commands.CreateGallery;

public class CreateGalleryCommandValidator : AbstractValidator<CreateGalleryCommand>
{
    public CreateGalleryCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}
