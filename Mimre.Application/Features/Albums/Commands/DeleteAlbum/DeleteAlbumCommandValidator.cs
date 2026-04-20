using FluentValidation;

namespace Mimre.Application.Features.Albums.Commands.DeleteAlbum;

public class DeleteAlbumCommandValidator : AbstractValidator<DeleteAlbumCommand>
{
    public DeleteAlbumCommandValidator()
    {
        RuleFor(x => x.AlbumId).NotEmpty();
    }
}
