using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Desnz.Chmm.Identity.UnitTests.Commands.EditOrganisation
{
    public class EditOrganisationSchemeParticipationCommandHandlerTests
    {
        private Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private EditOrganisationSchemeParticipationCommandHandler _handler;

        private Guid _existingOrganisation = Guid.NewGuid();
        private Mock<IOrganisation> _mockOrganisation;
        private Guid _unknownOrganisation = Guid.NewGuid();

        public EditOrganisationSchemeParticipationCommandHandlerTests()
        {
            _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);
            _mockOrganisation = new Mock<IOrganisation>();
            _mockOrganisationsRepository
                .Setup(r => r.GetByIdForUpdate(_existingOrganisation, It.IsAny<bool>(), It.IsAny<bool>(), true))
                .ReturnsAsync(_mockOrganisation.Object);
            _mockOrganisationsRepository
                .Setup(r => r.GetByIdForUpdate(_unknownOrganisation, It.IsAny<bool>(), It.IsAny<bool>(), true))
                .ReturnsAsync((Organisation?)null);
            _mockOrganisationsRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _handler = new EditOrganisationSchemeParticipationCommandHandler(
                new Mock<ILogger<EditOrganisationSchemeParticipationCommandHandler>>().Object,
                _mockOrganisationsRepository.Object);
        }

        [Fact]
        public async Task ShouldReturnNotFound_When_OrganisationIsNotfound()
        {
            // Arrange
            var command = new EditOrganisationSchemeParticipationCommand()
            {
                IsNonSchemeParticipant = true,
                OrganisationId = _unknownOrganisation
            };
            var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {_unknownOrganisation}");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldUpdateOrganisation_When_OrganisationIsFound()
        {
            // Arrange
            var isNonSchemeParticipant = false;

            var command = new EditOrganisationSchemeParticipationCommand()
            {
                IsNonSchemeParticipant = isNonSchemeParticipant,
                OrganisationId = _existingOrganisation
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockOrganisation.Verify(x => x.UpdateSchemeParticipation(isNonSchemeParticipant), Times.Once());
            _mockOrganisationsRepository.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
