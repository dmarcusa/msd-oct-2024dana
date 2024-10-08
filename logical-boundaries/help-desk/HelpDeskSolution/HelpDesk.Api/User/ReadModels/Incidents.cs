using HelpDesk.Api.Shared;
using HelpDesk.Api.TierOneSupport.Events;
using HelpDesk.Api.User.Events;
using Marten.Events;

namespace HelpDesk.Api.User.ReadModels;

public class Incident
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CatalogId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset Created { get; set; }
    public IncidentStatus Status { get; set; }

    public static Incident Create(IEvent<EmployeeLoggedIncident> evt)
    {
        return new Incident
        {
            Id = evt.Id,
            Description = evt.Data.Description,
            CatalogId = evt.Data.SoftwareId,
            UserId = evt.Data.EmployeeId,
            Created = evt.Timestamp,
            Status = IncidentStatus.PendingTier1Review
        };
    }

    public void Apply(IncidentContactRecorded evt)
    {
        Status = IncidentStatus.CustomerContacted;
    }

    public bool ShouldDelete(EmployeeCancelledIncident evt)
    {
        return Status == IncidentStatus.PendingTier1Review; // the suspenders to your belt.
    }
}