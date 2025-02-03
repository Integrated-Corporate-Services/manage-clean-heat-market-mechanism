using Xunit;
using Desnz.Chmm.Identity.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Options;
using Desnz.Chmm.Common;
using System.Linq.Expressions;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers;
using FluentAssertions;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using Desnz.Chmm.Identity.Common.Dtos;
using Newtonsoft.Json;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Common.Services;
using Microsoft.AspNetCore.Http;
using Desnz.Chmm.Identity.UnitTests.Helpers;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationAddressConstants;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands;

public class OnboardManufacturerCommandHandlerTests
{
    private readonly Mock<IUsersRepository> _mockUsersRepository;
    private readonly Mock<IRolesRepository> _mockRolesRepository;
    private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
    private readonly Mock<IIdentityNotificationService> _mockNotificationService;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<ILogger<OnboardManufacturerCommandHandler>> _mockLogger;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly OnboardManufacturerCommandHandler _handler;

    public OnboardManufacturerCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<OnboardManufacturerCommandHandler>>();
        _mockUsersRepository = new Mock<IUsersRepository>();
        _mockRolesRepository = new Mock<IRolesRepository>();
        _mockOrganisationsRepository = new Mock<IOrganisationsRepository>();
        _mockNotificationService = new Mock<IIdentityNotificationService>();
        _mockFileService = new Mock<IFileService>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _mockUsersRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

