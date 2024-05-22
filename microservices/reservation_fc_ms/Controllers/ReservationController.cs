using System;
using System.Data;
using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using reservation_fc_ms.Data;
using reservation_fc_ms.Models;
using reservation_fc_ms.Services;
namespace reservation_fc_ms.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController : ControllerBase
	{
        private readonly ApiDbContext _context;
        private readonly IAuthenticationService _authenticationService;

        public ReservationController(ApiDbContext context, IAuthenticationService authenticationService)
        {
            _context = context;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<IEnumerable<Reservation>> Get()
        {
            var toto =  await _context.Reservations.ToListAsync();
            return toto;
        }

        [HttpPost]
        [Route("addToto")]
        public async Task<ActionResult> CreateToto(Reservation reservation)
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var bearer = headerValue.Parameter;
                var response = await _authenticationService.GetAuthenticationResponseAsync("https://user_ms_web/api/authentication/me", bearer!);
                if (!response.IsSuccessStatusCode)
                {
                    return Unauthorized("The User does not exist...");
                }
                //Console.WriteLine($"Arun test : {reservation.RoomId}");
                var responseBody = await response.Content.ReadAsStringAsync();
                reservation.UserUid = responseBody;
                reservation.CreationDate = DateTime.UtcNow;
                Console.WriteLine(reservation);
                /*
                //room case
                var httpClient2 = new HttpClient();
                httpClient2.BaseAddress = new Uri("http://hotel_web:5001/");
                var roomResponse = await httpClient2.GetAsync("Room/" + reservation.RoomId);
                if(!roomResponse.IsSuccessStatusCode)
                {
                    return NotFound("The room was not found...");
                }
                */
                //DateTime dateTime = DateTime.ParseExact(reservation.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeKind.Utc);
                
                var formattedStartDate = TimeZoneInfo.ConvertTimeToUtc(reservation.StartDate);
                reservation.StartDate = formattedStartDate;
                var formattedEndDate = TimeZoneInfo.ConvertTimeToUtc(reservation.EndDate);
                Console.WriteLine(formattedStartDate);
                reservation.EndDate = formattedEndDate;
                
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                return Ok("the reservation has been created");
            }
            return BadRequest("L'utilisateur n'est pas authentifié...");
        }

    }
}

