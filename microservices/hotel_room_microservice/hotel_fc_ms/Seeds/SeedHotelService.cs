using System;
using hotel_fc_ms.Models;
using Microsoft.EntityFrameworkCore;
using hotel_fc_ms.Data;

namespace hotel_fc_ms.Seeds
{
	public class SeedHotelService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedHotelService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

                // Exécutez les opérations de seeding ici en utilisant dbContext
                // Par exemple :
                await SeedDataAsync(dbContext);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Pas d'action spécifique requise lors de l'arrêt de l'application
            return Task.CompletedTask;
        }
        private async Task SeedDataAsync(ApiDbContext dbContext)
        {
            // Logique de seeding de vos données initiales
            // Utilisez dbContext pour ajouter des entités à votre base de données
                // Vérifiez si des données existent déjà dans la base de données
            if (dbContext.Hotels.Any())
            {
                Console.WriteLine("Hotel Table is not empty!");
                // Les données de seed existent déjà, vous pouvez choisir de les ignorer ou de les mettre à jour si nécessaire
                return;
            }

            // Créez quelques exemples de données pour les hôtels
            var hotels = new[]
            {
                new Hotel
                {
                    Name = "Hotel 1",
                    Address = "14, Rue de l'Hôtel, Ville 1",
                    Email = "hotel1@example.com",
                    PhoneNumber = "1234567890"
                },
                new Hotel
                {
                    Name = "Hotel 2",
                    Address = "156, Rue de l'Hôtel, Ville 2",
                    Email = "hotel2@example.com",
                    PhoneNumber = "9876543210"
                },
                // Ajoutez autant d'hôtels que vous le souhaitez...
            };

            // Ajoutez les hôtels à votre contexte de base de données
            await dbContext.Hotels.AddRangeAsync(hotels);

            // Enregistrez les modifications dans la base de données
            await dbContext.SaveChangesAsync();
        }
    }
}

