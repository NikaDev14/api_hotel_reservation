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
    [TestFixture]
    public class HotelControllerTests
    {
        [Test]
        public async Task Create_ValidHotel_ReturnsOkResult()
        {
            // Arrange
            var hotel = new Hotel
            {
                Name = "Test Hotel",
                Address = "Test Address",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
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

            var dbContextMock = new Mock<TestApiDbContext>(options) { CallBase = true };
            var hotelsDbSetMock = new Mock<DbSet<Hotel>>();

            dbContextMock.SetupGet(x => x.TestHotels).Returns(hotelsDbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var controller = new HotelController(dbContextMock.Object, authenticationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;

            // Act
            var result = await controller.Create(hotel);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"The hotel {hotel.Name} has been added", okResult.Value);
        }

        [Test]
        public async Task Create_InvaliToken_ReturnsUnauthorized()
        {
            // Arrange
            var hotel = new Hotel
            {
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

            var dbContextMock = new Mock<TestApiDbContext>(options) { CallBase = true };
            var hotelsDbSetMock = new Mock<DbSet<Hotel>>();

            dbContextMock.SetupGet(x => x.TestHotels).Returns(hotelsDbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var controller = new HotelController(dbContextMock.Object, authenticationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;
            
            // Act
            var result = await controller.Create(hotel);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedObjectResult = result as UnauthorizedObjectResult;
            Assert.AreEqual("The User does not exist...", unauthorizedObjectResult.Value);
        }

        [Test]
        public async Task Create_EmptyToken_ReturnsBadRequest()
        {
            // Arrange
            var hotel = new Hotel
            {
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

            var dbContextMock = new Mock<TestApiDbContext>(options) { CallBase = true };
            var hotelsDbSetMock = new Mock<DbSet<Hotel>>();

            dbContextMock.SetupGet(x => x.TestHotels).Returns(hotelsDbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var controller = new HotelController(dbContextMock.Object, authenticationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.HttpContext.Request.Headers[HeaderNames.Authorization] = "";

            // Act
            var result = await controller.Create(hotel);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.AreEqual("L'utilisateur n'est pas authentifié...", badRequestObjectResult.Value);
        }

        [Test]
        public async Task Create_HotelAlreadyExists_ReturnsUnauthorized()
        {
            // Arrange
            var hotel = new Hotel
            {
                Name = "Test Hotel",
                Address = "Test Address",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
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
            using (var dbContext = new ApiDbContext(options))
            {
                // Ajouter un hôtel existant dans la base de données
                dbContext.Hotels.Add(new Hotel
                {
                    Name = "Test Hotel",
                    Address = "Test Address",
                    Email = "test@example.com",
                    PhoneNumber = "1234567890",
                    UserUid = "test-user-uid"
                });
                await dbContext.SaveChangesAsync();

                //var authenticationServiceMock = new Mock<IAuthenticationService>();
                // ... le reste de votre code de configuration du mock IAuthenticationService ...

                var controller = new HotelController(dbContext, authenticationServiceMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;

                // Act
                var result = await controller.Create(hotel);

                // Assert
                Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
                var unauthorizedObjectResult = result as UnauthorizedObjectResult;
                Assert.AreEqual("The hotel already exists...", unauthorizedObjectResult.Value);
            }
        }

        [Test]
        public async Task Get_ReturnsListOfHotels()
        {
            // Arrange
            var hotels = new List<Hotel>
    {
        new Hotel {                     Name = "Test Hotel first",
                    Address = "Test Address",
                    Email = "test@example.com",
                    PhoneNumber = "1234567890",
                    UserUid = "test-user-uid" },
        new Hotel {                     Name = "Test Hote secondl",
                    Address = "Test Address",
                    Email = "test@example.com",
                    PhoneNumber = "1234567890",
                    UserUid = "test-user-uid" },
        new Hotel {                     Name = "Test Hotel three",
                    Address = "Test Address",
                    Email = "test@example.com",
                    PhoneNumber = "1234567890",
                    UserUid = "test-user-uid" }
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
                dbContext.Hotels.AddRange(hotels);
                await dbContext.SaveChangesAsync();

                var authenticationServiceMock = new Mock<IAuthenticationService>();
                // ... le reste de votre code de configuration du mock IAuthenticationService ...

                var controller = new HotelController(dbContext, authenticationServiceMock.Object);

                // Act
                var result = await controller.Get();

                // Assert
                Assert.IsInstanceOf<IEnumerable<Hotel>>(result);

                var hotelList = result.ToList();
                Assert.AreEqual(hotels.Count, hotelList.Count);

                for (int i = 0; i < hotels.Count; i++)
                {
                    Assert.AreEqual(hotels[i].Name, hotelList[i].Name);
                    Assert.AreEqual(hotels[i].Address, hotelList[i].Address);
                }
            }
        }

        [Test]
        public async Task Get_ExistingHotelId_ReturnsHotel()
        {
            // Arrange
            var hotelId = 1;
            var expectedHotel = new Hotel { Id = hotelId, Name = "Test Hotel", Address = "Test Address", Email = "sample@example.com", PhoneNumber = "9876543210" };

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
                dbContext.Hotels.Add(expectedHotel);
                await dbContext.SaveChangesAsync();

                var authenticationServiceMock = new Mock<IAuthenticationService>();
                // ... le reste de votre code de configuration du mock IAuthenticationService ...

                var controller = new HotelController(dbContext, authenticationServiceMock.Object);

                // Act
                var result = await controller.Get(hotelId);

                // Assert
                Assert.IsInstanceOf<ActionResult<Hotel>>(result);
                Assert.IsInstanceOf<Hotel>(result.Value);
                Assert.AreEqual(expectedHotel.Id, result.Value.Id);
                Assert.AreEqual(expectedHotel.Name, result.Value.Name);
                Assert.AreEqual(expectedHotel.Address, result.Value.Address);
                Assert.AreEqual(expectedHotel.PhoneNumber, result.Value.PhoneNumber);
            }
        }

        [Test]
        public async Task Get_NonExistingHotelId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingHotelId = 1;

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
                var authenticationServiceMock = new Mock<IAuthenticationService>();
                // ... le reste de votre code de configuration du mock IAuthenticationService ...

                var controller = new HotelController(dbContext, authenticationServiceMock.Object);

                // Act
                var result = await controller.Get(nonExistingHotelId);

                // Assert
                Assert.IsInstanceOf<NotFoundResult>(result.Result);
            }
        }

    }

    public class TestApiDbContext : ApiDbContext
    {
        public TestApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Hotel> TestHotels { get; set; }
    }
}
