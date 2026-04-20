using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.ShareLinks.Commands.DeleteShareLink;

public class DeleteShareLinkCommandHandler(IUnitOfWork uow) : IRequestHandler<DeleteShareLinkCommand>
{
    public async Task Handle(DeleteShareLinkCommand request, CancellationToken ct)
    {
        var link = await uow.ShareLinks.GetByIdAsync(request.ShareLinkId, ct)
            ?? throw new NotFoundException(nameof(ShareLink), request.ShareLinkId);

        uow.ShareLinks.Remove(link);
        await uow.SaveChangesAsync(ct);
    }
}
