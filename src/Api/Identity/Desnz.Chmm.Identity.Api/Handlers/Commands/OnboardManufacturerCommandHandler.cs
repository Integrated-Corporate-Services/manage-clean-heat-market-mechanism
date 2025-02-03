using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Api.Infrastructure.Repositories;
using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Desnz.Chmm.Identity.Api.Services;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Handlers;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationAddressConstants;

namespace Desnz.Chmm.Identity.Api.Handlers;

public class OnboardManufacturerCommandHandler : BaseRequestHandler<OnboardManufacturerCommand, ActionResult<Guid>>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IFileService _fileService;
    private readonly IRolesRepository _rolesRepository;
    private readonly IOrganisationsRepository _organisationsRepository;
    private readonly IIdentityNotificationService _notificationService;


    public OnboardManufacturerCommandHandler(
        ILogger<BaseRequestHandler<OnboardManufacturerCommand, ActionResult<Guid>>> logger,
        IRolesRepository rolesRepository,
        IUsersRepository usersRepository,
        IOrganisationsRepository organisationsRepository,
        IFileService fileService,
        IIdentityNotificationService notificationService) : base(logger)
    {
        _rolesRepository = rolesRepository;
        _usersRepository = usersRepository;
        _fileService = fileService;
        _organisationsRepository = organisationsRepository;
        _notificationService = notificationService;
    }

    public override async Task<ActionResult<Guid>> Handle(OnboardManufacturerCommand command, CancellationToken cancellationToken)
    {
        var organisationDetails = JsonConvert.DeserializeObject<CreateOrganisationDto>(command.OrganisationDetailsJson)!;

        var userEmails = organisationDetails.Users.Select(u => u.Email.ToLower()).ToList();
        foreach (var email in userEmails)
        {
            var existingUser = await _usersRepository.Get(u => u.Email == email);
            if (existingUser != null)
                return UserAlreadyExistsWhilstOnboarding();
        }

        var manufacturerRole = await _rolesRepository.Get(r => r.Name == IdentityConstants.Roles.Manufacturer, true);
        if (manufacturerRole == null)
            return CannotFindRole(IdentityConstants.Roles.Manufacturer);

        switch (organisationDetails.LegalAddressType)
        {
            case LegalCorrespondenceAddressType.UseSpecifiedAddress:
                if(organisationDetails.Addresses.Count() != 2)
                {
                    return MustSpeficyAddressDetails(organisationDetails.LegalAddressType);
                } 
                else if(organisationDetails.Addresses.Count(i => i.IsUsedAsLegalCorrespondence) != 1)
                {
                    return MustSpeficyAddressDetails(organisationDetails.LegalAddressType);
                }
                else
                {
                    var address = organisationDetails.Addresses.Single(i => i.IsUsedAsLegalCorrespondence);
                    if(string.IsNullOrEmpty(address.LineOne) ||
                       string.IsNullOrEmpty(address.City) ||
                       string.IsNullOrEmpty(address.Postcode))
                    {
                        return MustSpeficyAddressDetails(organisationDetails.LegalAddressType);
                    }
                }
                break;
            case LegalCorrespondenceAddressType.NoLegalCorrespondenceAddress:
                if(organisationDetails.Addresses.Count() > 1 || organisationDetails.Addresses.Any(i => i.IsUsedAsLegalCorrespondence))
                {
                    return MustNotSpeficyAddressDetails(organisationDetails.LegalAddressType);
                }
                break;
            case LegalCorrespondenceAddressType.UseRegisteredOffice:
                if (organisationDetails.Addresses.Count() != 2 || organisationDetails.Addresses.Count(i => i.IsUsedAsLegalCorrespondence) != 1)
                {
                    return MustNotSpeficyAddressDetails(organisationDetails.LegalAddressType);
                }
                break;
        }

        var createdBy = organisationDetails.Users[0].Name;

        var organisation = new Organisation(organisationDetails, new List<ChmmRole> { manufacturerRole }, createdBy);

        if (command.Files != null)
        {
            foreach (var file in command.Files)
            {
                var fileKey = $"{organisation.Id}/{file.FileName}";
                var response = await _fileService.UploadFileAsync(OrganisationFileConstants.Buckets.IdentityOrganisationStructures, fileKey, file);
                if (response.ValidationError != null)
                {
                    return Responses.BadRequest(response.ValidationError);
                }
                organisation.AddOrganisationStructureFile(new AwsFileDto(file.FileName, response.FileKey), createdBy);
            }
        }
        await _organisationsRepository.Create(organisation);

        await _notificationService.NotifyManufacturerOnboarded(organisation);

        return Responses.Created(organisation.Id);
    }
}