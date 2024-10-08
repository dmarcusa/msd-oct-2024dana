namespace HelpDesk.Api.SoftwareCenter;

public record CatalogItemAdded(Guid Id, string Title, string Description);

public record CatalogItemRetired(Guid Id);