using HelpDesk.Api.User.ReadModels;
using HelpDesk.Api.User.Services;
using Marten;

namespace HelpDesk.Api.User.Endpoints.Incidents;

[ApiExplorerSettings(GroupName = "User Incidents")]
[Produces("application/json")]
public class QueryController(IQuerySession session, IProvideUserInformation userProvider) : ControllerBase
{
    /// <summary>
    ///     A list of the software catalog items the user is entitled to install.
    /// </summary>
    /// <returns></returns>
    [HttpGet("/user/catalog")]
    public ActionResult GetUserCatalogAsync()
    {
        // placeholder - there is a backlog story card (ha!) that says user's have access to a subset of the catalog
        // e.g. only the catalog items they have been entitled to install. For now, we'll just return all of them.
        return Redirect("/catalog");
    }

    /// <summary>
    ///     A list of the current incidents that the user has reported.
    /// </summary>
    /// <returns></returns>
    [HttpGet("/user/incidents", Name = "GetUserIncidents")]
    public async Task<ActionResult> GetUserIncidentsAsync(CancellationToken ct)
    {
        var user = await userProvider.GetUserInfoAsync();
        var response = await session.Query<Incident>().Where(s => s.UserId == user.UserId).ToListAsync(ct);

        return Ok(response);
    }

    /// <summary>
    ///     Information about a specific incident that the user has reported.
    /// </summary>
    [HttpGet("/user/incidents/{incidentId:guid}")]
    public async Task<ActionResult> GetIncidentForCatalogItemAsync(Guid incidentId, CancellationToken ct)
    {
        var user = await userProvider.GetUserInfoAsync();
        var readModel = await session
            .Query<Incident>()
            .Where(s => s.UserId == user.UserId && s.Id == incidentId)
            .SingleOrDefaultAsync(ct);


        if (readModel == null) return NotFound();
        // This will return a 404, but if you really wanted to, you could leave the userId out of the predicate
        // and then punch them for not minding their own business
        // if (user.UserId != readModel.UserId)
        // {
        //     return Forbid();
        // }
        return Ok(readModel);
    }
}