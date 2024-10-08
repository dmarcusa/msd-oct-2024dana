using HelpDesk.Api.TierOneSupport.Events;
using HelpDesk.Api.User.Services;
using Marten;

namespace HelpDesk.Api.TierOneSupport.Endpoints;

public record ContactRecordRequest(string Note);

[ApiExplorerSettings(GroupName = "Tier One Support")]
public class CommandsController(IDocumentSession session, IProvideUserInformation userInfo) : ControllerBase
{
    [HttpPost("/tierone/submitted-incidents/{incidentId:guid}/contact-records")]
    public async Task<ActionResult> AddContactRecordForIncident(
        Guid incidentId,
        [FromBody] ContactRecordRequest request, CancellationToken ct)
    {
        var info = await userInfo.GetUserInfoAsync();
        // log an event to the event log.
        var evt = new IncidentContactRecorded(incidentId, info.UserId, request.Note);
        session.Events.Append(incidentId, evt);
        await session.SaveChangesAsync(ct);

        return Ok(evt);
    }
}