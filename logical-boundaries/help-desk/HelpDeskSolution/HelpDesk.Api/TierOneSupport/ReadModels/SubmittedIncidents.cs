using HelpDesk.Api.Shared;
using HelpDesk.Api.TierOneSupport.Events;
using HelpDesk.Api.User.Events;
using Marten.Events;

namespace HelpDesk.Api.TierOneSupport.ReadModels;

public class SubmittedIncident
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CatalogId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset Created { get; set; }
    public IncidentStatus Status { get; set; }
    public List<Comment> Comments { get; set; } = new();

    public static SubmittedIncident Create(IEvent<EmployeeLoggedIncident> evt)
    {
        return new SubmittedIncident
        {
            Id = evt.Id,
            Description = evt.Data.Description,
            CatalogId = evt.Data.SoftwareId,
            UserId = evt.Data.EmployeeId,
            Created = evt.Timestamp,
            Status = IncidentStatus.PendingTier1Review
        };
    }

    public void Apply(IEvent<IncidentContactRecorded> evt)
    {
        Status = IncidentStatus.CustomerContacted;
        Comments.Add(new Comment(evt.Id, evt.Data.TierOneTechId, evt.Data.Note, evt.Timestamp));
    }

    public bool ShouldDelete(EmployeeCancelledIncident evt)
    {
        return Status == IncidentStatus.PendingTier1Review; // the suspenders to your belt.
    }
}

public record Comment(Guid Id, Guid TierOneTechId, string Note, DateTimeOffset Created);