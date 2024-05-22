using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using options_fc_ms.Controllers;
using options_fc_ms.Data;
using options_fc_ms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using options_fc_ms.Services;
namespace test_options_fc_ms
{
	public class HotelOptionControllerTests
	{
        [Test]
        public async Task Create_ValidRoomHotel_ReturnsOkResult()
        {   
            var expectedOption = new Option
            {
                Id = 123,
                Name = "Test Option"
            };

            var authorizationHeaderValue = "Bearer <votre_token>";
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add(HeaderNames.Authorization, authorizationHeaderValue);

            var authenticationResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("test-user-uid")
            };
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var mockHotelService = new Mock<IHotelService>();
            mockHotelService.Setup(s => s.GetHotelByIdResponseAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });
            authenticationServiceMock
                .Setup(x => x.GetAuthenticationResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResponse);
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new ApiDbContext(options))
            {
                dbContext.Options.Add(expectedOption);
                await dbContext.SaveChangesAsync();

                var controller = new HotelOptionController(dbContext, authenticationServiceMock.Object, mockHotelService.Object) 
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;
                var result = await controller.Create(new HotelOption
                {
                    OptionId = expectedOption.Id,
                    HotelId = 1,
                    Quantity = 1
                });
                Assert.IsInstanceOf<OkObjectResult>(result);
            }
        }

        [Test]
        public async Task Create_InvalidToken_ReturnsUnauthrizedResult()
        {
            var expectedOption = new Option
            {
                Id = 123,
                Name = "Test Option"
            };

            var authorizationHeaderValue = "Bearer <votre_token>";
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add(HeaderNames.Authorization, authorizationHeaderValue);

            var authenticationResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = null
            };
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var mockHotelService = new Mock<IHotelService>();
            mockHotelService.Setup(s => s.GetHotelByIdResponseAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });
            authenticationServiceMock
                .Setup(x => x.GetAuthenticationResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResponse);
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new ApiDbContext(options))
            {
                dbContext.Options.Add(expectedOption);
                await dbContext.SaveChangesAsync();

                var controller = new HotelOptionController(dbContext, authenticationServiceMock.Object, mockHotelService.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;
                var result = await controller.Create(new HotelOption
                {
                    OptionId = expectedOption.Id,
                    HotelId = 1,
                    Quantity = 1
                });
                Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
                var unauthorizedObjectResult = result as UnauthorizedObjectResult;
                Assert.AreEqual("The User does not exist...", unauthorizedObjectResult.Value);
            }
        }
        [Test]
        public async Task Update_ValidHotelOption_ReturnsOk()
        {
            // Arrange
            var existingHotelOption = new HotelOption
            {
                Id = 123,
                OptionId = 456,
                HotelId = 789,
                Quantity = 1
            };

            var updatedHotelOption = new HotelOption
            {
                OptionId = existingHotelOption.OptionId,
                HotelId = existingHotelOption.HotelId,
                Quantity = 2
            };

            var authorizationHeaderValue = "Bearer <votre_token>";
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add(HeaderNames.Authorization, authorizationHeaderValue);

            var authenticationResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("test-user-uid")
            };

            var mockHotelService = new Mock<IHotelService>();
            var mockAuthenticationService = new Mock<IAuthenticationService>();
            mockAuthenticationService
                .Setup(x => x.GetAuthenticationResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResponse);

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new ApiDbContext(options))
            {
                dbContext.HotelOptions.Add(existingHotelOption);
                await dbContext.SaveChangesAsync();

                var controller = new HotelOptionController(dbContext, mockAuthenticationService.Object, mockHotelService.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;
                var result = await controller.Update(existingHotelOption.Id, updatedHotelOption);

                Assert.IsInstanceOf<OkObjectResult>(result);
                Assert.AreEqual(updatedHotelOption.Quantity, existingHotelOption.Quantity);
            }
        }

        [Test]
        public async Task Update_InvalidHotelOptionNotFoundReturnsUnauthorized()
        {
            var existingHotelOption = new HotelOption
            {
                Id = 123,
                OptionId = 456,
                HotelId = 789,
                Quantity = 1
            };

            var updatedHotelOption = new HotelOption
            {
                Id = 1234,
                OptionId = existingHotelOption.OptionId,
                HotelId = 789,
                Quantity = 2
            };

            var authorizationHeaderValue = "Bearer <votre_token>";
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add(HeaderNames.Authorization, authorizationHeaderValue);

            var authenticationResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("test-user-uid")
            };

            var mockHotelService = new Mock<IHotelService>();
            var mockAuthenticationService = new Mock<IAuthenticationService>();
            mockAuthenticationService
                .Setup(x => x.GetAuthenticationResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResponse);

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new ApiDbContext(options))
            {
                dbContext.HotelOptions.Add(existingHotelOption);
                await dbContext.SaveChangesAsync();

                var controller = new HotelOptionController(dbContext, mockAuthenticationService.Object, mockHotelService.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;
                var result = await controller.Update(updatedHotelOption.Id, updatedHotelOption);

                Assert.IsInstanceOf<NotFoundObjectResult>(result);
            }
        }
        [Test]
        public async Task Update_MissingQuantityReturnsBadRequest()
        {
            var existingHotelOption = new HotelOption
            {
                Id = 123,
                OptionId = 456,
                HotelId = 789,
                Quantity = 1
            };

            var updatedHotelOption = new HotelOption
            {
                Id = 1234,
                OptionId = existingHotelOption.OptionId,
                HotelId = 789
            };

            var authorizationHeaderValue = "Bearer <votre_token>";
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add(HeaderNames.Authorization, authorizationHeaderValue);

            var authenticationResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("test-user-uid")
            };

            var mockHotelService = new Mock<IHotelService>();
            var mockAuthenticationService = new Mock<IAuthenticationService>();
            mockAuthenticationService
                .Setup(x => x.GetAuthenticationResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(authenticationResponse);

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new ApiDbContext(options))
            {
                dbContext.HotelOptions.Add(existingHotelOption);
                await dbContext.SaveChangesAsync();

                var controller = new HotelOptionController(dbContext, mockAuthenticationService.Object, mockHotelService.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;
                var result = await controller.Update(existingHotelOption.Id, updatedHotelOption);

                Assert.IsInstanceOf<BadRequestObjectResult>(result);
            }
        }
    }

    public class TestApiRoomHotelDbContext : ApiDbContext
    {
        public TestApiRoomHotelDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
    }
}

