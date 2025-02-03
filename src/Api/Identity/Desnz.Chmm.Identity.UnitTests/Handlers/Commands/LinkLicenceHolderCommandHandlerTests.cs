using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Commands;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Handlers.Commands;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands;

public class LinkLicenceHolderCommandHandlerTests
{
    private readonly Mock<ILogger<LinkLicenceHolderCommandHandler>> _mockLogger;
    private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;
    private readonly Mock<ILicenceHolderRepository> _mockLicenceHolderRepository;
    private readonly Mock<ILicenceHolderLinkRepository> _mockLicenceHolderLinkRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly Mock<Chmm.Common.Providers.IDateTimeProvider> _mockDateTimeProvider;
    private readonly Mock<ICreditLedgerService> _mockCreditLedgerService;

    private readonly LinkLicenceHolderCommandHandler _handler;

    private readonly Guid _licenceHolderId = Guid.NewGuid();
    private readonly Guid _organisationId = Guid.NewGuid();
    private readonly LicenceHolder _licenceHolder = new LicenceHolder(1, "Something Else");
    private readonly Organisation _organisation = new Organisation(new Common.Dtos.Organisation.CreateOrganisationDto
    {
        Addresses = new(),
        CreditContactDetails = new Common.Dtos.CreditContactDetailsDto { },
        HeatPumpBrands = Array.Empty<string>(),
        ResponsibleUndertaking = new Common.Dtos.ResponsibleUndertakingDto { },
        Users = new()
        {
             new Common.Dtos.ManufacturerUser.CreateManufacturerUserDto
             {
                  IsResponsibleOfficer = true,
                  Email = "email@example.com"
             }
        }
    }, new() { new ChmmRole(Roles.Manufacturer) });

    private static readonly DateOnly SchemeYearStartDate = new DateOnly(2024, 1, 1);
    private readonly HttpObjectResponse<SchemeYearDto> SchemeYearResponse = new(new HttpResponseMessage(HttpStatusCode.OK), 
        new SchemeYearDto()
            {
                SurrenderDayDate = new DateOnly(2024, 4, 4),
                StartDate = SchemeYearStartDate
            }
        , null);

    public LinkLicenceHolderCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<LinkLicenceHolderCommandHandler>>();
        _mockOrganisationsRepository = new Mock<IOrganisationsRepository>();
        _mockLicenceHolderRepository = new Mock<ILicenceHolderRepository>();
        _mockLicenceHolderLinkRepository = new Mock<ILicenceHolderLinkRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mockSchemeYearService = new Mock<ISchemeYearService>();
        _mockDateTimeProvider = new Mock<Chmm.Common.Providers.IDateTimeProvider>();
        _mockCreditLedgerService = new Mock<ICreditLedgerService>(MockBehavior.Strict);

        _mockLicenceHolderRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

