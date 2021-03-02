using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using NhsLoginAndAppPoc.Models;

namespace NhsLoginAndAppPoc.Services
{
    public class UserInfoService : IUserInfoService
    {
        public async Task<UserInfo> Get(string accessToken)
        {
            using var client = new HttpClient();

            UserInfoResponse userInfo = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = "https://auth.sandpit.signin.nhs.uk/userinfo",
                Token = accessToken
            });

            var user = new UserInfo
            {
                NhsNumber = userInfo.GetClaim("nhs_number"),
                Name = userInfo.GetClaim("family_name"),
                DateOfBirth = userInfo.GetClaim("birthdate"),
                PhoneNumber = userInfo.GetClaim("phone_number"),
                Email = userInfo.GetClaim("email")
            };

            return user;
        }
    }

    public static class UserInfoExtensions
    {
        public static string GetClaim(this UserInfoResponse userInfo, string type) =>
            userInfo.Claims.SingleOrDefault(c => c.Type == type)?.Value;
    }
}