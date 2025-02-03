using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Queries;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Queries;

public class GetLicenceHolderLinksHistoryQueryHandlerTests : TestClaimsBase
{
    private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
    private readonly Mock<ILogger<GetLicenceHolderLinksHistoryQueryHandler>> _mockLogger;

    private readonly GetLicenceHolderLinksHistoryQueryHandler _handler;

    public GetLicenceHolderLinksHistoryQueryHandlerTests()
    {

        _mockLogger = new Mock<ILogger<GetLicenceHolderLinksHistoryQueryHandler>>();
        _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);

        _handler = new GetLicenceHolderLinksHistoryQueryHandler(_mockLogger.Object, _mockOrganisationsRepository.Object);
    }

    [Fact]
    internal async Task ShouldReturnNotFound_When_OrganisationDoesNotExists()
    {
        //Arrange
        var organisationId = Guid.NewGuid();

        var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {organisationId}");

        _mockOrganisationsRepository.Setup(x => x.GetById(It.IsAny<Guid>(), false, false, true, false)).ReturnsAsync((Organisation?)null);

        //Act
        var result = await _handler.Handle(new GetLicenceHolderLinksHistoryQuery(organisationId), CancellationToken.None);

        //Assert
        result.Result.Should().BeEquivalentTo(expectedResult);
        result.Value.Should().BeNull();
    }


    [Fact]
    internal async Task ShouldReturnAllLinksForEachLicenceHolderBasedOnStartDate_When_OrganisationExists()
    {
        //Arrange
        var organisation = GetMockOrganisation();

        var licenseHolder = new LicenceHolder(1, "Test licence holder");
        var licenseHolder2 = new LicenceHolder(2, "Test licence holder 2");

        var licenceHolderLink = new LicenceHolderLink(licenseHolder, organisation.Id, new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 2));
        var licenceHolderLink2 = new LicenceHolderLink(licenseHolder, organisation.Id, new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 3));
        var licenceHolderLink3 = new LicenceHolderLink(licenseHolder2, organisation.Id, new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 3));
        var licenceHolderLinks = new List<LicenceHolderLink>
        {
            licenceHolderLink,
            licenceHolderLink2,
            licenceHolderLink3
        };

        foreach (var link in licenceHolderLinks)
        {
            organisation.AddLicenceHolderLink(link);
        }

        var expectedLicenceHolders = new List<LicenceHolderLinkDto>
        {
            new LicenceHolderLinkDto
            {
                Id = licenceHolderLink.Id,
                OrganisationId = organisation.Id,
                OrganisationName = organisation.Name,
                LicenceHolderId = licenseHolder.Id,
                LicenceHolderName = licenseHolder.Name,
                StartDate = licenceHolderLink.StartDate
            },
            new LicenceHolderLinkDto
            {
                Id = licenceHolderLink2.Id,
                OrganisationId = organisation.Id,
                OrganisationName = organisation.Name,
                LicenceHolderId = licenseHolder.Id,
                LicenceHolderName = licenseHolder.Name,
                StartDate = licenceHolderLink2.StartDate
            },
            new LicenceHolderLinkDto
            {
                Id = licenceHolderLink3.Id,
                OrganisationId = organisation.Id,
                OrganisationName = organisation.Name,
                LicenceHolderId = licenseHolder2.Id,
                LicenceHolderName = licenseHolder2.Name,
                StartDate = licenceHolderLink3.StartDate
            }
        };

        var query = new GetLicenceHolderLinksHistoryQuery(organisation.Id);
        _mockOrganisationsRepository.Setup(x => x.GetById(It.IsAny<Guid>(), false, false, true, false)).ReturnsAsync(organisation);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        result.Value.Should().BeEquivalentTo(expectedLicenceHolders);
    }

    public static Organisation GetMockOrganisation()
    {
        var createOrganisationDto = new CreateOrganisationDto()
        {
            Addresses = new List<CreateOrganisationAddressDto>()
            {
                new()
                {
                    LineOne = "Test line one",
                    City = "Test city",
                    Postcode = "Test postcode",
                    IsUsedAsLegalCorrespondence = false
                }
            },
            Users = new List<CreateManufacturerUserDto>()
            {
                new()
                {
                    TelephoneNumber = "012345678901",
                    IsResponsibleOfficer = true,
                    Name = "Test name",
                    Email = "test@test",
                    JobTitle = "Test job title"
                }
            },
            IsOnBehalfOfGroup = false,
            ResponsibleUndertaking = new ResponsibleUndertakingDto()
            {
                Name = "Test name",
            },
            IsFossilFuelBoilerSeller = false,
            CreditContactDetails = new CreditContactDetailsDto(),
        };
        var roles = new List<ChmmRole>() { new ChmmRole(Roles.Manufacturer) };
        var organisation = new Api.Entities.Organisation(createOrganisationDto, roles);
        return organisation;
    }
}
