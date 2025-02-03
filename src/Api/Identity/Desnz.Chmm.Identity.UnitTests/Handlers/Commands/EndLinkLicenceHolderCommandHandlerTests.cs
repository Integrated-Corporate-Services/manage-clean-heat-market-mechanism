using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Providers;
using Desnz.Chmm.Configuration.Common.Dtos;
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
using Organisation = Desnz.Chmm.Identity.Api.Entities.Organisation;

namespace Desnz.Chmm.Identity.UnitTests.Handlers.Commands;

public class EndLinkLicenceHolderCommandHandlerTests
{
    private readonly Mock<ILogger<EndLinkLicenceHolderCommandHandler>> _mockLogger;
    private readonly Mock<ISchemeYearService> _mockSchemeYearService;
    private readonly Mock<ILicenceHolderRepository> _mockLicenceHolderRepository;
    private readonly Mock<ILicenceHolderLinkRepository> _mockLicenceHolderLinkRepository;
    private readonly Mock<IOrganisationsRepository> _mockOrganisationsRepository;

    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly DateTimeOverrideProvider _dateTimeProvider;
    private readonly EndLinkLicenceHolderCommandHandler _handler;

    private readonly Guid _licenceHolderId = Guid.NewGuid();
    private readonly LicenceHolder _licenceHolder = new LicenceHolder(1, "Something Else");
    private readonly Organisation _organisationToTransfer = new Organisation(new Common.Dtos.Organisation.CreateOrganisationDto
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

    public EndLinkLicenceHolderCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<EndLinkLicenceHolderCommandHandler>>();
        _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
        _mockLicenceHolderRepository = new Mock<ILicenceHolderRepository>(MockBehavior.Strict);
        _mockLicenceHolderLinkRepository = new Mock<ILicenceHolderLinkRepository>(MockBehavior.Strict);
        _mockOrganisationsRepository = new Mock<IOrganisationsRepository>(MockBehavior.Strict);

        _unitOfWork = new Mock<IUnitOfWork>();

        _mockLicenceHolderRepository.Setup(x => x.UnitOfWork).Returns(_unitOfWork.Object);

        _dateTimeProvider = new DateTimeOverrideProvider();

        _handler = new EndLinkLicenceHolderCommandHandler(
            _mockLogger.Object,
            _mockSchemeYearService.Object,
            _mockLicenceHolderRepository.Object,
            _mockLicenceHolderLinkRepository.Object,
            _mockOrganisationsRepository.Object,
            _dateTimeProvider);
    }

