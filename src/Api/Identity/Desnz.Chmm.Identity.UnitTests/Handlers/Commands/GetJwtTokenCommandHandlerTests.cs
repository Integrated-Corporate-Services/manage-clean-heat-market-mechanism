using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Configuration;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.Identity.Api.Infrastructure;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands;

public class GetJwtTokenCommandHandlerTests
{
    private readonly Mock<ITokenService> _tokenService;
    private readonly Mock<IUsersRepository> _usersRepository;
    private readonly Mock<IOneLoginService> _oneLoginService;
    private readonly Mock<IOptions<EnvironmentConfig>> _environmentConfig;
    private readonly Mock<IOptions<ApiKeyPolicyConfig>> _mcsSynchronisationPolicyConfig;
    private readonly Mock<ILogger<GetJwtTokenCommandHandler>> _logger;

    private readonly GetJwtTokenCommandHandler _handler;

    public GetJwtTokenCommandHandlerTests()
    {
        _tokenService = new Mock<ITokenService>();
        _usersRepository = new Mock<IUsersRepository>();
        _oneLoginService = new Mock<IOneLoginService>();
        _environmentConfig = new Mock<IOptions<EnvironmentConfig>>();
        _mcsSynchronisationPolicyConfig = new Mock<IOptions<ApiKeyPolicyConfig>>();
        _logger = new Mock<ILogger<GetJwtTokenCommandHandler>>();

        _mcsSynchronisationPolicyConfig.Setup(x => x.Value).Returns(new ApiKeyPolicyConfig
        {
            ApiKey = "apiKey",
            HeaderName = "headerName"
        });

        _handler = new GetJwtTokenCommandHandler(
            _tokenService.Object,
            _usersRepository.Object,
            _oneLoginService.Object,
            _environmentConfig.Object,
            _mcsSynchronisationPolicyConfig.Object,
            _logger.Object);
    }

    [Fact]
    public async Task GenerateJwtToken_For_ApiUser()
    {
        // Arrange

        // Act
        var response = await _handler.Handle(new GetJwtTokenCommand("", "", "", apiKey: "apiKey"), CancellationToken.None);

        // Assert
        _tokenService.Verify(x => x.GenerateJwtToken("", null, "apiKey"), Times.Once);
    }

    [Fact]
    public async Task GenerateJwtToken_For_DevBypass()
    {
        // Arrange
        var testUser = new ChmmUser("Test User", "test@example.com", new());
        _environmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig
        {
            EnvironmentName = "dev"
        });
        _usersRepository.Setup(x => x.GetByEmail("test@example.com", true, true))
            .Returns(Task.FromResult(testUser));

        // Act
        var response = await _handler.Handle(new GetJwtTokenCommand("", "", email: "test@example.com"), CancellationToken.None);

        // Assert
        _tokenService.Verify(x => x.GenerateJwtToken("test@example.com", testUser, null), Times.Once);
    }

    [Fact]
    public async Task GenerateJwtToken_For_Subject()
    {
        // Arrange
        var testUser = new ChmmUser("Test User", "test@example.com", new());
        _environmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig
        {
            EnvironmentName = "unit tests"
        });
        _usersRepository.Setup(x => x.GetBySubject("subject", true, true))
            .Returns(Task.FromResult(testUser));
        _tokenService.Setup(x => x.ValidateTokenAsync("", CancellationToken.None))
            .Returns(Task.FromResult(true));
        _oneLoginService.Setup(x => x.GetUserInfo(""))
            .Returns(Task.FromResult<HttpObjectResponse<OneLoginUserInfoDto>>(
                new CustomHttpObjectResponse<OneLoginUserInfoDto>(
                    new OneLoginUserInfoDto("subject", DateTime.UtcNow.ToString())
                    {
                        Email = "test@example.com"
                    })));

        // Act
        var response = await _handler.Handle(new GetJwtTokenCommand("", "", email: "test@example.com"), CancellationToken.None);

        // Assert
        _usersRepository.Verify(x => x.GetBySubject("subject", true, true), Times.Once);
        _usersRepository.Verify(x => x.GetByEmail("test@example.com", true, true), Times.Never);
        _tokenService.Verify(x => x.GenerateJwtToken("test@example.com", testUser, null), Times.Once);
    }

    [Fact]
    public async Task GenerateJwtToken_For_Email()
    {
        // Arrange
        var testUser = new ChmmUser("Test User", "test@example.com", new());
        _environmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig
        {
            EnvironmentName = "unit tests"
        });
        _usersRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(default)).Returns(Task.FromResult(0));
        _usersRepository.Setup(x => x.GetByEmail("test@example.com", true, true))
            .Returns(Task.FromResult(testUser));
        _tokenService.Setup(x => x.ValidateTokenAsync("", CancellationToken.None))
            .Returns(Task.FromResult(true));
        _oneLoginService.Setup(x => x.GetUserInfo(""))
            .Returns(Task.FromResult<HttpObjectResponse<OneLoginUserInfoDto>>(
                new CustomHttpObjectResponse<OneLoginUserInfoDto>(
                    new OneLoginUserInfoDto("subject", DateTime.UtcNow.ToString())
                    {
                        Email = "test@example.com"
                    })));

        // Act
        var response = await _handler.Handle(new GetJwtTokenCommand("", "", email: "test@example.com"), CancellationToken.None);

        // Assert
        _usersRepository.Verify(x => x.GetBySubject("subject", true, true), Times.Once);
        _usersRepository.Verify(x => x.GetByEmail("test@example.com", true, true), Times.Once);
        _usersRepository.Verify(x => x.UnitOfWork.SaveChangesAsync(default), Times.Once);
        _tokenService.Verify(x => x.GenerateJwtToken("test@example.com", testUser, null), Times.Once);
    }

    [Fact]
    public async Task Fail_For_Malicious_Email_Impersonation()
    {
        // Arrange
        var testUser = new ChmmUser("Test User", "test@example.com", new());
        testUser.SetSubject("real user subject");
        _environmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig
        {
            EnvironmentName = "unit tests"
        });
        _usersRepository.Setup(x => x.GetByEmail("test@example.com", true, true))
            .Returns(Task.FromResult(testUser));
        _tokenService.Setup(x => x.ValidateTokenAsync("", CancellationToken.None))
            .Returns(Task.FromResult(true));
        _oneLoginService.Setup(x => x.GetUserInfo(""))
            .Returns(Task.FromResult<HttpObjectResponse<OneLoginUserInfoDto>>(
                new CustomHttpObjectResponse<OneLoginUserInfoDto>(
                    new OneLoginUserInfoDto("malicious user subject", DateTime.UtcNow.ToString())
                    {
                        Email = "test@example.com"
                    })));

        // Act
        var response = await _handler.Handle(new GetJwtTokenCommand("", "", email: "test@example.com"), CancellationToken.None);

        // Assert
        _usersRepository.Verify(x => x.GetBySubject("malicious user subject", true, true), Times.Once);
        _usersRepository.Verify(x => x.GetByEmail("test@example.com", true, true), Times.Once);
        _tokenService.Verify(x => x.GenerateJwtToken("test@example.com", testUser, null), Times.Never);
        _tokenService.Verify(x => x.GenerateJwtToken("test@example.com", null, null), Times.Once);
    }
}
