using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using hotel_fc_ms.Controllers;
using hotel_fc_ms.Data;
using hotel_fc_ms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using hotel_fc_ms.Services;
using HotelTests;

namespace test_hotel_fc_ms
{
	public class RoomHotelControllerTests
	{
        [Test]
        public async Task Create_ValidRoomHotel_ReturnsOkResult()
        {
            // Arrange
            var roomHotel = new RoomHotel
            {
                RoomId = 123,
                HotelId = 321,
                nbItems = 5,
                UserUid = "test-user-uid"
            };
            var expectedRoom = new Room
            {
                Id = 123,
                Name = "Test Room",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F
            };

            var expectedHotel = new Hotel
            {
                Id = 321,
                Name = "Test Hotel",
                Address = "Test Address",
                Email = "test@example.com",
                PhoneNumber = "1234567890"
            };
            // Mocking the request headers
            var authorizationHeaderValue = "Bearer <votre_token>";
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add(HeaderNames.Authorization, authorizationHeaderValue);

            // Mocking the authentication service response
            var authenticationResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("test-user-uid")
            };
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            authenticationServiceMock
                .Setup(x => x.GetAuthenticationResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResponse);
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            // Mocking the database context
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            // Créer une instance réelle du contexte de base de données en utilisant la base de données en mémoire
            using (var dbContext = new ApiDbContext(options))
            {
                dbContext.Rooms.Add(expectedRoom);
                dbContext.Hotels.Add(expectedHotel);
                await dbContext.SaveChangesAsync();
                // ... le reste de votre code de configuration du mock IAuthenticationService ...

                var controller = new RoomHotelController(dbContext, authenticationServiceMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;
                // Act
                var result = await controller.Create(roomHotel);

                // Assert
                Assert.IsInstanceOf<OkObjectResult>(result);
                var okResult = result as OkObjectResult;
                Assert.AreEqual("The room Hotel has been added", okResult.Value);
            }
        }

        [Test]
        public async Task Create_InValidRoomId_ReturnsUnauthorized()
        {
            // Arrange
            var roomHotel = new RoomHotel
            {
                RoomId = 123,
                HotelId = 321,
                nbItems = 5,
                UserUid = "test-user-uid"
            };
            var expectedRoom = new Room
            {
                Name = "Test Room",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F
            };

            var expectedHotel = new Hotel
            {
                Id = 321,
                Name = "Test Hotel",
                Address = "Test Address",
                Email = "test@example.com",
                PhoneNumber = "1234567890"
            };
            // Mocking the request headers
            var authorizationHeaderValue = "Bearer <votre_token>";
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add(HeaderNames.Authorization, authorizationHeaderValue);

            // Mocking the authentication service response
            var authenticationResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("test-user-uid")
            };
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            authenticationServiceMock
                .Setup(x => x.GetAuthenticationResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResponse);
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            // Mocking the database context
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            // Créer une instance réelle du contexte de base de données en utilisant la base de données en mémoire
            using (var dbContext = new ApiDbContext(options))
            {
                dbContext.Rooms.Add(expectedRoom);
                dbContext.Hotels.Add(expectedHotel);
                await dbContext.SaveChangesAsync();
                // ... le reste de votre code de configuration du mock IAuthenticationService ...

                var controller = new RoomHotelController(dbContext, authenticationServiceMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;
                // Act
                var result = await controller.Create(roomHotel);

                // Assert
                Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
                var unauthorizedObjectResult = result as UnauthorizedObjectResult;
                Assert.AreEqual("The room already exists for this hotel or check room and hotel ids", unauthorizedObjectResult.Value);
            }
        }

