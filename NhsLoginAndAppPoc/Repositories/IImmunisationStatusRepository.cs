using System.Threading.Tasks;
using NhsLoginAndAppPoc.Models;

namespace NhsLoginAndAppPoc.Repositories
{
    public interface IImmunisationStatusRepository
    {
        Task<ImmunisationStatusResponse> Get(string nhsNumber);
    }
}