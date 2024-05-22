using System.Net.Http.Headers;
namespace reservation_fc_ms.Services
{
    public interface IAuthenticationService
    {
        Task<HttpResponseMessage> GetAuthenticationResponseAsync(string url, string bearer);
    }

    public class AuthenticationService : IAuthenticationService
    {
        public async Task<HttpResponseMessage> GetAuthenticationResponseAsync(string url, string bearer)
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var httpClient = new HttpClient(httpClientHandler);
            //httpClient.BaseAddress = new Uri("https://admin-ms");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            return await httpClient.GetAsync(url);
        }
    }
}