        _handler = new LinkLicenceHolderCommandHandler(
            _mockLogger.Object,
            _mockOrganisationsRepository.Object,
            _mockLicenceHolderRepository.Object,
            _mockLicenceHolderLinkRepository.Object,
            _mockSchemeYearService.Object,
            _mockDateTimeProvider.Object,
            _mockCreditLedgerService.Object);
    }

    [Fact]
    public async Task Handle_Invalid_OrganisationId_Returns_NotFound()
    {
        //Arrange
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult<Organisation>(null));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));

        var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {_organisationId}");

        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisationId);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_OrganisationToTransferNotActive_Returns_BadRequest()
    {
        //Arrange
        var schemeYearStartDate = SchemeYearConstants.StartDate;

        _licenceHolder.AddLicenceHolderLink(new LicenceHolderLink(_organisation.Id, _licenceHolder.Id, schemeYearStartDate));

        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(_licenceHolder);

        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), It.IsAny<List<Guid>>(), It.IsAny<bool>())).ReturnsAsync(_organisation);

        var expectedResult = Responses.BadRequest($"Organisation with Id: {_organisation.Id} has an invalid status: Pending");

        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisation.Id);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }


    [Fact]
    public async Task Handle_Invalid_LicenceHolderId_Returns_NotFound()
    {
        //Arrange
        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult<LicenceHolder>(null));

        var expectedResult = Responses.NotFound($"Failed to get Licence Holder with Id: {_licenceHolderId}");

        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisationId);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_LicenceHolder_Already_Linked_With_Onngoing_EndDate_Returns_BadRequest()
    {
        //Arrange
        var date = new DateOnly(2024, 1, 1);
        var licenceHolderLink = new LicenceHolderLink(_organisation.Id, _licenceHolder.Id, date);
        licenceHolderLink.EndLink(date.AddDays(1));
        _licenceHolder.AddLicenceHolderLink(new LicenceHolderLink(_organisation.Id, _licenceHolder.Id, date));

        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));
        _mockDateTimeProvider.Setup(x => x.UtcDateNow).Returns(date.AddDays(2));

        var expectedResult = Responses.BadRequest($"Licence holder {_licenceHolderId} already has an ongoing link");

        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisation.Id);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_LicenceHolder_Already_Linked_With_null_EndDate_Returns_BadRequest()
    {
        //Arrange
        var date = new DateOnly(2024, 1, 1);
        var licenceHolderLink = new LicenceHolderLink(_organisation.Id, _licenceHolder.Id, date);
        _licenceHolder.AddLicenceHolderLink(new LicenceHolderLink(_organisation.Id, _licenceHolder.Id, date));

        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));
        _mockDateTimeProvider.Setup(x => x.UtcDateNow).Returns(new DateOnly(2024, 1, 1));

        var expectedResult = Responses.BadRequest($"Licence holder {_licenceHolderId} already has an ongoing link");

        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisation.Id);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_Invalid_CurrentSchemeYear_Returns_BadRequest()
    {
        //Arrange
        var expectedResult = Responses.BadRequest($"Failed to get current Scheme Year, problem: null");

        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));
        _mockSchemeYearService.Setup(x => x.GetFirstSchemeYear(It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);

        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(httpResponse);

        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisationId, new DateOnly(2023, 12, 31));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }


    [Fact]
    public async Task Handle_Invalid_FirstSchemeYear_Returns_BadRequest()
    {
        //Arrange
        var expectedResult = Responses.BadRequest($"Failed to get first Scheme Year, problem: null");

        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));
        _mockSchemeYearService.Setup(x => x.GetFirstSchemeYear(It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);

        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetFirstSchemeYear(It.IsAny<string>())).ReturnsAsync(httpResponse);

        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisationId, new DateOnly(2023, 12, 31));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_PreviousSchemeYear_SurrenderDayDate_Earlier_ThanStartDate_DoesNotCarryOver_PreviousYearCredits()
    {
        //Arrange
        var id = Guid.NewGuid();
        var expectedResult = Responses.Created(id);

        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));
        _mockSchemeYearService.Setup(x => x.GetFirstSchemeYear(It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);

        SchemeYearResponse.Result.PreviousSchemeYearId = Guid.NewGuid();
        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);

        _mockLicenceHolderLinkRepository.Setup(x => x.Create(It.IsAny<LicenceHolderLink>(), true)).ReturnsAsync(id);
        _mockLicenceHolderLinkRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            PreviousSchemeYearId = Guid.NewGuid()
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);
        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisationId);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
        _mockCreditLedgerService.Verify(x => x.CarryOverNewLicenceHolders(It.IsAny<CarryOverNewLicenceHoldersCommand>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PreviousSchemeYear_Invalid_PreviousYear_Returns_BadRequest()
    {
        //Arrange
        var id = Guid.NewGuid();
        var expectedResult = Responses.BadRequest($"Failed to get previous Scheme Year, problem: null");
        //var expectedResult = Responses.Created(id);

        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));
        _mockSchemeYearService.Setup(x => x.GetFirstSchemeYear(It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);

        HttpObjectResponse<SchemeYearDto> schemeYearResponse = new(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);

        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemeYearResponse);

        _mockLicenceHolderLinkRepository.Setup(x => x.Create(It.IsAny<LicenceHolderLink>(), true)).ReturnsAsync(id);
        _mockLicenceHolderLinkRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            PreviousSchemeYearId = Guid.NewGuid()
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);
        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisationId);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
        _mockCreditLedgerService.Verify(x => x.CarryOverNewLicenceHolders(It.IsAny<CarryOverNewLicenceHoldersCommand>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Success_No_PreviousSchemeYear_Returns_Created()
    {
        //Arrange
        var id = Guid.NewGuid();
        var expectedResult = Responses.Created(id);

        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));
        _mockSchemeYearService.Setup(x => x.GetFirstSchemeYear(It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);
        _mockLicenceHolderLinkRepository.Setup(x => x.Create(It.IsAny<LicenceHolderLink>(), true)).ReturnsAsync(id);
        _mockLicenceHolderLinkRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);
        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisationId);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
        _mockCreditLedgerService.Verify(x => x.CarryOverNewLicenceHolders(It.IsAny<CarryOverNewLicenceHoldersCommand>(), It.IsAny<string>()), Times.Never);
    }


    [Fact]
    public async Task Handle_Success_PreviousSchemeYear_DoesNot_CarryOver_Returns_Created()
    {
        //Arrange
        var id = Guid.NewGuid();
        var expectedResult = Responses.Created(id);

        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));
        _mockSchemeYearService.Setup(x => x.GetFirstSchemeYear(It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);
        _mockLicenceHolderLinkRepository.Setup(x => x.Create(It.IsAny<LicenceHolderLink>(), true)).ReturnsAsync(id);
        _mockLicenceHolderLinkRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            PreviousSchemeYearId = Guid.NewGuid()
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);


        SchemeYearResponse.Result.PreviousSchemeYearId = Guid.NewGuid();
        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);


        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisationId);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
        _mockCreditLedgerService.Verify(x => x.CarryOverNewLicenceHolders(It.IsAny<CarryOverNewLicenceHoldersCommand>(), It.IsAny<string>()), Times.Never);
    }


    [Fact]
    public async Task Handle_Success_PreviousSchemeYear_Returns_Failed_CarryOver_Invokation()
    {
        //Arrange
        var id = Guid.NewGuid();
        var expectedResult = Responses.BadRequest($"Failed to carry over credits for new licence holders, problem: null");

        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));
        _mockSchemeYearService.Setup(x => x.GetFirstSchemeYear(It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);
        _mockLicenceHolderLinkRepository.Setup(x => x.Create(It.IsAny<LicenceHolderLink>(), true)).ReturnsAsync(id);
        _mockLicenceHolderLinkRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockDateTimeProvider.Setup(x => x.UtcDateNow).Returns(new DateOnly(2024, 4, 5));

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            PreviousSchemeYearId = Guid.NewGuid()
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);


        var response = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.BadRequest), null);

        _mockCreditLedgerService.Setup(x => x.CarryOverNewLicenceHolders(It.IsAny<CarryOverNewLicenceHoldersCommand>(), It.IsAny<string>())).Returns(Task.FromResult(response));

        SchemeYearResponse.Result.PreviousSchemeYearId = Guid.NewGuid();
        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);


        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisationId, new DateOnly(2024, 4, 1));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
        _mockCreditLedgerService.Verify(x => x.CarryOverNewLicenceHolders(It.IsAny<CarryOverNewLicenceHoldersCommand>(), It.IsAny<string>()), Times.Once);
    }


    [Fact]
    public async Task Handle_Success_PreviousSchemeYear_Returns_Created888()
    {
        //Arrange
        var id = Guid.NewGuid();
        var expectedResult = Responses.Created(id);

        _organisation.Activate();
        _mockOrganisationsRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Organisation, bool>>>(), null, It.IsAny<bool>())).Returns(Task.FromResult(_organisation));
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(_licenceHolder));
        _mockSchemeYearService.Setup(x => x.GetFirstSchemeYear(It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);
        _mockLicenceHolderLinkRepository.Setup(x => x.Create(It.IsAny<LicenceHolderLink>(), true)).ReturnsAsync(id);
        _mockLicenceHolderLinkRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockDateTimeProvider.Setup(x => x.UtcDateNow).Returns(new DateOnly(2024, 4, 5));

        var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            Id = SchemeYearConstants.Id,
            Year = SchemeYearConstants.Year,
            PreviousSchemeYearId = Guid.NewGuid()
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

        var response = new HttpObjectResponse<object>(new HttpResponseMessage(HttpStatusCode.OK), null);

        _mockCreditLedgerService.Setup(x => x.CarryOverNewLicenceHolders(It.IsAny<CarryOverNewLicenceHoldersCommand>(), It.IsAny<string>())).Returns(Task.FromResult(response));

        SchemeYearResponse.Result.PreviousSchemeYearId = Guid.NewGuid();
        var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(SchemeYearResponse);

        var command = new LinkLicenceHolderCommand(_licenceHolderId, _organisationId, new DateOnly(2024, 4, 1));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
        _mockCreditLedgerService.Verify(x => x.CarryOverNewLicenceHolders(It.IsAny<CarryOverNewLicenceHoldersCommand>(), It.IsAny<string>()), Times.Once);
    }
}
