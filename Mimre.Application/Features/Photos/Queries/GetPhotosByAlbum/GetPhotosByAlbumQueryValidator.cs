using FluentValidation;

namespace Mimre.Application.Features.Photos.Queries.GetPhotosByAlbum;

public class GetPhotosByAlbumQueryValidator : AbstractValidator<GetPhotosByAlbumQuery>
{
    public GetPhotosByAlbumQueryValidator()
    {
        RuleFor(x => x.AlbumId)
            .NotEmpty();

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100.");
    }
}
