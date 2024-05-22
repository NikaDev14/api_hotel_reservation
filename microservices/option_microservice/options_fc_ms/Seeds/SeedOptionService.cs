using System;
using options_fc_ms.Models;
using Microsoft.EntityFrameworkCore;
using options_fc_ms.Data;

namespace options_fc_ms.Seeds
{
    public class SeedOptionService : IHostedService
    {

        private readonly IServiceProvider _serviceProvider;

        public SeedOptionService(IServiceProvider serviceProvider)
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
            if (dbContext.Options.Any())
            {
                Console.WriteLine("Options Table is not empty!");
                return;
            }
            var options = new[]
            {
                new Option
                {
                    Name = "Places de garages",
                    Price = 25F
                },
                new Option
                {
                    Name = "Lits bébé",
                    Price = 0F
                },
                new Option
                {
                    Name = "Pack romance",
                    Price = 50F
                },
                new Option
                {
                    Name = "Petit déjeuner",
                    Price = 30F
                },
            };

            await dbContext.Options.AddRangeAsync(options);

            await dbContext.SaveChangesAsync();
        }
    }
}