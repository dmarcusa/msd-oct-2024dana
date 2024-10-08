using Marten;

namespace HelpDesk.Api.SoftwareCenter.Endpoints;

[ApiExplorerSettings(GroupName = "Software Center Integration")]
[Produces("application/json")]
[Consumes("application/json")]
[Route("software-center/catalog")]
public class SoftwareCenterController(IDocumentSession session) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> AddItemToCatalogAsync([FromBody] CatalogItemRequestModel request,
        CancellationToken ct)
    {
        // whatever rules we have about this. can't think of anything now, but validate the input.
        var @event = new CatalogItemAdded(Guid.NewGuid(), request.Title, request.Description);
        session.Events.StartStream(@event.Id, @event);
        await session.SaveChangesAsync(ct);
        return Ok(@event);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteCatalogItemAsync(Guid id, CancellationToken ct)
    {
        // whatever rules (more on this tomorrow...)
        var @event = new CatalogItemRetired(id);
        session.Events.Append(@event.Id, @event);
        await session.SaveChangesAsync(ct);
        return NoContent();
    }
}

// Models (Commands?)

public record CatalogItemRequestModel(string Title, string Description);