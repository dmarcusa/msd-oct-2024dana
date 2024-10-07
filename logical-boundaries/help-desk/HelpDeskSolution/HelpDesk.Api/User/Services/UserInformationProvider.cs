using HelpDesk.Api.Services;
using Marten;

namespace HelpDesk.Api.User.Services;

public class UserInformationProvider(IHttpContextAccessor context, IDocumentSession session) : IProvideUserInformation
{
    public async Task<UserInfo> GetUserInfoAsync()
    {
        var sub = context.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ??
                  throw new Exception("Cannot be used in a non-authenticated environment");


        var user = await session.Query<User.ReadModels.User>().Where(u => u.Sub == sub).FirstOrDefaultAsync();
        if (user == null)
        {
            var id = Guid.NewGuid();
            session.Events.StartStream(id, new UserCreated(id, sub));
            await session.SaveChangesAsync();
            return new UserInfo(id);
        }
        else
        {
            session.Events.Append(user.Id, new UserLoggedIn(user.Id));
            await session.SaveChangesAsync();
            return new UserInfo(user.Id);
        }
    }
}