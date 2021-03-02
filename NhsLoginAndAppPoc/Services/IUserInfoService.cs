using System.Threading.Tasks;
using NhsLoginAndAppPoc.Models;

namespace NhsLoginAndAppPoc.Services
{
    public interface IUserInfoService
    {
        Task<UserInfo> Get(string accessToken);
    }
}