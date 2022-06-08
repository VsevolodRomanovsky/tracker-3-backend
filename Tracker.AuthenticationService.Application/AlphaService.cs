using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Tracker.AuthenticationService.Auth0.ConfigObjects;
using Tracker.AuthenticationService.Dto;

namespace Tracker.AuthenticationService.Application
{
    public class AlphaService: IAlphaService
    {
        private readonly HttpClient _httpClient;
        private readonly Auth0Config _config;

        public AlphaService(IHttpClientFactory httpClientFactory, IOptions<Auth0Config> config)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(AlphaService));
            _config = config.Value;
        }

        public async Task<JObject> GetUserDataFromAlpha(LoginDto loginDto)
        {
            var data = JsonConvert.SerializeObject(new
            {
                username = loginDto.Login,
                password = loginDto.Password
            });

            var stringContent = new StringContent(data, UnicodeEncoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_config.AlphaUrl, stringContent);

            var content = await response.Content.ReadAsStringAsync();
            var jsonResult = JObject.Parse(content);
            return jsonResult;
        }
    }
}
