using Xunit;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.Configuration.Settings;
using Microsoft.Extensions.Options;
using Desnz.Chmm.Common;
using Desnz.Chmm.Identity.Api.Entities;
using FluentAssertions;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.Identity.Api.Services;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands;

public class IdentityNotificationServiceTests
{
    private readonly Mock<IUsersRepository> _mockUsersRepository;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<IOptions<GovukNotifyConfig>> _mockOptionsGovukNotifyConfig;
    private readonly Mock<IOptions<EnvironmentConfig>> _mockOptionsGovukEnvironmentConfig;
    private readonly Mock<ILogger<IdentityNotificationService>> _mockLogger;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    private readonly IdentityNotificationService _service;
    private readonly GovukNotifyConfig _govukNotifyConfig;
    private readonly ChmmRole _manufacturerRole;
    private readonly CreateManufacturerUserDto _applicant;
    private readonly CreateManufacturerUserDto _responsibleOfficer;
    private readonly List<ChmmRole> _adminRoles;
    private readonly ChmmUser _seniorTechnicalOfficer;
    private readonly ChmmUser _regulatoryOfficer;
    private readonly ChmmUser _principalTechnicalOfficer;

    private const string url = "https://chmm.gov.uk/";

    public IdentityNotificationServiceTests()
    {
        _manufacturerRole = new ChmmRole(IdentityConstants.Roles.Manufacturer);

        _applicant = new CreateManufacturerUserDto
        {
            TelephoneNumber = "023456789012",
            IsResponsibleOfficer = false,
            Name = "Applicant Officer",
            Email = "test1@test",
            JobTitle = "ApplicantOfficer"
        };

        _responsibleOfficer = new CreateManufacturerUserDto
        {
            TelephoneNumber = "012345678901",
            IsResponsibleOfficer = true,
            Name = "Responsible Officer",
            Email = "test2@test",
            JobTitle = "ResponsibleOfficer"
        };

        _adminRoles = IdentityConstants.Roles.AdminsList.Select(x => new ChmmRole(x)).ToList();
        
        _regulatoryOfficer = new ChmmUser("Admin 1", "z1@z", _adminRoles.Where(x => x.Name == IdentityConstants.Roles.RegulatoryOfficer).ToList());
        _seniorTechnicalOfficer = new ChmmUser("Admin 2", "z2@z", _adminRoles.Where(x => x.Name == IdentityConstants.Roles.SeniorTechnicalOfficer).ToList());
        _principalTechnicalOfficer = new ChmmUser("Admin 3", "z3@z", _adminRoles.Where(x => x.Name == IdentityConstants.Roles.PrincipalTechnicalOfficer).ToList());

        _mockLogger = new Mock<ILogger<IdentityNotificationService>>();
        _mockUsersRepository = new Mock<IUsersRepository>();
        _mockNotificationService = new Mock<INotificationService>();
        _mockOptionsGovukNotifyConfig = new Mock<IOptions<GovukNotifyConfig>>();
        _mockOptionsGovukEnvironmentConfig = new Mock<IOptions<EnvironmentConfig>>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _govukNotifyConfig = new GovukNotifyConfig();
        _mockOptionsGovukNotifyConfig.Setup(x => x.Value).Returns(_govukNotifyConfig);
        _mockOptionsGovukEnvironmentConfig.Setup(x => x.Value).Returns(new EnvironmentConfig() { BaseUrl = url });
        _mockUsersRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

        _service = new IdentityNotificationService(_mockLogger.Object,
                                                   _mockNotificationService.Object,
                                                   _mockUsersRepository.Object,
                                                   _mockOptionsGovukNotifyConfig.Object,
                                                   _mockOptionsGovukEnvironmentConfig.Object);

    }


    [Fact]
    internal async Task NotifyUserActivated_Can_Notify()
    {
        //Arrange
        var appUser = new ChmmUser(_responsibleOfficer, new List<ChmmRole> { _manufacturerRole });

        //Act
        await _service.NotifyUserActivated(appUser);

        //Assert
        _mockNotificationService.Verify(x => x.SendNotification(appUser.Email, GovukNotifyTemplateConstants.AccountActivatedTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null), Times.Exactly(1));
    }

    [Fact]
    internal async Task NotifyUserDeactivated_Can_Notify()
    {
        //Arrange
        var appUser = new ChmmUser(_responsibleOfficer, new List<ChmmRole> { _manufacturerRole });

        //Act
        await _service.NotifyUserDeactivated(appUser);

        //Assert
        _mockNotificationService.Verify(x => x.SendNotification(appUser.Email, GovukNotifyTemplateConstants.AccountDeactivatedTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null), Times.Exactly(1));
    }

    [Fact]
    internal async Task NotifyAdminUserInvited_Can_Notify()
    {
        //Arrange
        var appUser = new ChmmUser(_responsibleOfficer, new List<ChmmRole> { _manufacturerRole });

        //Act
        await _service.NotifyAdminUserInvited(appUser);

        //Assert
        _mockNotificationService.Verify(x => x.SendNotification(appUser.Email, GovukNotifyTemplateConstants.AdminAccountCreatedTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null), Times.Exactly(1));
    }

