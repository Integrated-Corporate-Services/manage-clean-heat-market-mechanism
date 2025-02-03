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
    public class EditOrganisationSeniorResponsibleOfficerCommandHandlerTests
    {
        private Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private EditOrganisationSeniorResponsibleOfficerCommandHandler _handler;

        private Guid _existingOrganisation = Guid.NewGuid();
        private Mock<IOrganisation> _mockOrganisation;
        private Guid _unknownOrganisation = Guid.NewGuid();

        public EditOrganisationSeniorResponsibleOfficerCommandHandlerTests()
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

            _handler = new EditOrganisationSeniorResponsibleOfficerCommandHandler(
                new Mock<ILogger<EditOrganisationSeniorResponsibleOfficerCommandHandler>>().Object,
                _mockOrganisationsRepository.Object);
        }

        [Fact]
        public async Task ShouldReturnNotFound_When_OrganisationIsNotfound()
        {
            // Arrange
            var command = new EditOrganisationSeniorResponsibleOfficerCommand()
            {
                OrganisationId = _unknownOrganisation,
                IsApplicantSeniorResponsibleOfficer = true,
            };
            var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {_unknownOrganisation}");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldUpdateOrganisation_When_OrganisationIsFound_And_ApplicantIsSeniorResponsibleOfficer()
        {
            // Arrange
            var isApplicantSeniorResponsibleOfficer = true;

            var command = new EditOrganisationSeniorResponsibleOfficerCommand()
            {
                OrganisationId = _existingOrganisation,
                IsApplicantSeniorResponsibleOfficer = isApplicantSeniorResponsibleOfficer
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockOrganisation.Verify(x => x.RemoveResponsibleOfficerIfExists(), Times.Once());
            _mockOrganisationsRepository.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_OrganisationIsFound_DetailsGiven_And_ApplicantIsSeniorResponsibleOfficer()
        {
            // Arrange
            var name = "New Name";
            var jobTitle = "New Job Title";
            var telephoneNumber = "43289432";
            var isApplicantSeniorResponsibleOfficer = true;

            var command = new EditOrganisationSeniorResponsibleOfficerCommand()
            {
                JobTitle = jobTitle,
                Name = name,
                OrganisationId = _existingOrganisation,
                TelephoneNumber = telephoneNumber,
                IsApplicantSeniorResponsibleOfficer = isApplicantSeniorResponsibleOfficer
            };
            
            var expectedResult = Responses.BadRequest($"Cannot update Organisation with Id: {_existingOrganisation}, speficying details and Applicant is Senior Responsible Officer");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_OrganisationIsFound_DetailsNotGiven_And_ApplicantSeniorResponsibleOfficer()
        {
            // Arrange
            var isApplicantSeniorResponsibleOfficer = false;

            var command = new EditOrganisationSeniorResponsibleOfficerCommand()
            {
                OrganisationId = _existingOrganisation,
                IsApplicantSeniorResponsibleOfficer = isApplicantSeniorResponsibleOfficer
            };
            var expectedResult = Responses.BadRequest($"Cannot update Organisation with Id: {_existingOrganisation}, speficying no details and Applicant is not Senior Responsible Officer");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldUpdateOrganisation_When_OrganisationIsFound_DetailsGiven_And_ApplicantNotSeniorResponsibleOfficer()
        {
            // Arrange
            var name = "New Name";
            var jobTitle = "New Job Title";
            var telephoneNumber = "43289432";
            var isApplicantSeniorResponsibleOfficer = false;

            var command = new EditOrganisationSeniorResponsibleOfficerCommand()
            {
                JobTitle = jobTitle,
                Name = name,
                OrganisationId = _existingOrganisation,
                TelephoneNumber = telephoneNumber,
                IsApplicantSeniorResponsibleOfficer = isApplicantSeniorResponsibleOfficer
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockOrganisation.Verify(x => x.UpdateSeniorResponsibleOfficerIfExists(name, jobTitle, telephoneNumber), Times.Once());
            _mockOrganisationsRepository.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
