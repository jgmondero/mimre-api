using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Photos.Commands.UploadPhoto;

public record UploadPhotoCommand(
    Guid AlbumId,
    Guid UserId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    Stream FileStream) : IRequest<PhotoDto>;
