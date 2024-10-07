using Marten;
using Marten.Events.Projections;

namespace HelpDesk.Api.Catalog;

public class CatalogController(IDocumentSession session) : ControllerBase
{
    [HttpGet("/catalog")]
    public async Task<ActionResult> GetFullCatalogAsync()
    {
        var response = await session.get
        //    [new SoftwareCatalogItem]
        //    );
        return Ok();
    }
}

public class SoftwareCatalogItem
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

public class SoftwareCatalogItemProjection : MultiStreamProjection<SoftwareCatalogItem, Guid>
{
    public SoftwareCatalogItemProjection()
    {
        Identity<CatalogItemAdded>(i => i.Id);
        DeleteEvent<CatalogItemRetired>();
    }

    public void Apply(CatalogItemAdded @event, SoftwareCatalogItem view)
    {
        view.Id = @event.Id;
        view.Title = @event.Title;
        view.Description = @event.Description;
    }
}

