using System.Threading.Tasks;
using NhsLoginAndAppPoc.Models;

namespace NhsLoginAndAppPoc.Services
{
    public interface IImmunisationStatusService
    {
        Task<ImmunisationStatusViewModel> Get(string accessToken);
    }
}