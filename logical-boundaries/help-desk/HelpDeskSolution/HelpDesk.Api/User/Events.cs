namespace HelpDesk.Api.User;

public record UserCreated(Guid Id, string Sub);
public record UserLoggedIn(Guid Id);