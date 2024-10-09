using Marten;
using SoftwareCatalogService.Outgoing;

namespace HelpDesk.Api.SoftwareCenter.Handlers;

public class CatalogItemHandler(ILogger<CatalogItemHandler> logger)
{

    public async Task HandleAsync(SoftwareCatalogItemCreated msg, IDocumentSession session, CancellationToken ct)
    {
        var @event = new CatalogItemAdded(Guid.NewGuid(), msg.Name, msg.Description);
        session.Events.StartStream(@event.Id, @event);
        logger.LogInformation("Got a new title from the software center, yo! {title}", msg.Name);
        await session.SaveChangesAsync(ct);
    }

    public async Task HandleAsync(SoftwareCatalogItemRetired msg, IDocumentSession session, CancellationToken ct)
    {

        var @event = new CatalogItemRetired(Guid.Parse(msg.Id));
        session.Events.Append(@event.Id, @event);
        logger.LogInformation("Retired something from the software center {Id}", msg.Id);
        await session.SaveChangesAsync(ct);
    }
}
