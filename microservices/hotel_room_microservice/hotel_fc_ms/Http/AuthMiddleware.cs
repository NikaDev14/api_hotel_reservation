using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;


namespace hotel_fc_ms.Http
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string token = context.Request.Headers[HeaderNames.Authorization]!;

        if (!string.IsNullOrEmpty(token))
        {
            // Appel à la route "/authenticate" du microservice "Toto"
            using var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", token);
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("http://web:5000/My/test");
            if (response.IsSuccessStatusCode)
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = (int)response.StatusCode;
                   // context.Response.Body = "toto";
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
}