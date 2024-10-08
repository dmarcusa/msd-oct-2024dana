using Marten;
using SoftwareCatalogService.Outgoing;

namespace HelpDesk.Api.SoftwareCenter.Handlers;

public class CatalogItemHandler(ILogger<CatalogItemHandler> logger)
{

    public async Task HandleAsync(SoftwareCatalogItemCreated msg, IDocumentSession session, CancellationToken ct)
    {
        var @event = new CatalogItemAdded(Guid.NewGuid(), msg.Name, msg.Description);
        session.Events.StartStream(@event.Id, @event);
        logger.LogInformation("Got another one from the serer yo! {title}", msg.Name);
        await session.SaveChangesAsync(ct);
    }

    public async Task HandleAsync(SoftwareCatalogItemCreated msg, IDocumentSession session, CancellationToken ct)
    {
        var @event = new CatalogItemRetired(Guid.Parse(msg.Id));
        session.Events.Append(@event.Id, @event);
        await session.SaveChangesAsync(ct);
    }
}