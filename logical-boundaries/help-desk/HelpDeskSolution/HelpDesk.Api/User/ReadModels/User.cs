using HelpDesk.Api.User.Events;
using Marten.Events;
using Marten.Events.Aggregation;

namespace HelpDesk.Api.User.ReadModels;

public record User
{
    public Guid Id { get; init; }
    public int Version { get; set; }
    public string Sub { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset LastLogin { get; init; }
}

public class UserProjection : SingleStreamProjection<User>
{
    public static User Create(IEvent<UserCreated> user)
    {
        return new User
        {
            Id = user.Data.Id,
            Created = user.Timestamp,
            Sub = user.Data.Sub,
            LastLogin = user.Timestamp
        };
    }

    public static User Apply(IEvent<UserLoggedIn> user, User view)
    {
        return view with { LastLogin = user.Timestamp };
    }
}