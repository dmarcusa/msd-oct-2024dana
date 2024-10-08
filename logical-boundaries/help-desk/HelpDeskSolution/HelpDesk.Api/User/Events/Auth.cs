namespace HelpDesk.Api.User.Events;

public record UserCreated(Guid Id, string Sub);

public record UserLoggedIn(Guid Id);