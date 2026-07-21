using FluentValidation;

namespace Mimre.Application.Features.Photos.Commands.SetCoverPhoto;

public class SetCoverPhotoCommandValidator : AbstractValidator<SetCoverPhotoCommand>
{
    public SetCoverPhotoCommandValidator()
    {
        RuleFor(x => x.PhotoId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
