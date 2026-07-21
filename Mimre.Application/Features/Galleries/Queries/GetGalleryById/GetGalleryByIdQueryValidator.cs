using FluentValidation;

namespace Mimre.Application.Features.Galleries.Queries.GetGalleryById;

public class GetGalleryByIdQueryValidator : AbstractValidator<GetGalleryByIdQuery>
{
    public GetGalleryByIdQueryValidator()
    {
        RuleFor(x => x.GalleryId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
