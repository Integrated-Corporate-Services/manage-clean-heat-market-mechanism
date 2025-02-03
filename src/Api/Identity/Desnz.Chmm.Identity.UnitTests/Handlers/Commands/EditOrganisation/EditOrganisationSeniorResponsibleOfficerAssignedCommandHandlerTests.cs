using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Desnz.Chmm.Identity.UnitTests.Fixtures.Handlers.Queries.GetOrganisationsQueryHandlerTestsFixture;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands.EditOrganisation
{
    public class EditOrganisationSeniorResponsibleOfficerAssignedCommandHandlerTests
    {
        private readonly Mock<ILogger<EditOrganisationSeniorResponsibleOfficerAssignedCommandHandler>> _logger;
        private readonly Mock<IOrganisationsRepository> _organisationsRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        private readonly EditOrganisationSeniorResponsibleOfficerAssignedCommandHandler _sut;

        public EditOrganisationSeniorResponsibleOfficerAssignedCommandHandlerTests()
        {
            _logger = new Mock<ILogger<EditOrganisationSeniorResponsibleOfficerAssignedCommandHandler>>();
            _organisationsRepository = new Mock<IOrganisationsRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _sut = new EditOrganisationSeniorResponsibleOfficerAssignedCommandHandler(_logger.Object, _organisationsRepository.Object);
        }

        [Fact]
        public async void ShouldReturnNotFound_When_OrganisationIsNotfound()
        {
            //Arrange
            var orgId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new EditOrganisationSeniorResponsibleOfficerAssignedCommand { OrganisationId = orgId, UserId = userId };
            var expectedResult = Responses.NotFound();

            _organisationsRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), false, true, false, true)).ReturnsAsync((Organisation?)null);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void ShouldReturn400BadRequest_When_UserIsNotPartOfOrganisation()
        {
            //Arrange
            var testOrg = GetMockOrganisation();

            var orgId = testOrg.Id;
            var userId = Guid.NewGuid();
            var command = new EditOrganisationSeniorResponsibleOfficerAssignedCommand { OrganisationId = orgId, UserId = userId };
            var expectedResult = Responses.BadRequest($"Cannot assign user {userId} as SRO for organisation {orgId} because user is not part of organisation");

            _organisationsRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), false, true, false, true)).ReturnsAsync(testOrg);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void ShouldReturn400BadRequest_When_UserIsNotActive()
        {
            //Arrange
            var testOrg = GetMockOrganisation();

            var orgId = testOrg.Id;
            var userId = testOrg.ChmmUsers.First().Id;
            var command = new EditOrganisationSeniorResponsibleOfficerAssignedCommand { OrganisationId = orgId, UserId = userId };
            var expectedResult = Responses.BadRequest($"Cannot assign user {userId} as SRO for organisation {orgId} because they are not Active");

            _organisationsRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), false, true, false, true)).ReturnsAsync(testOrg);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void ShouldReturn200Ok_When_UserIsActiveAndPartOfOrganisation()
        {
            //Arrange
            var testOrg = GetMockOrganisation();

            var orgId = testOrg.Id;
            var userId = testOrg.ChmmUsers.First().Id;
            var command = new EditOrganisationSeniorResponsibleOfficerAssignedCommand { OrganisationId = orgId, UserId = userId };
            var expectedResult = Responses.Ok();

            testOrg.ChmmUsers.First().Activate();
            _organisationsRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), false, true, false, true)).ReturnsAsync(testOrg);
            _organisationsRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

            //Act
            var result = await _sut.Handle(command, CancellationToken.None);

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
