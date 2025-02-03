using AutoMapper;
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

public class GetLicenceHoldersAllQueryHandlerTests
{
    private readonly Mock<ILicenceHolderRepository> _mockLicenceHolderRepository;
    private readonly Mock<ILogger<GetLicenceHoldersAllQueryHandler>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;

    private readonly GetLicenceHoldersAllQueryHandler _handler;

    private readonly List<LicenceHolder> _licenceHolders;

    public GetLicenceHoldersAllQueryHandlerTests()
    {
        _licenceHolders = new List<LicenceHolder>
        {
            new LicenceHolder(1, "Test")
        };

        _mockLogger = new Mock<ILogger<GetLicenceHoldersAllQueryHandler>>();
        _mockLicenceHolderRepository = new Mock<ILicenceHolderRepository>();

        _mockMapper = new Mock<IMapper>();

        _handler = new GetLicenceHoldersAllQueryHandler(_mockLogger.Object, _mockLicenceHolderRepository.Object, _mockMapper.Object);
    }

    [Fact]
    internal async Task Can_GetLiceseHolders_OkAsync_Test()
    {
        //Arrange
        var expectedUsers = new List<LicenceHolderDto>() { new LicenceHolderDto() };
        var query = new GetLicenceHoldersAllQuery();
        _mockLicenceHolderRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), false, false)).Returns(Task.FromResult(_licenceHolders));
        _mockMapper.Setup(x => x.Map<List<LicenceHolderDto>>(_licenceHolders)).Returns(expectedUsers);

        //Act
        var actionResult = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<List<LicenceHolderDto>>>(actionResult);
        Assert.Single(actionResult.Value);
    }
}
