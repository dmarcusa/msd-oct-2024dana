using Marten;

namespace HelpDesk.Api.Catalog;

[Route("/software-center/catalog")]
public class SoftwareCenterController(IDocumentSession session) : ControllerBase
{
    [HttpPost("catalog")]
    public async Task<ActionResult> AddItemToCatalogAsync([FromBody] CatalogItemRequestModel request)
    {
        var evt = new CatalogItemAdded(Guid.NewGuid(), request.Title, request.Description);
        session.Events.StartStream(evt.Id, evt);
        await session.SaveChangesAsync();
        return Ok(evt);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCatalogId(Guid id)
    {
        var evt = new CatalogItemRetired(id);
        session.Events.Append(evt.Id, evt);
        await session.SaveChangesAsync();
        return NoContent();
    }
}
//Events

public record CatalogItemRequestModel(string Title, string Description);
public record CatalogItemAdded(Guid Id, string Title, string Description);

public record CatalogItemRetired(Guid Id);

