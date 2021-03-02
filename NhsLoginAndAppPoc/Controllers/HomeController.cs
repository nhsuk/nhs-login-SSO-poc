using Microsoft.AspNetCore.Mvc;
using NhsLoginAndAppPoc.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using NhsLoginAndAppPoc.Services;

namespace NhsLoginAndAppPoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IImmunisationStatusService _immunisationStatusService;
        private readonly IConfiguration _configuration;

        public HomeController(
            IImmunisationStatusService immunisationStatusService,
            IConfiguration configuration)
        {
            _immunisationStatusService = immunisationStatusService;
            _configuration = configuration;
        }

        [Route("/")]
        [Route("the-nhs-website-immunisation-status-service")]
        public IActionResult Index() => View();

        [Authorize]
        [Route("get-immunisation-status")]
        public async Task<IActionResult> ImmunisationStatus()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            ImmunisationStatusViewModel model = await _immunisationStatusService.Get(accessToken);
            model.ManageLoginUrl = string.Format(_configuration["ManageLoginUrl"], accessToken);

            return View(model);
        }
    }
}
