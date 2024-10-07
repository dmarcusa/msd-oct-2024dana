using HelpDesk.Api.Services;
using Marten;

namespace HelpDesk.Api.User.Services;

public class FakeDevelopmentUserInformation(IDocumentSession session, ILogger<FakeDevelopmentUserInformation> logger) : IProvideUserInformation
{
    public async Task<UserInfo> GetUserInfoAsync()
    { 
       logger.LogWarning("Using a Fake User Information Provider, No Not Use In Production");
        var sub = "bob";
        var fakeId = Guid.Parse("2ea27f79-ec2a-4b8f-a77b-721a91eb01b2");
        var user = await session.Query<ReadModels.User>().Where(u => u.Sub == sub).FirstOrDefaultAsync();
        if (user == null)
        {
            session.Events.StartStream(fakeId, new UserCreated(fakeId, sub));
        }
        else
        {
            session.Events.Append(user.Id, new UserLoggedIn(user.Id));
        }
        await session.SaveChangesAsync();
        
        return new UserInfo(fakeId);
    }
}



