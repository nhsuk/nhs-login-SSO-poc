using System.Threading.Tasks;
using AutoMapper;
using NhsLoginAndAppPoc.Models;
using NhsLoginAndAppPoc.Repositories;

namespace NhsLoginAndAppPoc.Services
{
    public class ImmunisationStatusService : IImmunisationStatusService
    {
        private readonly IMapper _mapper;
        private readonly IUserInfoService _userInfoService;
        private readonly IImmunisationStatusRepository _immunisationStatusRepository;

        public ImmunisationStatusService(
            IMapper mapper,
            IUserInfoService userInfoService,
            IImmunisationStatusRepository immunisationStatusRepository)
        {
            _mapper = mapper;
            _userInfoService = userInfoService;
            _immunisationStatusRepository = immunisationStatusRepository;
        }

        public async Task<ImmunisationStatusViewModel> Get(string accessToken)
        {
            UserInfo userInfo = await _userInfoService.Get(accessToken);
            var model = _mapper.Map<ImmunisationStatusViewModel>(userInfo);

            if (string.IsNullOrEmpty(userInfo.NhsNumber))
                return model;

            ImmunisationStatusResponse immunisationStatus =
                await _immunisationStatusRepository.Get(userInfo.NhsNumber);

            return _mapper.Map(immunisationStatus, model);
        }
    }
}