using AutoMapper;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries;

public class GetLicenceHoldersUnlinkedQueryHandlerTests
{
    private readonly Mock<ILicenceHolderRepository> _mockLicenceHolderRepository;
    private readonly Mock<ILogger<GetLicenceHoldersUnlinkedQueryHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;

    private readonly GetLicenceHoldersUnlinkedQueryHandler _handler;

    private readonly List<LicenceHolder> _licenceHolders;

    public GetLicenceHoldersUnlinkedQueryHandlerTests()
    {
        _licenceHolders = new List<LicenceHolder>
        {
            new LicenceHolder(1, "Test")
        };

        _mockLogger = new Mock<ILogger<GetLicenceHoldersUnlinkedQueryHandler>>();
        _mockLicenceHolderRepository = new Mock<ILicenceHolderRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();

        _handler = new GetLicenceHoldersUnlinkedQueryHandler(_mockLogger.Object, _mockLicenceHolderRepository.Object, _mockMapper.Object, _mockDateTimeProvider.Object);
    }

    [Fact]
    internal async Task Can_GetLicenceHolders_OkAsync_Test()
    {
        //Arrange
        var expectedLicenceHolders = new List<LicenceHolderDto>() { new LicenceHolderDto() };
        var query = new GetLicenceHoldersUnlinkedQuery();
        _mockLicenceHolderRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), false, false)).Returns(Task.FromResult(_licenceHolders));
        _mockMapper.Setup(x => x.Map<List<LicenceHolderDto>>(_licenceHolders)).Returns(expectedLicenceHolders);

        //Act
        var actionResult = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<List<LicenceHolderDto>>>(actionResult);
        Assert.Single(actionResult.Value);
    }
}
