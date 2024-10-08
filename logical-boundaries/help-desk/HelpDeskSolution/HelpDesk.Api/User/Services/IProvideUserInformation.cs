using HelpDesk.Api.User.Models;

namespace HelpDesk.Api.User.Services;

public interface IProvideUserInformation
{
    Task<UserInfo> GetUserInfoAsync();
}