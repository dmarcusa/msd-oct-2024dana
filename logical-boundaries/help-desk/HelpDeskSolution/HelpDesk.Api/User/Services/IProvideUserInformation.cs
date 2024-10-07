namespace HelpDesk.Api.Services;

public interface IProvideUserInformation
{
    Task<UserInfo> GetUserInfoAsync();
}