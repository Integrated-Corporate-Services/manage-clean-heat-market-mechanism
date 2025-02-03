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
    public class EditOrganisationCreditContactDetailsCommandHandlerTests
    {
        private Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private EditOrganisationCreditContactDetailsCommandHandler _handler;

        private Guid _existingOrganisation = Guid.NewGuid();
        private Mock<IOrganisation> _mockOrganisation;
        private Guid _unknownOrganisation = Guid.NewGuid();

        public EditOrganisationCreditContactDetailsCommandHandlerTests()
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

            _handler = new EditOrganisationCreditContactDetailsCommandHandler(
                new Mock<ILogger<EditOrganisationCreditContactDetailsCommandHandler>>().Object,
                _mockOrganisationsRepository.Object);
        }

        [Fact]
        public async Task ShouldReturnNotFound_When_OrganisationIsNotfound()
        {
            // Arrange
            var command = new EditOrganisationCreditContactDetailsCommand()
            {
                Name = "New Name",
                OrganisationId = _unknownOrganisation,
                TelephoneNumber = "84392084489032",
                Email = "email@example.com"
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
            var name = "New Name";
            var email = "email@example.com";
            var telephoneNumber = "43289432";

            var command = new EditOrganisationCreditContactDetailsCommand()
            {
                Email = email,
                Name = name,
                OrganisationId = _existingOrganisation,
                TelephoneNumber = telephoneNumber
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockOrganisation.Verify(x => x.UpdateCreditContactDetails(name, email, telephoneNumber), Times.Once());
            _mockOrganisationsRepository.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
