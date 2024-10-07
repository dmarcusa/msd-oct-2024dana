using HelpDesk.Api.Incidents.TierOne;
using HelpDesk.Api.Services;
using Marten;
using Marten.Events;
//using Microsoft.AspNetCore.Authorization;

namespace HelpDesk.Api.Incidents;

public class UserIncidentsController(IProvideUserInformation userInfoProvider, IDocumentSession session) : ControllerBase
{
    [HttpPost("/user/authorized-software/{catalogId:guid}/incidents")]
    //[Authorize]
    public async Task<ActionResult> AddUserIncidentAsync(
        Guid catalogId,
        [FromBody] UserIncidentRequestModel request)
    {
        // TODO: if catalog id does not exit for this user or whatever, return a 404.
        // If it has to be approved for this particular user it should be 403

        // TODO: validate the request model, if it is bad return 400
        // TODO: mark this constroller or method req authentification, that will return 401 or 403
        var employeeId = await userInfoProvider.GetUserInfoAsync();
        var evt = new EmployeeLoggedIncident(Guid.NewGuid(), employeeId.UserId, catalogId, request.Description);

        // TODO: ??? Save it?
        //session.Insert(evt);
        //await session.SaveChangesAsync();

        session.Events.StartStream(evt.Id, evt);
        // A stream is a bunch of related things that happen to something over time.
        await session.SaveChangesAsync();

        return Ok(evt);
    }

    [HttpGet("/user/authorized-software/{catalogId:guid}/incidents/{incidentId:guid}")]
    public async Task<ActionResult> GetIncidentForCatalogItemAsync(Guid catalogId, Guid incidentId)
    {
        //TODO: verify catalog id
        //might be a little inefficent - it is going to read ALL the events in the stream
        //every time and create this
        //await session.Events.AggregateStreamAsync<IncidentReadModel>(incidentId);

        var readModel = await session.LoadAsync<IncidentReadModel>(incidentId);

        if (readModel == null)
        {
            return NotFound();
        }
        return Ok(readModel);
    }

    [HttpDelete("/user/incidents/{incidentId:guid}")]
    public async Task<ActionResult> CancelIncidentAsync(Guid incidentId)
    {
        // Decide - can they do this.
        //   - only the user that created this issue can delete - 403
        //   - don't mark one as cancelled if it doesn't exist.
        //   - it can only be deleted if it is in the PendingTeir1 review state, after that
        //     ?? Maybe the tier1 can delete it? ask? but we'll return a http Conflict response.

        // always return a 204. No content.

        //var savedIncident = await session.LoadAsync<IncidentReadModel>(incidentId);
        var rl = await session.Events.FetchForWriting<IncidentReadModel>(incidentId);
        var savedIncident = rl.Aggregate;
        if (savedIncident == null)
        {
            return NotFound();
        }
        if (savedIncident.Status != IncidentStatus.PendingTier1Review)
        {
            return StatusCode(409);
        }

        session.Events.Append(incidentId, new EmployeeCancelledIncident(incidentId));
        await session.SaveChangesAsync(); // only append this if the version of the read model is the same as when I read it originally
        return NoContent();
    }
}


// Model
public record UserIncidentRequestModel(string Description);

//Read Model

public enum IncidentStatus { PendingTier1Review, CustomerContacted }
public class IncidentReadModel
{
    public Guid Id { get; set; }
    public int Version { get; set; }

    public string Description { get; set; } = string.Empty;

    public Guid CatalogId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset Created { get; set; }

    public IncidentStatus Status { get; set; }

    public static IncidentReadModel Create(IEvent<EmployeeLoggedIncident> evt)
    {
        return new IncidentReadModel
        {
            Id = evt.Id,
            Description = evt.Data.Description,
            CatalogId = evt.Data.SoftwareId,
            UserId = evt.Data.EmployeeId,
            Created = evt.Timestamp,
            Status = IncidentStatus.PendingTier1Review
        };
    }

    public bool ShouldDelete(EmployeeCancelledIncident evt)
    {
        return Status == IncidentStatus.PendingTier1Review;
    }

    public void Apply(IncidentContactRecorded evt)
    {
        Status = IncidentStatus.CustomerContacted;
    }
}
