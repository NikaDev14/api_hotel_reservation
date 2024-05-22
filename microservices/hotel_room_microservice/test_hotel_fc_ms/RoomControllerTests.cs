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
namespace HotelTests
{
    public class RoomControllerTests
	{
        [Test]
        public async Task Create_ValidRoom_ReturnsOkResult()
        {
            // Arrange
            var room = new Room
            {
                Name = "Test Room",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F,
                UserUid = "test-user-uid"
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

            var dbContextMock = new Mock<TestApiRoomsDbContext>(options) { CallBase = true };
            var roomsDbSetMock = new Mock<DbSet<Room>>();

            dbContextMock.SetupGet(x => x.TestRooms).Returns(roomsDbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var controller = new RoomController(dbContextMock.Object, authenticationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;

            // Act
            var result = await controller.Create(room);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"The room {room.Name} has been added", okResult.Value);
        }

        [Test]
        public async Task Create_InvalidToken_ReturnsUnauthorized()
        {
            var room = new Room
            {
                Name = "Test Room",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F,
                UserUid = "test-user-uid"
            };

            // Mocking the request headers
            var authorizationHeaderValue = "Bearer <votre_token>";
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add(HeaderNames.Authorization, authorizationHeaderValue);

            // Mocking the authentication service response
            var authenticationResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = null
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

            var dbContextMock = new Mock<TestApiRoomsDbContext>(options) { CallBase = true };
            var roomsDbSetMock = new Mock<DbSet<Room>>();

            dbContextMock.SetupGet(x => x.TestRooms).Returns(roomsDbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var controller = new RoomController(dbContextMock.Object, authenticationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;

            // Act
            var result = await controller.Create(room);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedObjectResult = result as UnauthorizedObjectResult;
            Assert.AreEqual("The User does not exist...", unauthorizedObjectResult.Value);
        }

        [Test]
        public async Task Create_EmptyToken_ReturnsBadRequest()
        {
            // Arrange
            var room = new Room
            {
                Name = "Test Room",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F
            };

            // Mocking the request headers
            var authorizationHeaderValue = "Bearer <votre_token>";
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add(HeaderNames.Authorization, authorizationHeaderValue);

            // Mocking the authentication service response
            var authenticationResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = null
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

            var dbContextMock = new Mock<TestApiRoomsDbContext>(options) { CallBase = true };
            var roomsDbSetMock = new Mock<DbSet<Room>>();

            dbContextMock.SetupGet(x => x.TestRooms).Returns(roomsDbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var controller = new RoomController(dbContextMock.Object, authenticationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.HttpContext.Request.Headers[HeaderNames.Authorization] = "";

            // Act
            var result = await controller.Create(room);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.AreEqual("L'utilisateur n'est pas authentifié...", badRequestObjectResult.Value);
        }

        [Test]
        public async Task Create_RoomAlreadyExists_ReturnsUnauthorized()
        {
            // Arrange
            var room = new Room
            {
                Name = "Test Room",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F
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
            using (var dbContext = new ApiDbContext(options))
            {
                // Ajouter un hôtel existant dans la base de données
                dbContext.Rooms.Add(new Room
                {
                    Name = "Test Room",
                    NameShortcut = "FR",
                    nbPersonsMax = 5,
                    Price = 12.5F
                });
                await dbContext.SaveChangesAsync();

                //var authenticationServiceMock = new Mock<IAuthenticationService>();
                // ... le reste de votre code de configuration du mock IAuthenticationService ...

                var controller = new RoomController(dbContext, authenticationServiceMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;

                // Act
                var result = await controller.Create(room);

                // Assert
                Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
                var unauthorizedObjectResult = result as UnauthorizedObjectResult;
                Assert.AreEqual("The room already exists...", unauthorizedObjectResult.Value);
            }
        }

        [Test]
        public async Task Get_NonExistingRoomId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingRoomId = 1;

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new ApiDbContext(options))
            {
                var authenticationServiceMock = new Mock<IAuthenticationService>();

                var controller = new RoomController(dbContext, authenticationServiceMock.Object);

                // Act
                var result = await controller.Get(nonExistingRoomId);

                // Assert
                Assert.IsInstanceOf<NotFoundResult>(result.Result);
            }
        }

        [Test]
        public async Task Get_ExistingRoomId_ReturnsRoom()
        {
            // Arrange
            var roomId = 1;
            var expectedRoom = new Room
            {
                Name = "Test Room",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F
            };

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new ApiDbContext(options))
            {
                dbContext.Rooms.Add(expectedRoom);
                await dbContext.SaveChangesAsync();

                var authenticationServiceMock = new Mock<IAuthenticationService>();

                var controller = new RoomController(dbContext, authenticationServiceMock.Object);

                var result = await controller.Get(roomId);

                Assert.IsInstanceOf<ActionResult<Room>>(result);
                Assert.IsInstanceOf<Room>(result.Value);
                Assert.AreEqual(expectedRoom.Id, result.Value.Id);
                Assert.AreEqual(expectedRoom.NameShortcut, result.Value.NameShortcut);
                Assert.AreEqual(expectedRoom.nbPersonsMax, result.Value.nbPersonsMax);
                Assert.AreEqual(expectedRoom.Price, result.Value.Price);
            }
        }

        [Test]
        public async Task Get_ReturnsListOfRooms()
        {
            var rooms = new List<Room>
            {
                new Room
            {
                Name = "Test Room1",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F
            },
                new Room
            {
                Name = "Test Room2",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F
            },
                new Room
            {
                Name = "Test Room3",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F
            },
                new Room
            {
                Name = "Test Room4",
                NameShortcut = "FR",
                nbPersonsMax = 5,
                Price = 12.5F
            }
            };

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new ApiDbContext(options))
            {
                dbContext.Rooms.AddRange(rooms);
                await dbContext.SaveChangesAsync();

                var authenticationServiceMock = new Mock<IAuthenticationService>();

                var controller = new RoomController(dbContext, authenticationServiceMock.Object);

                var result = await controller.Get();

                Assert.IsInstanceOf<IEnumerable<Room>>(result);

                var roomsList = result.ToList();
                Assert.AreEqual(rooms.Count, roomsList.Count);

                for (int i = 0; i < rooms.Count; i++)
                {
                    Assert.AreEqual(rooms[i].Name, roomsList[i].Name);
                }
            }
        }
    }
    public class TestApiRoomsDbContext : ApiDbContext
    {
        public TestApiRoomsDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Room> TestRooms { get; set; }
    }
}

