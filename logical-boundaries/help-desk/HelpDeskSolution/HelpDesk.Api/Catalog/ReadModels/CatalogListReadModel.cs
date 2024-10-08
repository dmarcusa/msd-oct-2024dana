using HelpDesk.Api.SoftwareCenter;
using Marten.Events.Aggregation;

namespace HelpDesk.Api.Catalog.ReadModels;

public record CatalogItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CatalogItemProjection : SingleStreamProjection<CatalogItem>
{
    public CatalogItemProjection()
    {
        DeleteEvent<CatalogItemRetired>(e => true);
    }

    public void Apply(CatalogItemAdded @event, CatalogItem view)
    {
        view.Id = @event.Id;
        view.Title = @event.Title;
        view.Description = @event.Description;
    }
}