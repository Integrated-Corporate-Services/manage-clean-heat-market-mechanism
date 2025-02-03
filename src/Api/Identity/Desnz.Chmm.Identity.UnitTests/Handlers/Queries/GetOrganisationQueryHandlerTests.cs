using AutoMapper;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
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

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries
{
    public class GetOrganisationQueryHandlerTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private readonly Mock<ICurrentUserService> _mockUserService;

        private GetOrganisationQueryHandler _handler;

        public GetOrganisationQueryHandlerTests()
        {
            _mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);
            _mockUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);

            _mockUserService.SetupGet(x => x.CurrentUser).Returns(GetMockUser());

            _handler = new GetOrganisationQueryHandler(
                new Mock<ILogger<BaseRequestHandler<GetOrganisationQuery, ActionResult<GetEditableOrganisationDto>>>>().Object,
                _mockMapper.Object,
                _mockOrganisationsRepository.Object,
                _mockUserService.Object) ;
        }

        [Fact]
        public async Task ShouldReturnOrganisation()
        {
            // Arrange
            var organisation = GetMockOrganisation();
            var responsibleOfficerId = organisation.ResponsibleOfficerId;
            var query = new GetOrganisationQuery()
            {
                OrganisationId = Guid.NewGuid(),
            };
            var expectedResult = new GetEditableOrganisationDto()
            {
                Users = new List<EditManufacturerUserDto>() 
                { 
                    new () 
                    {
                        Id = responsibleOfficerId,
                        IsResponsibleOfficer = true,
                        Email = "email@example.com"
                    } 
                }
            };

            _mockOrganisationsRepository.Setup(r => r.Get(o => o.Id == query.OrganisationId, null, false))
                .ReturnsAsync(organisation);
            _mockMapper.Setup(x => x.Map<GetEditableOrganisationDto>(organisation)).Returns(new GetEditableOrganisationDto()
            {
                Users = new List<EditManufacturerUserDto>()
                {
                    new()
                    {
                        Id = responsibleOfficerId,
                        Email = "email@example.com"
                    }
                }
            });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldReturnNotFound_When_OrganisationNotFound()
        {
            // Arrange
            var query = new GetOrganisationQuery()
            {
                OrganisationId = Guid.NewGuid(),
            };
            var expectedResult = Responses.NotFound("Failed to get Organisation with Id: " + query.OrganisationId);

            _mockOrganisationsRepository.Setup(r => r.Get(o => o.Id == query.OrganisationId, null, false))
                .ReturnsAsync((Organisation?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