        [Test]
        public async Task Create_InvalidNbItems_ReturnsBadRequest()
        {
            var roomHotel = new RoomHotel
            {
                RoomId = 123,
                HotelId = 321,
                UserUid = "test-user-uid"
            };

            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            using (var dbContext = new ApiDbContext(options))
            {
                var controller = new RoomHotelController(dbContext, authenticationServiceMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                var result = await controller.Create(roomHotel);
                Assert.IsInstanceOf<BadRequestObjectResult>(result);
                var badRequestObjectResult = result as BadRequestObjectResult;
                Assert.AreEqual("Missing fields...", badRequestObjectResult.Value);
            }
        }

        [Test]
        public async Task Create_InvalidHotelId_ReturnsBadRequest()
        {
            var roomHotel = new RoomHotel
            {
                RoomId = 123,
                UserUid = "test-user-uid"
            };

            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            using (var dbContext = new ApiDbContext(options))
            {
                var controller = new RoomHotelController(dbContext, authenticationServiceMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                var result = await controller.Create(roomHotel);
                Assert.IsInstanceOf<BadRequestObjectResult>(result);
                var badRequestObjectResult = result as BadRequestObjectResult;
                Assert.AreEqual("Missing fields...", badRequestObjectResult.Value);
            }
        }

        [Test]
        public async Task Create_InvaliRoomlId_ReturnsBadRequest()
        {
            var roomHotel = new RoomHotel
            {
                HotelId = 321,
                UserUid = "test-user-uid"
            };

            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            using (var dbContext = new ApiDbContext(options))
            {
                var controller = new RoomHotelController(dbContext, authenticationServiceMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                var result = await controller.Create(roomHotel);
                Assert.IsInstanceOf<BadRequestObjectResult>(result);
                var badRequestObjectResult = result as BadRequestObjectResult;
                Assert.AreEqual("Missing fields...", badRequestObjectResult.Value);
            }
        }

        [Test]
        public async Task Get_ReturnsListOfRoomHotels()
        {
            // Arrange
            var roomHotels = new List<RoomHotel>
            {
                new RoomHotel
            {
                RoomId = 1234,
                HotelId = 3210,
                nbItems = 5,
                UserUid = "test-user-uid"
            },
                new RoomHotel
            {
                RoomId = 1236,
                HotelId = 3212,
                nbItems = 5,
                UserUid = "test-user-uid"
            },
                new RoomHotel
            {
                RoomId = 1231,
                HotelId = 3213,
                nbItems = 5,
                UserUid = "test-user-uid"
            },
                new RoomHotel
            {
                RoomId = 1232,
                HotelId = 3217,
                nbItems = 5,
                UserUid = "test-user-uid"
            }
            };

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            // Mocking the database context
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            // Créer une instance réelle du contexte de base de données en utilisant la base de données en mémoire
            using (var dbContext = new ApiDbContext(options))
            {
                dbContext.RoomHotels.AddRange(roomHotels);
                await dbContext.SaveChangesAsync();

                var authenticationServiceMock = new Mock<IAuthenticationService>();
                // ... le reste de votre code de configuration du mock IAuthenticationService ...

                var controller = new RoomHotelController(dbContext, authenticationServiceMock.Object);

                // Act
                var result = await controller.Get();

                // Assert
                Assert.IsInstanceOf<IEnumerable<RoomHotel>>(result);

                var roomHotelsList = result.ToList();
                Assert.AreEqual(roomHotels.Count, roomHotelsList.Count);

                for (int i = 0; i < roomHotels.Count; i++)
                {
                    Assert.AreEqual(roomHotels[i].Id, roomHotelsList[i].Id);
                    Assert.AreEqual(roomHotels[i].HotelId, roomHotelsList[i].HotelId);
                    Assert.AreEqual(roomHotels[i].RoomId, roomHotelsList[i].RoomId);
                    Assert.AreEqual(roomHotels[i].nbItems, roomHotelsList[i].nbItems);
                }
            }
        }
    }
    public class TestApiRoomHotelDbContext : ApiDbContext
    {
        public TestApiRoomHotelDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public virtual DbSet<RoomHotel> TestRoomHotels { get; set; }
        public virtual DbSet<Room> TestRooms { get; set; }
        public virtual DbSet<Hotel> TestHotels { get; set; }
    }
}

