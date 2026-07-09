using FluentValidation;

namespace Mimre.Application.Features.Albums.Queries.GetAlbumsByGallery;

public class GetAlbumsByGalleryQueryValidator : AbstractValidator<GetAlbumsByGalleryQuery>
{
    public GetAlbumsByGalleryQueryValidator()
    {
        RuleFor(x => x.GalleryId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100.");
    }
}
