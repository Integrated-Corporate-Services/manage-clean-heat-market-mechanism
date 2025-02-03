using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.AutoMapper;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Desnz.Chmm.Identity.UnitTests.Fixtures.Handlers.Queries.GetOrganisationQueryHandlerTestsFixture;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries;

public class GetManufacturerUserQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
    private readonly Mock<IUsersRepository> _mockUsersRepository;
    private readonly Mock<ICurrentUserService> _mockUserService;

    private readonly GetManufacturerUserQueryHandler _handler;

    public GetManufacturerUserQueryHandlerTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.Internal().MethodMappingEnabled = false;
            cfg.AddProfile<IdentityAutoMapperProfile>();
        }).CreateMapper();

        _mockUsersRepository = new Mock<IUsersRepository>();
        _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);
        _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);

        _mockUserService.SetupGet(x => x.CurrentUser).Returns(GetMockUser());

        _handler = new GetManufacturerUserQueryHandler(
            new Mock<ILogger<BaseRequestHandler<GetManufacturerUserQuery, ActionResult<ViewManufacturerUserDto>>>>().Object,
            _mockUserService.Object,
            _mockOrganisationsRepository.Object,
            _mockUsersRepository.Object,
            _mapper);
    }

    [Fact]
    public async Task ShouldReturnNotFound_When_OrganisationNotFound()
    {
        // Arrange
        var query = new GetManufacturerUserQuery(Guid.NewGuid(), Guid.NewGuid());

        var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {query.OrganisationId}");

        _mockOrganisationsRepository.Setup(r => r.Get(o => o.Id == query.OrganisationId, null, false))
            .ReturnsAsync((Organisation?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnNotFound_When_UserNotFound()
    {
        // Arrange
        var organisation = GetMockOrganisation();
        var query = new GetManufacturerUserQuery(Guid.NewGuid(), Guid.NewGuid());

        var expectedResult = Responses.NotFound($"Failed to get User with Id: {query.UserId}");

        _mockOrganisationsRepository.Setup(r => r.Get(o => o.Id == query.OrganisationId, null, false))
            .ReturnsAsync(organisation);
        _mockUsersRepository.Setup(r => r.Get(o => o.Id == query.UserId && o.OrganisationId == query.OrganisationId, true, false))
            .ReturnsAsync((ChmmUser?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ShouldReturnOk()
    {
        // Arrange
        var organisation = GetMockOrganisation();
        var user = new ChmmUser("Test", "test@example.com", new());
        var query = new GetManufacturerUserQuery(Guid.NewGuid(), Guid.NewGuid());

        var expectedResult = _mapper.Map<ViewManufacturerUserDto>(user);

        _mockOrganisationsRepository.Setup(r => r.Get(o => o.Id == query.OrganisationId, null, false))
            .ReturnsAsync(organisation);
        _mockUsersRepository.Setup(r => r.Get(o => o.Id == query.UserId && o.OrganisationId == query.OrganisationId, true, false))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().BeEquivalentTo(expectedResult);
    }
}