    [Fact]
    internal async Task NotifyAdminUserEdited_Can_Notify()
    {
        //Arrange
        var appUser = new ChmmUser(_responsibleOfficer, new List<ChmmRole> { _manufacturerRole });

        //Act
        await _service.NotifyAdminUserEdited(appUser);

        //Assert
        _mockNotificationService.Verify(x => x.SendNotification(appUser.Email, GovukNotifyTemplateConstants.AccountUpdatedTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null), Times.Exactly(1));
    }


    [Fact]
    internal async Task NotifyManufacturerUserEdited_Can_Notify()
    {
        //Arrange
        var appUser = new ChmmUser(_responsibleOfficer, new List<ChmmRole> { _manufacturerRole });

        //Act
        await _service.NotifyManufacturerUserEdited(appUser);

        //Assert
        _mockNotificationService.Verify(x => x.SendNotification(appUser.Email, GovukNotifyTemplateConstants.AccountUpdatedTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null), Times.Exactly(1));
    }


    [Fact]
    internal async Task NotifyOrganisationEdited_Can_Notify()
    {
        //Arrange
        var roles = new List<ChmmRole>() { _manufacturerRole };
        roles.AddRange(_adminRoles);

        var oganisation = new Organisation(GetCreateOrganisationDto(), roles);

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult( new List<ChmmUser> { _regulatoryOfficer, _seniorTechnicalOfficer, _principalTechnicalOfficer }));

        //Act
        await _service.NotifyOrganisationEdited(oganisation);

        //Assert
        _mockNotificationService.Verify(x => x.SendNotification(It.IsAny<string>(), GovukNotifyTemplateConstants.ManufacturerUpdatedTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null), Times.Exactly(5));
    }


    [Fact]
    internal async Task NotifyManufacturerOnboarded_Can_Notify()
    {
        //Arrange
        var roles = new List<ChmmRole>() { _manufacturerRole };
        roles.AddRange(_adminRoles);

        var oganisation = new Organisation(GetCreateOrganisationDto(), roles);

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { _regulatoryOfficer, _seniorTechnicalOfficer, _principalTechnicalOfficer }));

        //Act
        await _service.NotifyManufacturerOnboarded(oganisation);

        //Assert
        _mockNotificationService.Verify(x => x.SendNotification(It.IsAny<string>(), GovukNotifyTemplateConstants.ManufacturerSubmitApplicationTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null), Times.Exactly(5));
    }

    [Fact]
    internal async Task NotifyNotifyManufacturerApproved_Can_Notify()
    {
        //Arrange
        var roles = new List<ChmmRole>() { _manufacturerRole };
        roles.AddRange(_adminRoles);

        var oganisation = new Organisation(GetCreateOrganisationDto(), roles);

        _mockUsersRepository.Setup(x => x.GetAdmins(false)).Returns(Task.FromResult(new List<ChmmUser> { _regulatoryOfficer, _seniorTechnicalOfficer, _principalTechnicalOfficer }));

        //Act
        await _service.NotifyManufacturerApproved(oganisation);

        //Assert
        _mockNotificationService.Verify(x => x.SendNotification(It.IsAny<string>(), GovukNotifyTemplateConstants.ManufacturerApplicationApprovedTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null), Times.Exactly(5));
    }

    [Fact]
    internal async Task NotifyManufacturerUserInvited_Can_Notify()
    {
        //Arrange
        const string name = "Joe Bloggs";
        const string email = "joe@manufacturer.co.uk";
        var roles = new List<ChmmRole>() { _manufacturerRole };

        var userDto = new CreateManufacturerUserDto() 
        {
            Email = email,
            Name = name,
            IsResponsibleOfficer = false,
            JobTitle = "Compliance Officer",
            TelephoneNumber = "071234567"
        };
        var user = new ChmmUser(userDto, roles);
        const string invitingUserName = "Jane Doe";

        //Act
        await _service.NotifyManufacturerUserInvited(user, invitingUserName);

        //Assert
        _mockNotificationService.Verify(x => x.SendNotification(email, GovukNotifyTemplateConstants.ManufacturerAccountCreatedTemplateId, It.IsAny<Dictionary<string, dynamic>>(), null, null), Times.Once);
        var validatePersonalisation = (Dictionary<string, dynamic> d) =>
        {
            d.Should().ContainKey("user-that-did-inviting");
            ((object)d["user-that-did-inviting"]).Should().BeEquivalentTo(invitingUserName);
            d.Should().ContainKey("name");
            ((object)d["name"]).Should().BeEquivalentTo(name);
            d.Should().ContainKey("url");
            ((object)d["url"]).Should().BeEquivalentTo(url);
            return true;
        };
        _mockNotificationService.Verify(x => x.SendNotification(email, GovukNotifyTemplateConstants.ManufacturerAccountCreatedTemplateId, It.Is<Dictionary<string, dynamic>>(d => validatePersonalisation(d)), null, null), Times.Once);
    }

    private CreateOrganisationDto GetCreateOrganisationDto()
    {
        return new CreateOrganisationDto()
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
                _applicant,
                _responsibleOfficer
            },
            IsOnBehalfOfGroup = false,
            ResponsibleUndertaking = new ResponsibleUndertakingDto()
            {
                Name = "Test name",
            },
            IsFossilFuelBoilerSeller = false,
            CreditContactDetails = new CreditContactDetailsDto(),
        };
    }
}
