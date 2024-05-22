using System;
using hotel_fc_ms.Models;
using Microsoft.EntityFrameworkCore;
using hotel_fc_ms.Data;

namespace hotel_fc_ms.Seeds
{
	public class SeedRoomService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedRoomService(IServiceProvider serviceProvider)
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
            if (dbContext.Rooms.Any())
            {
                Console.WriteLine("Room Table is not empty!");
                // Les données de seed existent déjà, vous pouvez choisir de les ignorer ou de les mettre à jour si nécessaire
                return;
            }

            // Créez quelques exemples de données pour les hôtels
            var rooms = new[]
            {
                new Room
                {
                    Name = "Suite présidentielle",
                    NameShortcut = "SR",
                    nbPersonsMax = 5,
                    Price = 1000.0F
                },
                new Room
                {
                    Name = "Suite",
                    NameShortcut = "S",
                    nbPersonsMax = 3,
                    Price = 720.0F
                },
                new Room
                {
                    Name = "Junior Suite",
                    NameShortcut = "JS",
                    nbPersonsMax = 2,
                    Price = 500.0F
                },
                new Room
                {
                    Name = "Chambre de luxe",
                    NameShortcut = "CD",
                    nbPersonsMax = 2,
                    Price = 300.0F
                },
                new Room
                {
                    Name = "Chambre standard",
                    NameShortcut = "CS",
                    nbPersonsMax = 2,
                    Price = 150.0F
                }
            };

            // Ajoutez les hôtels à votre contexte de base de données
            await dbContext.Rooms.AddRangeAsync(rooms);

            // Enregistrez les modifications dans la base de données
            await dbContext.SaveChangesAsync();
        }
    }
}