        _handler = new OnboardManufacturerCommandHandler(_mockLogger.Object,
                                                         _mockRolesRepository.Object,
                                                         _mockUsersRepository.Object,
                                                         _mockOrganisationsRepository.Object,
                                                         _mockFileService.Object,
                                                         _mockNotificationService.Object);
    }

    [Fact]
    internal async Task TestPostUser_BadRequestAsync()
    {
        //Arrange
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(GetCreateOrganisationDto()) };

        var expectedResult = Responses.NotFound("Failed to get Role with Id: Manufacturer");

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Result.Should().BeEquivalentTo(expectedResult);
    }


    [Fact]
    internal async Task TestPostUser_BadRequestAsync_WhenUserEists()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        var userDto = organisationDto.Users.First();

        var appUser = new ChmmUser(userDto, new List<ChmmRole> { manufacturerRole });
        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.Get(It.Is<Expression<Func<ChmmUser, bool>>>(y => y.Compile()(appUser)), false, false)).Returns(Task.FromResult(appUser));
        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));

        var expectedResult = Responses.BadRequest("Registration cannot be completed, contact the Administrator.");

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    internal async Task TestLegalAddress_BadRequestAsync_UseSpecifiedAddress_OnlySupplyOneAddress()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        organisationDto.LegalAddressType = LegalCorrespondenceAddressType.UseSpecifiedAddress;

        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));
        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();

        var expectedResult = Responses.BadRequest($"Cannot onboard Organisation speficying no address and {LegalCorrespondenceAddressType.UseSpecifiedAddress}");

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    internal async Task TestLegalAddress_BadRequestAsync_UseSpecifiedAddress_SupplyTwoAddressesButNoLegalAddress()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        organisationDto.Addresses.Add(new()
        {
            LineOne = "Test line one",
            City = "Test city",
            Postcode = "Test postcode",
            IsUsedAsLegalCorrespondence = false
        });
        organisationDto.LegalAddressType = LegalCorrespondenceAddressType.UseSpecifiedAddress;

        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));
        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();

        var expectedResult = Responses.BadRequest($"Cannot onboard Organisation speficying no address and {LegalCorrespondenceAddressType.UseSpecifiedAddress}");

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    internal async Task TestLegalAddress_BadRequestAsync_UseSpecifiedAddress_SupplyTwoAddressesButNoDetails()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        organisationDto.Addresses.Add(new()
        {
            IsUsedAsLegalCorrespondence = true
        });
        organisationDto.LegalAddressType = LegalCorrespondenceAddressType.UseSpecifiedAddress;

        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));
        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();

        var expectedResult = Responses.BadRequest($"Cannot onboard Organisation speficying no address and {LegalCorrespondenceAddressType.UseSpecifiedAddress}");

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    internal async Task TestLegalAddress_OkAsync_UseSpecifiedAddress()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        organisationDto.Addresses.Add(new()
        {
            LineOne = "Test line one",
            City = "Test city",
            Postcode = "Test postcode",
            IsUsedAsLegalCorrespondence = true
        });
        organisationDto.LegalAddressType = LegalCorrespondenceAddressType.UseSpecifiedAddress;

        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));
        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<Guid>>(actionResult);
        _mockNotificationService.Verify(x => x.NotifyManufacturerOnboarded(It.IsAny<Api.Entities.Organisation>()), Times.Exactly(1));
    }

    [Fact]
    internal async Task TestLegalAddress_BadRequestAsync_NoLegalCorrespondenceAddress_SupplyOnlyOneAddressButIsLegalAddress()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        organisationDto.Addresses.First().IsUsedAsLegalCorrespondence = true;
        organisationDto.LegalAddressType = LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress;

        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));
        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();

        var expectedResult = Responses.BadRequest($"Cannot onboard Organisation speficying address and {LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress}");

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    internal async Task TestLegalAddress_BadRequestAsync_NoLegalCorrespondenceAddress_SupplyTwoAddressesButNoDetails()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        organisationDto.Addresses.Add(new()
        {
            LineOne = "Test line one",
            City = "Test city",
            Postcode = "Test postcode",
            IsUsedAsLegalCorrespondence = false
        });
        organisationDto.LegalAddressType = LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress;

        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));
        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();

        var expectedResult = Responses.BadRequest($"Cannot onboard Organisation speficying address and {LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress}");

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    internal async Task TestLegalAddress_OkAsync_NoLegalCorrespondenceAddress()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        organisationDto.Addresses.First().IsUsedAsLegalCorrespondence = false;
        organisationDto.LegalAddressType = LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress;

        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));
        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<Guid>>(actionResult);
        _mockNotificationService.Verify(x => x.NotifyManufacturerOnboarded(It.IsAny<Api.Entities.Organisation>()), Times.Exactly(1));
    }

    [Fact]
    internal async Task TestLegalAddress_BadRequestAsync_UseRegisteredOffice_SupplyTwoAddressesNonLegalAddresses()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        organisationDto.Addresses.Add(new()
        {
            LineOne = "Test line one",
            City = "Test city",
            Postcode = "Test postcode",
            IsUsedAsLegalCorrespondence = false
        });
        organisationDto.LegalAddressType = LegalCorrespondenceAddressType.UseRegisteredOffice;

        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));
        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();

        var expectedResult = Responses.BadRequest($"Cannot onboard Organisation speficying address and {LegalCorrespondenceAddressType.UseRegisteredOffice}");

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    internal async Task TestLegalAddress_BadRequestAsync_UseRegisteredOffice_SupplyOneAddressNonLegalAddresses()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        organisationDto.Addresses.First().IsUsedAsLegalCorrespondence = false;
        organisationDto.LegalAddressType = LegalCorrespondenceAddressType.UseRegisteredOffice;

        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));
        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();

        var expectedResult = Responses.BadRequest($"Cannot onboard Organisation speficying address and {LegalCorrespondenceAddressType.UseRegisteredOffice}");

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    internal async Task TestLegalAddress_OkAsync_UseRegisteredOffice()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var organisationDto = GetCreateOrganisationDto();
        organisationDto.Addresses.Add(new()
        {
            LineOne = "Test line one",
            City = "Test city",
            Postcode = "Test postcode",
            IsUsedAsLegalCorrespondence = true
        });
        organisationDto.LegalAddressType = LegalCorrespondenceAddressType.UseRegisteredOffice;

        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });
        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = JsonConvert.SerializeObject(organisationDto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));
        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<Guid>>(actionResult);
        _mockNotificationService.Verify(x => x.NotifyManufacturerOnboarded(It.IsAny<Api.Entities.Organisation>()), Times.Exactly(1));
    }

    [Fact]
    internal async Task TestPostUser_SameApplicantAndResponsibleOfficer_OkAsync()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var applicant = new CreateManufacturerUserDto { Name = "Simon Blacksmith", Email = "Simon.Blacksmith@example.com", IsResponsibleOfficer = false };
        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });

        var dto = new CreateOrganisationDto()
        {
            IsOnBehalfOfGroup = true,
            ResponsibleUndertaking = new()
            {
                Name = $"Test Organisation",
                CompaniesHouseNumber = $"0228504"
            },
            Addresses = new List<Common.Dtos.OrganisationAddress.CreateOrganisationAddressDto>(),
            IsFossilFuelBoilerSeller = true,
            HeatPumpBrands = new[] { "Worcester Bosch", "Viessmann" },
            Users = new List<CreateManufacturerUserDto>() { new CreateManufacturerUserDto { Name = "Joe Bloggs", Email = "joe.bloggs@example.com", IsResponsibleOfficer = true } },
            CreditContactDetails = new()
            {
                Name = "Test Contact",
                Email = "test.contact@example.com",
                TelephoneNumber = "01908278450"
            }
        };

        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = System.Text.Json.JsonSerializer.Serialize(dto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));

        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();
        _mockNotificationService.Setup(x => x.NotifyManufacturerOnboarded(It.IsAny<Api.Entities.Organisation>()));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<Guid>>(actionResult);
        _mockNotificationService.Verify(x => x.NotifyManufacturerOnboarded(It.IsAny<Api.Entities.Organisation>()), Times.Exactly(1));
    }

    [Fact]
    internal async Task TestPostUser_DifferentApplicantAndResponsibleOfficer_OkAsync()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var applicant = new CreateManufacturerUserDto { Name = "Simon Blacksmith", Email = "Simon.Blacksmith@example.com", IsResponsibleOfficer = false };
        var responsibleOfficer = new CreateManufacturerUserDto { Name = "Joe Bloggs", Email = "joe.bloggs@example.com", IsResponsibleOfficer = true };
        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });

        var dto = new CreateOrganisationDto()
        {
            IsOnBehalfOfGroup = true,
            ResponsibleUndertaking = new()
            {
                Name = $"Test Organisation",
                CompaniesHouseNumber = $"0228504"
            },
            Addresses = new List<Common.Dtos.OrganisationAddress.CreateOrganisationAddressDto>(),
            IsFossilFuelBoilerSeller = true,
            HeatPumpBrands = new[] { "Worcester Bosch", "Viessmann" },
            Users = new List<CreateManufacturerUserDto>()
            {
                applicant, responsibleOfficer
            },
            CreditContactDetails = new()
            {
                Name = "Test Contact",
                Email = "test.contact@example.com",
                TelephoneNumber = "01908278450"
            }
        };

        var command = new OnboardManufacturerCommand() { OrganisationDetailsJson = System.Text.Json.JsonSerializer.Serialize(dto) };

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));

        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();
        _mockNotificationService.Setup(x => x.NotifyManufacturerOnboarded(It.IsAny<Api.Entities.Organisation>()));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<Guid>>(actionResult);
        _mockNotificationService.Verify(x => x.NotifyManufacturerOnboarded(It.IsAny<Api.Entities.Organisation>()), Times.Exactly(1));
    }

    [Fact]
    internal async Task TestPostUser_SameApplicantAndResponsibleOfficer_IncludeFiles_OkAsync()
    {
        //Arrange
        var manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);
        var adminRole = new ChmmRole(IdentityConstants.Roles.SeniorTechnicalOfficer);

        var applicant = new CreateManufacturerUserDto { Name = "Simon Blacksmith", Email = "Simon.Blacksmith@example.com", IsResponsibleOfficer = false };
        var adminUser = new ChmmUser("Joe Bloggs", "joe.bloggs@example.com", new List<ChmmRole> { adminRole });

        var dto = new CreateOrganisationDto()
        {
            IsOnBehalfOfGroup = true,
            ResponsibleUndertaking = new()
            {
                Name = $"Test Organisation",
                CompaniesHouseNumber = $"0228504"
            },
            Addresses = new List<CreateOrganisationAddressDto>(),
            IsFossilFuelBoilerSeller = true,
            HeatPumpBrands = new[] { "Worcester Bosch", "Viessmann" },
            Users = new List<CreateManufacturerUserDto>() { new CreateManufacturerUserDto { Name = "Joe Bloggs", Email = "joe.bloggs@example.com", IsResponsibleOfficer = true } },
            CreditContactDetails = new()
            {
                Name = "Test Contact",
                Email = "test.contact@example.com",
                TelephoneNumber = "01908278450"
            }
        };

        var files = new List<IFormFile>
        {
            FileHelper.CreateFormFile("File 1.txt")
        };

        var command = new OnboardManufacturerCommand()
        {
            OrganisationDetailsJson = System.Text.Json.JsonSerializer.Serialize(dto),
            Files = files
        };

        _mockFileService.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()))
            .Returns(Task.FromResult(new FileService.FileUploadResponse(null, "")));

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { adminUser }));

        _mockRolesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ChmmRole, bool>>>(), true)).Returns(Task.FromResult(manufacturerRole)).Verifiable();
        _mockNotificationService.Setup(x => x.NotifyManufacturerOnboarded(It.IsAny<Organisation>()));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.NotNull(actionResult);
        Assert.IsType<ActionResult<Guid>>(actionResult);
        _mockFileService.Verify(x => x.UploadFileAsync(
            It.Is<string>(s => s == "identity-organisation-structures"),
            It.Is<string>(s => s.EndsWith("File 1.txt")),
            It.IsAny<IFormFile>()), Times.Exactly(1));
    }

    private static CreateOrganisationDto GetCreateOrganisationDto()
    {
        var editOrganisationDto = new CreateOrganisationDto()
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
        return editOrganisationDto;
    }
}