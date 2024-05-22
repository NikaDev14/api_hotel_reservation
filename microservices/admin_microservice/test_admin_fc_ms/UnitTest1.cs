using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;
using UserFcApi.Controllers;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using admin_fc_ms.Data;
using admin_fc_ms.Models;
using admin_fc_ms.Utils;

namespace TestUserFcAppi;

public class AuthenticationControllerTests
{
    private APIDbContext _dbContext;
    private TokenService _tokenService;
    private AuthenticationController _controller;

    [SetUp]
    public void Setup()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var options = new DbContextOptionsBuilder<APIDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .UseInternalServiceProvider(serviceProvider)
            .Options;

        _dbContext = new APIDbContext(options);
        _tokenService = new TokenService();
        var userManagerMock = new Mock<MockUserManager>();
        userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => _dbContext.Users.FirstOrDefault(u => u.Email == email));
        _controller = new AuthenticationController(
            userManagerMock.Object,
            _dbContext,
            _tokenService
        );
    }

    [Test]
    public async Task Register_ReturnsSuccess()
    {
        // Arrange
        var newUser = new RegisterRequest
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "password"
        };

        // Act
        var result = await _controller.Register(newUser) as CreatedAtActionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(newUser.Email, result.RouteValues["email"]);
    }

    [Test]
    public async Task Register_FailsWhenUsernameIsMissing()
    {
        var missingUsernameRequest = new RegisterRequest
        {
            Email = "testuser@example.com",
            Password = "password"
        };

        var response = await _controller.Register(missingUsernameRequest);

        Assert.IsInstanceOf<BadRequestObjectResult>(response);
    }

    [Test]
    public async Task Register_FailsWhenEmailIsMissing()
    {
        var missingUsernameRequest = new RegisterRequest
        {
            Username = "testuser",
            Password = "password"
        };

        var response = await _controller.Register(missingUsernameRequest);

        Assert.IsInstanceOf<BadRequestObjectResult>(response);
    }

    [Test]
    public async Task Register_FailsWhenPasswordIsMissing()
    {
        var missingUsernameRequest = new RegisterRequest
        {
            Username = "testuser",
            Email = "testuser@example.com"
        };

        var response = await _controller.Register(missingUsernameRequest);

        Assert.IsInstanceOf<BadRequestObjectResult>(response);
    }

    [Test]
    public async Task Register_FailsWhenPasswordIsTooShort()
    {
        var missingUsernameRequest = new RegisterRequest
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "pass"
        };

        var response = await _controller.Register(missingUsernameRequest);

        Assert.IsInstanceOf<BadRequestObjectResult>(response);
    }

}

public class MockUserManager : UserManager<IdentityUser>
{
    public MockUserManager()
        : base(new Mock<IUserStore<IdentityUser>>().Object,
               new Mock<IOptions<IdentityOptions>>().Object,
               new Mock<IPasswordHasher<IdentityUser>>().Object,
               new IUserValidator<IdentityUser>[0],
               new IPasswordValidator<IdentityUser>[0],
               new Mock<ILookupNormalizer>().Object,
               new Mock<IdentityErrorDescriber>().Object,
               new Mock<IServiceProvider>().Object,
               new Mock<ILogger<UserManager<IdentityUser>>>().Object)
    {
    }
}
