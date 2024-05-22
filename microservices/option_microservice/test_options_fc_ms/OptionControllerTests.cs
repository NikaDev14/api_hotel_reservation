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
    public class OptionControllerTests
    {
        [Test]
        public async Task Create_ValidOption_ReturnsOkResult()
        {
            var option = new Option
            {
                Name = "foo",
                Price = 15.2F
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

            var dbContextMock = new Mock<TestApiOptionsDbContext>(options) { CallBase = true };
            var optionsDbSetMock = new Mock<DbSet<Option>>();

            dbContextMock.SetupGet(x => x.TestOptions).Returns(optionsDbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var controller = new OptionController(dbContextMock.Object, authenticationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;

            var result = await controller.Create(option);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual($"The option {option.Name} has been added", okResult.Value);
        }

        [Test]
        public async Task Create_InvalidToken_ReturnsUnauthorized()
        {
            var option = new Option
            {
                Name = "foo",
                Price = 15.2F
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

            var dbContextMock = new Mock<TestApiOptionsDbContext>(options) { CallBase = true };
            var optionsDbSetMock = new Mock<DbSet<Option>>();

            dbContextMock.SetupGet(x => x.TestOptions).Returns(optionsDbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var controller = new OptionController(dbContextMock.Object, authenticationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;

            var result = await controller.Create(option);

            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
            var unauthorizedObjectResult = result as UnauthorizedObjectResult;
            Assert.AreEqual("The User does not exist...", unauthorizedObjectResult.Value);
        }
        [Test]
        public async Task Create_EmptyToken_ReturnsBadRequest()
        {
            var option = new Option
            {
                Name = "foo",
                Price = 15.2F
            };

            var authorizationHeaderValue = "Bearer <votre_token>";
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add(HeaderNames.Authorization, authorizationHeaderValue);

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

            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            var dbContextMock = new Mock<TestApiOptionsDbContext>(options) { CallBase = true };
            var optionsDbSetMock = new Mock<DbSet<Option>>();

            dbContextMock.SetupGet(x => x.TestOptions).Returns(optionsDbSetMock.Object);
            dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var controller = new OptionController(dbContextMock.Object, authenticationServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.HttpContext.Request.Headers[HeaderNames.Authorization] = "";

            var result = await controller.Create(option);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.AreEqual("The user has no authenticated...", badRequestObjectResult.Value);
        }

        [Test]
        public async Task Create_OptionAlreadyExists_ReturnsUnauthorized()
        {
            var option = new Option
            {
                Name = "foo",
                Price = 15.2F
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
                dbContext.Options.Add(new Option
                {
                    Name = "foo"
                });
                await dbContext.SaveChangesAsync();
                var controller = new OptionController(dbContext, authenticationServiceMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                controller.HttpContext.Request.Headers[HeaderNames.Authorization] = authorizationHeaderValue;

                var result = await controller.Create(option);

                Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
                var unauthorizedObjectResult = result as UnauthorizedObjectResult;
                Assert.AreEqual("The option already exists...", unauthorizedObjectResult.Value);
            }
        }
        [Test]
        public async Task Get_NonExistingOptionId_ReturnsNotFound()
        {
            var nonExistingOptionId = 1;

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

                var controller = new OptionController(dbContext, authenticationServiceMock.Object);

                var result = await controller.Get(nonExistingOptionId);

                Assert.IsInstanceOf<NotFoundResult>(result.Result);
            }
        }

        [Test]
        public async Task Get_ExistingOptionId_ReturnsOption()
        {
            var optionId = 1;
            var expectedOption = new Option
            {
                Name = "Test Option"
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
                dbContext.Options.Add(expectedOption);
                await dbContext.SaveChangesAsync();

                var authenticationServiceMock = new Mock<IAuthenticationService>();
                var controller = new OptionController(dbContext, authenticationServiceMock.Object);

                var result = await controller.Get(optionId);

                Assert.IsInstanceOf<ActionResult<Option>>(result);
                Assert.IsInstanceOf<Option>(result.Value);
                Assert.AreEqual(expectedOption.Id, result.Value.Id);
            }
        }

        [Test]
        public async Task Get_ReturnsListOfOptions()
        {
            var expectedOptions = new List<Option>
            {
                new Option
            {
                Name = "Test Option1",
            },
                new Option
            {
                Name = "Test Option2",
            },
                new Option
            {
                Name = "Test Option3",
            },
                new Option
            {
                Name = "Test Option4",
            },
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
                dbContext.Options.AddRange(expectedOptions);
                await dbContext.SaveChangesAsync();

                var authenticationServiceMock = new Mock<IAuthenticationService>();

                var controller = new OptionController(dbContext, authenticationServiceMock.Object);

                var result = await controller.Get();

                Assert.IsInstanceOf<IEnumerable<Option>>(result);

                var optionsList = result.ToList();
                Assert.AreEqual(expectedOptions.Count, optionsList.Count);

                for (int i = 0; i < expectedOptions.Count; i++)
                {
                    Assert.AreEqual(expectedOptions[i].Name, optionsList[i].Name);
                }
            }
        }
    }
    public class TestApiOptionsDbContext : ApiDbContext
    {
        public TestApiOptionsDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Option> TestOptions { get; set; }
    }
}


