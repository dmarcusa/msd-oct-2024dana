using HelpDesk.Api.TierOneSupport.ReadModels;
using Marten;

namespace HelpDesk.Api.TierOneSupport.Endpoints;

[ApiExplorerSettings(GroupName = "Tier One Support")]
public class QueriesController(IQuerySession session) : ControllerBase
{
    [HttpGet("/tierone/submitted-incidents")]
    public async Task<ActionResult> GetSubmittedIncidentsAsync(CancellationToken ct)
    {
        // get all incidents that are in the submitted state.
        var response = await session.Query<SubmittedIncident>().ToListAsync(ct);
        return Ok(response);
    }
}