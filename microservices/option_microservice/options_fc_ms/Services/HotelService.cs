using System.Net.Http.Headers;
namespace options_fc_ms.Services
{
    public interface IHotelService
    {
        Task<HttpResponseMessage> GetHotelByIdResponseAsync(string url);
    }

    public class HotelService : IHotelService
    {
        public async Task<HttpResponseMessage> GetHotelByIdResponseAsync(string url)
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var httpClient = new HttpClient(httpClientHandler);


            return await httpClient.GetAsync(url);
        }
    }
}