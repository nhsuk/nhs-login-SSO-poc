using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NhsLoginAndAppPoc.Models;

namespace NhsLoginAndAppPoc.Repositories
{
    public class ImmunisationStatusRepository : IImmunisationStatusRepository
    {
        private readonly IConfiguration _configuration;

        public ImmunisationStatusRepository(IConfiguration configuration) => _configuration = configuration;

        public async Task<ImmunisationStatusResponse> Get(string nhsNumber)
        {
            using var client = new HttpClient();

            var uri = new Uri(QueryHelpers.AddQueryString(_configuration["Nims:ApiUrl"], nameof(nhsNumber), nhsNumber));
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Add("Subscription-Key", _configuration["Nims:ApiKey"]);

            client.DefaultRequestHeaders.Clear();
            HttpResponseMessage res = await client.SendAsync(req, CancellationToken.None);
            string json = await res.Content.ReadAsStringAsync();
            var immunisationStatus = JsonConvert.DeserializeObject<ImmunisationStatusResponse>(json);

            return immunisationStatus;
        }
    }
}