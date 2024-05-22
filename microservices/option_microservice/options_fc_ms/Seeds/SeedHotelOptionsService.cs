using System;
using options_fc_ms.Models;
using Microsoft.EntityFrameworkCore;
using options_fc_ms.Data;

namespace options_fc_ms.Seeds
{
	public class SeedHotelOptionsService : IHostedService
	{
        private readonly IServiceProvider _serviceProvider;

        public SeedHotelOptionsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

                await SeedDataAsync(dbContext);
            }
        }

		public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

		private async Task SeedDataAsync(ApiDbContext dbContext)
        {
            if (dbContext.HotelOptions.Any())
            {
                Console.WriteLine("Hotel Options Table is not empty!");
                return;
            }

			var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var httpClient = new HttpClient(httpClientHandler);

            var hotel1 = await httpClient.GetAsync("https://aspnet-hotel-app/api/hotels/1");
            if (!hotel1.IsSuccessStatusCode)
            {
                Console.WriteLine("the hotel for the first fixture is not found... pls check the hotel/rooom microservice...");
                return;
            }
            var babyBed = await dbContext.Options.FirstOrDefaultAsync(o => o.Name == "Lits bébé");
            var parking = await dbContext.Options.FirstOrDefaultAsync(o => o.Name == "Places de garages");

            var hotelOptions1 = new[]
                 {
                    new HotelOption
                    {
                        HotelId = 1,
                        OptionId = parking!.Id,
                        Quantity = 3
                    },
                    new HotelOption
                    {
                        HotelId = 1,
                        OptionId = babyBed!.Id,
                        Quantity = 2
                    }
                };

            await dbContext.HotelOptions.AddRangeAsync(hotelOptions1);

            await dbContext.SaveChangesAsync();

            var hotel2 = await httpClient.GetAsync("https://aspnet-hotel-app/api/hotels/2");
            if (!hotel2.IsSuccessStatusCode)
            {
                Console.WriteLine("the hotel for the second fixtures is not found... pls check the hotel/rooom microservice...");
                return;
            }

            var hotelOptions2 = new[]
                {
                    new HotelOption
                    {
                        HotelId = 2,
                        OptionId = parking!.Id,
                        Quantity = 2
                    },
                    new HotelOption
                    {
                        HotelId = 2,
                        OptionId = babyBed!.Id,
                        Quantity = 2
                    }
                };

            await dbContext.HotelOptions.AddRangeAsync(hotelOptions2);

            await dbContext.SaveChangesAsync();

        }

	}
}

