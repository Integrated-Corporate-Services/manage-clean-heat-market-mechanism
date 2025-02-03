using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationAddressConstants;

namespace Desnz.Chmm.Identity.UnitTests.Commands.EditOrganisation
{
    public class EditOrganisationLegalCorrespondenceAddressCommandHandlerTests
    {
        private Mock<IOrganisationsRepository> _mockOrganisationsRepository;
        private EditOrganisationLegalCorrespondenceAddressCommandHandler _handler;
        private Mock<IOrganisation> _mockOrganisation;

        private Guid _existingOrganisation = Guid.NewGuid();
        private Guid _unknownOrganisation = Guid.NewGuid();

        public EditOrganisationLegalCorrespondenceAddressCommandHandlerTests()
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

            _handler = new EditOrganisationLegalCorrespondenceAddressCommandHandler(
                new Mock<ILogger<EditOrganisationLegalCorrespondenceAddressCommandHandler>>().Object,
                _mockOrganisationsRepository.Object);
        }

        [Fact]
        public async Task ShouldReturnNotFound_When_OrganisationIsNotfound()
        {
            // Arrange
            var command = new EditOrganisationLegalCorrespondenceAddressCommand()
            {
                OrganisationId = _unknownOrganisation,
                LegalAddressType = LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress
            };
            var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {_unknownOrganisation}");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldUpdateOrganisation_When_OrganisationIsFound_And_NoLegalCorrespondenceAddress()
        {
            // Arrange
            var addressType = LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress;

            var command = new EditOrganisationLegalCorrespondenceAddressCommand()
            {
                LegalAddressType = addressType,
                OrganisationId = _existingOrganisation
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockOrganisation.Verify(x => x.RemoveLegalCorrespondenceAddressIfExists(LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress), Times.Once());
            _mockOrganisationsRepository.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task ShouldUpdateOrganisation_When_OrganisationIsFound_And_UseRegisteredOffice()
        {
            // Arrange
            var addressType = LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress;

            var command = new EditOrganisationLegalCorrespondenceAddressCommand()
            {
                LegalAddressType = addressType,
                OrganisationId = _existingOrganisation
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockOrganisation.Verify(x => x.RemoveLegalCorrespondenceAddressIfExists(LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress), Times.Once());
            _mockOrganisationsRepository.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task ShouldUpdateOrganisation_When_OrganisationIsFound_And_UseSpecifiedAddress()
        {
            // Arrange
            var addressType = LegalCorrespondenceAddressType.UseSpecifiedAddress;
            var city = "New City";
            var county = "New County";
            var lineOne = "New Line 1";
            var lineTwo = "New Line 2";
            var postcode = "Post";

            var command = new EditOrganisationLegalCorrespondenceAddressCommand()
            {
                City = city,
                County = county,
                LineOne = lineOne,
                LineTwo = lineTwo,
                Postcode = postcode,
                LegalAddressType = addressType,
                OrganisationId = _existingOrganisation
            };

            _mockOrganisation.Setup(x => x.UpdateLegalCorrespondenceAddress(LegalCorrespondenceAddressType.UseSpecifiedAddress, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new OrganisationAddress(new Common.Dtos.OrganisationAddress.CreateOrganisationAddressDto())).Verifiable(Times.Once());
            _mockOrganisationsRepository
                .Setup(r => r.GetByIdForUpdate(_existingOrganisation, It.IsAny<bool>(), It.IsAny<bool>(), true))
                .ReturnsAsync(_mockOrganisation.Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockOrganisationsRepository.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task ShouldCreateOrganisationAddress_When_NoPreviousAddressFound_And_UseSpecifiedAddress()
        {
            // Arrange
            var addressType = LegalCorrespondenceAddressType.UseSpecifiedAddress;
            var city = "New City";
            var county = "New County";
            var lineOne = "New Line 1";
            var lineTwo = "New Line 2";
            var postcode = "Post";

            var command = new EditOrganisationLegalCorrespondenceAddressCommand()
            {
                City = city,
                County = county,
                LineOne = lineOne,
                LineTwo = lineTwo,
                Postcode = postcode,
                LegalAddressType = addressType,
                OrganisationId = _existingOrganisation
            };

            _mockOrganisation.Setup(x => x.UpdateLegalCorrespondenceAddress(LegalCorrespondenceAddressType.UseSpecifiedAddress, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((OrganisationAddress?)null).Verifiable(Times.Once());
            _mockOrganisationsRepository
                .Setup(r => r.GetByIdForUpdate(_existingOrganisation, It.IsAny<bool>(), It.IsAny<bool>(), true))
                .ReturnsAsync(_mockOrganisation.Object);
            _mockOrganisationsRepository.Setup(x => x.CreateAddress(It.IsAny<Guid>(), It.IsAny<OrganisationAddress>(), false))
                .ReturnsAsync(Guid.NewGuid()).Verifiable(Times.Once());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockOrganisationsRepository.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_OrganisationIsFound_AddressSpecified_And_NoLegalCorrespondenceAddress()
        {
            // Arrange
            var addressType = LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress;
            var city = "New City";
            var county = "New County";
            var lineOne = "New Line 1";
            var lineTwo = "New Line 2";
            var postcode = "Post";

            var command = new EditOrganisationLegalCorrespondenceAddressCommand()
            {
                City = city,
                County = county,
                LineOne = lineOne,
                LineTwo = lineTwo,
                Postcode = postcode,
                LegalAddressType = addressType,
                OrganisationId = _existingOrganisation
            };

            var expectedResult = Responses.BadRequest($"Cannot update Organisation with Id: {_existingOrganisation}, speficying address and {addressType}");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_OrganisationIsFound_AddressSpecified_And_UseRegisteredOffice()
        {
            // Arrange
            var addressType = LegalCorrespondenceAddressType.UseRegisteredOffice;
            var city = "New City";
            var county = "New County";
            var lineOne = "New Line 1";
            var lineTwo = "New Line 2";
            var postcode = "Post";

            var command = new EditOrganisationLegalCorrespondenceAddressCommand()
            {
                City = city,
                County = county,
                LineOne = lineOne,
                LineTwo = lineTwo,
                Postcode = postcode,
                LegalAddressType = addressType,
                OrganisationId = _existingOrganisation
            };

            var expectedResult = Responses.BadRequest($"Cannot update Organisation with Id: {_existingOrganisation}, speficying address and {addressType}");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ShouldReturnBadRequest_When_OrganisationIsFound_AddressNotSpecified_And_UseSpecified()
        {
            // Arrange
            var addressType = LegalCorrespondenceAddressType.UseSpecifiedAddress;

            var command = new EditOrganisationLegalCorrespondenceAddressCommand()
            {
                LegalAddressType = addressType,
                OrganisationId = _existingOrganisation
            };

            var expectedResult = Responses.BadRequest($"Cannot update Organisation with Id: {_existingOrganisation}, speficying no address and {addressType}");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
