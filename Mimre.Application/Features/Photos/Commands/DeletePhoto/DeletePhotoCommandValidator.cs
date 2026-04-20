using FluentValidation;

namespace Mimre.Application.Features.Photos.Commands.DeletePhoto;

public class DeletePhotoCommandValidator : AbstractValidator<DeletePhotoCommand>
{
    public DeletePhotoCommandValidator()
    {
        RuleFor(x => x.PhotoId).NotEmpty();
    }
}