    [Fact]
    public async Task Handle_CannotLoadCurrentSchemeYear_Returns_BadRequest()
    {
        //Arrange
        var expectedResult = Responses.BadRequest($"Failed to get current Scheme Year, problem: null");

        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.InternalServerError), null, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, Guid.NewGuid(), new DateOnly(2024, 1, 1));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_InvalidLicenceHolderEndDate_Returns_BadRequest()
    {
        //Arrange
        _dateTimeProvider.OverrideDate(SchemeYearConstants.SurrenderDayDate.AddYears(1).AddDays(1));

        var schemeYearStartDate = SchemeYearConstants.StartDate.AddYears(1);
        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate,
            SurrenderDayDate = SchemeYearConstants.SurrenderDayDate.AddYears(1),
            PreviousSchemeYearId = Guid.NewGuid()
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);

        var previousSchemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = SchemeYearConstants.StartDate,
            SurrenderDayDate = SchemeYearConstants.SurrenderDayDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), null)).ReturnsAsync(previousSchemeYearResponse);

        var expectedResult = Responses.BadRequest($"End date of the link must be after the surrender day date of the scheme: {SchemeYearConstants.SurrenderDayDate}");

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, Guid.NewGuid(), schemeYearStartDate.AddDays(-1));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);
            
        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_CannotFindLicenceHolder_Returns_NotFound()
    {
        //Arrange
        var schemeYearStartDate = SchemeYearConstants.StartDate;
        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), true, true)).ReturnsAsync((LicenceHolder?)null);

        var expectedResult = Responses.NotFound($"Failed to get Licence Holder with Id: {_licenceHolderId}");

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, Guid.NewGuid(), schemeYearStartDate.AddDays(1));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_LicenceHolderAlreadyUnlinked_Returns_BadRequest()
    {
        //Arrange
        var schemeYearStartDate = SchemeYearConstants.StartDate;
        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(
            _licenceHolder);

        var expectedResult = Responses.BadRequest($"Licence holder {_licenceHolderId} is already unlinked");

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, Guid.NewGuid(), schemeYearStartDate.AddDays(1));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_CannotFindOrganisationToTransfer_Returns_NotFound()
    {
        //Arrange
        var organisationId = Guid.NewGuid();
        var organisationIdToTransfer = Guid.NewGuid();
        var schemeYearStartDate = SchemeYearConstants.StartDate;

        _licenceHolder.AddLicenceHolderLink(new LicenceHolderLink(organisationId, _licenceHolder.Id, schemeYearStartDate));

        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(
            _licenceHolder);
        _mockOrganisationsRepository.Setup(x => x.GetById(organisationIdToTransfer, false, false, false, false)).ReturnsAsync((Organisation?)null);

        var expectedResult = Responses.NotFound($"Failed to get Organisation with Id: {organisationIdToTransfer}");

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, organisationId, schemeYearStartDate.AddDays(1), organisationIdToTransfer);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_OrganisationToTransferNotActive_Returns_BadRequest()
    {
        //Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearStartDate = SchemeYearConstants.StartDate;

        _licenceHolder.AddLicenceHolderLink(new LicenceHolderLink(organisationId, _licenceHolder.Id, schemeYearStartDate));

        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(
            _licenceHolder);
        _mockOrganisationsRepository.Setup(x => x.GetById(_organisationToTransfer.Id, false, false, false, false)).ReturnsAsync(_organisationToTransfer);

        var expectedResult = Responses.BadRequest($"Organisation with Id: {_organisationToTransfer.Id} has an invalid status: Pending");

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, organisationId, schemeYearStartDate.AddDays(1), _organisationToTransfer.Id);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_StartDateBeforeEndDate_Returns_BadRequest()
    {
        //Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearStartDate = SchemeYearConstants.StartDate;

        _licenceHolder.AddLicenceHolderLink(new LicenceHolderLink(organisationId, _licenceHolder.Id, schemeYearStartDate));

        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(
            _licenceHolder);
        _mockLicenceHolderLinkRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var expectedResult = Responses.BadRequest($"End date of the link {schemeYearStartDate} must be after the start date of the link: {schemeYearStartDate.AddDays(-1)}");

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, organisationId, schemeYearStartDate.AddDays(-1));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_StartDateOnEndDate_Returns_BadRequest()
    {
        //Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearStartDate = SchemeYearConstants.StartDate;

        _licenceHolder.AddLicenceHolderLink(new LicenceHolderLink(organisationId, _licenceHolder.Id, schemeYearStartDate));

        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(
            _licenceHolder);
        _mockLicenceHolderLinkRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var expectedResult = Responses.BadRequest($"End date of the link {schemeYearStartDate} must be after the start date of the link: {schemeYearStartDate}");

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, organisationId, schemeYearStartDate);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Handle_SuccessNoTransfer_Returns_NoContent()
    {
        //Arrange
        var organisationId = Guid.NewGuid();
        var schemeYearStartDate = SchemeYearConstants.StartDate;

        _licenceHolder.AddLicenceHolderLink(new LicenceHolderLink(organisationId, _licenceHolder.Id, schemeYearStartDate));

        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(
            _licenceHolder);
        _mockLicenceHolderLinkRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, organisationId, schemeYearStartDate.AddDays(1));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(Responses.NoContent());
    }


    [Fact]
    public async Task Handle_SuccessNoTransfer_Returns_NoContent_DateValidation()
    {
        //Arrange
        var organisationId = Guid.NewGuid();

        _dateTimeProvider.OverrideDate(SchemeYearConstants.SurrenderDayDate.AddYears(1).AddDays(1));

        var schemeYearStartDate = SchemeYearConstants.StartDate.AddYears(1);
        _licenceHolder.AddLicenceHolderLink(new LicenceHolderLink(organisationId, _licenceHolder.Id, schemeYearStartDate));
        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate,
            SurrenderDayDate = SchemeYearConstants.SurrenderDayDate.AddYears(1),
            PreviousSchemeYearId = Guid.NewGuid()
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);

        var previousSchemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = SchemeYearConstants.StartDate,
            SurrenderDayDate = SchemeYearConstants.SurrenderDayDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), null)).ReturnsAsync(previousSchemeYearResponse);

        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(
            _licenceHolder);
        _mockLicenceHolderLinkRepository.Setup(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, organisationId, SchemeYearConstants.SurrenderDayDate.AddYears(1).AddDays(1));

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(Responses.NoContent());
    }

    [Fact]
    public async Task Handle_SuccessWithOrganisationTransfer_Returns_Created()
    {
        //Arrange
        var createdId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var schemeYearStartDate = SchemeYearConstants.StartDate;

        _licenceHolder.AddLicenceHolderLink(new LicenceHolderLink(organisationId, _licenceHolder.Id, schemeYearStartDate));
        _organisationToTransfer.Activate();

        var schemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
        {
            StartDate = schemeYearStartDate
        }, null);
        _mockSchemeYearService.Setup(x => x.GetCurrentSchemeYear(It.IsAny<CancellationToken>(), null)).ReturnsAsync(schemeYearResponse);
        _mockLicenceHolderRepository.Setup(x => x.Get(It.IsAny<Expression<Func<LicenceHolder, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(
            _licenceHolder);
        _mockOrganisationsRepository.Setup(x => x.GetById(_organisationToTransfer.Id, false, false, false, false)).ReturnsAsync(_organisationToTransfer);
        _mockLicenceHolderLinkRepository.Setup(x => x.Create(It.IsAny<LicenceHolderLink>(), true)).ReturnsAsync(createdId);

        var expectedResult = Responses.Created(createdId);

        var command = new EndLinkLicenceHolderCommand(_licenceHolderId, organisationId, schemeYearStartDate.AddDays(1), _organisationToTransfer.Id);

        //Act
        var actionResult = await _handler.Handle(command, CancellationToken.None);

        //Assert
        actionResult.Should().BeEquivalentTo(expectedResult);
    }
}
