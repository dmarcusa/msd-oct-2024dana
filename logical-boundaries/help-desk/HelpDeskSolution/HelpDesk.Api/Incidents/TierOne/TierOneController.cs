using HelpDesk.Api.Services;
using Marten;

namespace HelpDesk.Api.Incidents.TierOne;

public class TierOneController(IDocumentSession session, IProvideUserInformation userInfo) : ControllerBase
{
    //POST /tierone/submitted-incidents/{id}/contact-records
    [HttpPost("/tierone/submitted-incidents/{incidentId:guid}/contact-records")]
    public async Task<ActionResult> AddContactRecordForIncident(
        Guid incidentId,
        [FromBody] ContactRecordRequest request)
    {
        var info = await userInfo.GetUserInfoAsync();
        // log an event to the event log.
        var evt = new IncidentContactRecorded(incidentId, info.UserId, request.Note);
        session.Events.Append(incidentId, evt);
        await session.SaveChangesAsync();

        return Ok(evt);
    }
}

public record ContactRecordRequest(string Note);

public record IncidentContactRecorded(Guid Id, Guid TierOneTechId, string Note);