using System.Net.Http.Headers;
namespace reservation_fc_ms.Services
{
    public interface IRoomService
    {
        Task<HttpResponseMessage> GetRoomByIdResponseAsync(string url);
    }

    public class RoomService : IRoomService
    {
        public async Task<HttpResponseMessage> GetRoomByIdResponseAsync(string url)
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