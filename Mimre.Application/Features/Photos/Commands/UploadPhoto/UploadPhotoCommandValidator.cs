using FluentValidation;

namespace Mimre.Application.Features.Photos.Commands.UploadPhoto;

public class UploadPhotoCommandValidator : AbstractValidator<UploadPhotoCommand>
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "image/heic",
        "image/heif"
    ];

    public UploadPhotoCommandValidator()
    {
        RuleFor(x => x.AlbumId)
            .NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(512);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(ct => AllowedContentTypes.Contains(ct.ToLowerInvariant()))
            .WithMessage("File type not supported. Allowed: JPEG, PNG, WebP, HEIC.");

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .WithMessage("File cannot be empty.")
            .LessThanOrEqualTo(50 * 1024 * 1024)
            .WithMessage("File size cannot exceed 50MB.");
    }
}
