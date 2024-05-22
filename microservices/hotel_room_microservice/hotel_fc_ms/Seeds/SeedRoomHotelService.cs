using System;
using hotel_fc_ms.Models;
using Microsoft.EntityFrameworkCore;
using hotel_fc_ms.Data;

namespace hotel_fc_ms.Seeds
{
    public class SeedRoomHotelService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedRoomHotelService(IServiceProvider serviceProvider)
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
            if (dbContext.RoomHotels.Any())
            {
                Console.WriteLine("Room Hotel Table is not empty!");
                // Les données de seed existent déjà, vous pouvez choisir de les ignorer ou de les mettre à jour si nécessaire
                return;
            }

            // Recherchez l'entité avec le nom "titi" dans la table Toto
            var hotel1 = await dbContext.Hotels.FirstOrDefaultAsync(t => t.Name == "Hotel 1");
            
            if (hotel1 != null)
            {
                int hotel1Id = hotel1.Id;
                var suite = await dbContext.Rooms.FirstOrDefaultAsync(t => t.Name == "Suite");
                var juniorSuite = await dbContext.Rooms.FirstOrDefaultAsync(t => t.Name == "Junior Suite");
                var luxeRoom = await dbContext.Rooms.FirstOrDefaultAsync(t => t.Name == "Chambre de luxe");
                var standardRoom = await dbContext.Rooms.FirstOrDefaultAsync(t => t.Name == "Chambre standard");
                // L'entité avec le nom "titi" existe déjà dans la table Toto
                // Vous pouvez utiliser totoId comme nécessaire
                var roomsHotel1 = new[]
                {
                    new RoomHotel
                    {
                        RoomId = suite!.Id,
                        HotelId = hotel1Id,
                        nbItems = 1
                    },
                    new RoomHotel
                    {
                        RoomId = juniorSuite!.Id,
                        HotelId = hotel1Id,
                        nbItems = 1
                    },
                    new RoomHotel
                    {
                        RoomId = luxeRoom!.Id,
                        HotelId = hotel1Id,
                        nbItems = 1
                    },
                    new RoomHotel
                    {
                        RoomId = standardRoom!.Id,
                        HotelId = hotel1Id,
                        nbItems = 2
                    },
                };

                await dbContext.RoomHotels.AddRangeAsync(roomsHotel1);

                // Enregistrez les modifications dans la base de données
                await dbContext.SaveChangesAsync();
            }

            var hotel2 = await dbContext.Hotels.FirstOrDefaultAsync(t => t.Name == "Hotel 2");

            if (hotel2 != null)
            {
                int hotel2Id = hotel2.Id;
                var suitePresidentielle = await dbContext.Rooms.FirstOrDefaultAsync(t => t.NameShortcut == "SR");
                // L'entité avec le nom "titi" existe déjà dans la table Toto
                // Vous pouvez utiliser totoId comme nécessaire
                var roomsHotel2 = new[]
                {
                    new RoomHotel
                    {
                        RoomId = suitePresidentielle!.Id,
                        HotelId = hotel2Id,
                        nbItems = 2
                    },
                };

                await dbContext.RoomHotels.AddRangeAsync(roomsHotel2);

                // Enregistrez les modifications dans la base de données
                await dbContext.SaveChangesAsync();
            }
        }
    }
}

