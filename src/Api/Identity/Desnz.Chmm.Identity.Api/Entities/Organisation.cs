using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Common.Entities;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using System.Runtime.Intrinsics.X86;
using OrganisationConstants = Desnz.Chmm.Identity.Common.Constants.OrganisationConstants;

namespace Desnz.Chmm.Identity.Api.Entities
{
    /// <summary>
    /// Organisation entity
    /// </summary>
    public class Organisation : Entity, IOrganisation
    {
        #region Properties

        /// <summary>
        /// Full name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Companies House Number
        /// </summary>
        public string? CompaniesHouseNumber { get; private set; }

        /// <summary>
        /// Is the organisation part of a group
        /// </summary>
        public bool IsGroupRegistration { get; private set; }

        /// <summary>
        /// Is the organisation a fossil fuel boiler seller
        /// </summary>
        public bool IsFossilFuelBoilerSeller { get; private set; }

        /// <summary>
        /// Is the organisation a scheme participant or not?
        /// </summary>
        public bool IsNonSchemeParticipant { get; private set; }

        /// <summary>
        /// Stores information about the legal correspondence address:
        /// - Use the registered office address
        /// - Use a different address
        /// - I am not a scheme participant or near threshold supplier
        /// </summary>
        public string LegalAddressType { get;protected set; }

        /// <summary>
        /// Heat pump brands provided by the Organisation
        /// </summary>
        public string[]? HeatPumpBrands { get; private set; }

        /// <summary>
        /// Contact name for the Organisation
        /// </summary>
        public string? ContactName { get; private set; }

        /// <summary>
        /// Contact email for the Organisation
        /// </summary>
        public string? ContactEmail { get; private set; }

        /// <summary>
        /// Contact telephone number for the Organisation
        /// </summary>
        public string? ContactTelephoneNumber { get; private set; }

        public string Status { get; private set; }

        public Guid ResponsibleOfficerId { get; private set; }

        public Guid ApplicantId { get; private set; }



        /// <summary>
        /// Organisation's addresses
        /// </summary>
        public IReadOnlyCollection<OrganisationAddress> Addresses => _addresses;
        public IReadOnlyCollection<OrganisationDecisionComment> Comments => _comments;
        public IReadOnlyCollection<ChmmUser> ChmmUsers => _chmmUsers;
        public IReadOnlyCollection<OrganisationStructureFile> OrganisationStructureFiles => _organisationStructureFiles;
        public IReadOnlyCollection<OrganisationDecisionFile> OrganisationDecisionFiles => _organisationDecisionFiles;
        public IReadOnlyCollection<LicenceHolderLink> LicenceHolderLinks => _licenceHolderLinks;

        #endregion

        #region Private fields

        private List<OrganisationAddress> _addresses;
        private List<ChmmUser> _chmmUsers;
        private List<OrganisationDecisionComment> _comments;
        private List<OrganisationStructureFile> _organisationStructureFiles;
        private List<OrganisationDecisionFile> _organisationDecisionFiles;
        private List<LicenceHolderLink> _licenceHolderLinks;

        #endregion

        #region Constructors

        protected Organisation() : base()
        {
        }

        public Organisation(CreateOrganisationDto organisationDetail, List<ChmmRole> roles, string? createdBy = null) : base(createdBy)
        {
            IsGroupRegistration = organisationDetail.IsOnBehalfOfGroup;
            IsNonSchemeParticipant = organisationDetail.IsNonSchemeParticipant;
            LegalAddressType = organisationDetail.LegalAddressType;
            SetResponsibleUndertakingDetails(organisationDetail.ResponsibleUndertaking);

            _addresses = new List<OrganisationAddress>();
            AddAddresses(organisationDetail.Addresses, createdBy);

            IsFossilFuelBoilerSeller = organisationDetail.IsFossilFuelBoilerSeller;
            HeatPumpBrands = organisationDetail.HeatPumpBrands;

            _chmmUsers = new List<ChmmUser>();
            AddResponsibleOfficer(organisationDetail.Users, roles, createdBy);
            AddApplicant(organisationDetail.Users, roles, createdBy);

            _organisationStructureFiles = new List<OrganisationStructureFile>();
            _organisationDecisionFiles = new List<OrganisationDecisionFile>();

            SetContactDetails(organisationDetail.CreditContactDetails);
            Status = OrganisationConstants.Status.Pending;
        }

        #endregion

        #region Behaviours

        public List<LicenceHolderLinkDto> GetOngoingLicenceHolderLinks(DateOnly currentDate)
        {
            if (LicenceHolderLinks == null)
            {
                return new List<LicenceHolderLinkDto>();
            }

            return LicenceHolderLinks.Where(l => l.EndDate == null || l.EndDate >= currentDate).GroupBy(l => l.LicenceHolderId).Select(g =>
            {
                var link = g.OrderByDescending(l => l.StartDate).First();
                return new LicenceHolderLinkDto
                {
                    Id = link.Id,
                    LicenceHolderId = link.LicenceHolderId,
                    LicenceHolderName = link.LicenceHolder.Name,
                    OrganisationId = link.OrganisationId,
                    OrganisationName = Name,
                    StartDate = link.StartDate,
                    EndDate = link.EndDate,
                };
            }).ToList();
        }

        public List<LicenceHolderLinkDto> GetOngoingLicenceHolderLinksHistory()
        {
            if (LicenceHolderLinks == null)
            {
                return new List<LicenceHolderLinkDto>();
            }

            return LicenceHolderLinks.Select(l =>
            {
                return new LicenceHolderLinkDto
                {
                    Id = l.Id,
                    LicenceHolderId = l.LicenceHolderId,
                    LicenceHolderName = l.LicenceHolder.Name,
                    OrganisationId = l.OrganisationId,
                    OrganisationName = Name,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                };
            }).ToList();
        }

        public List<ViewOrganisationLicenceHolderDto> GetLinkedLicenceHolders()
        {
            if (LicenceHolderLinks == null)
            {
                return new List<ViewOrganisationLicenceHolderDto>();
            }

            return LicenceHolderLinks.Where(l => l.EndDate == null).GroupBy(l => l.LicenceHolderId).Select(g =>
            {
                var link = g.OrderByDescending(l => l.StartDate).First();
                return new ViewOrganisationLicenceHolderDto
                {
                    Name = link.LicenceHolder.Name
                };
            }).ToList();
        }

        // TODO: Used temporary for unit testing until we replace LicenceHolderLinkRepository Create with EF update via Organisation entity
        public void AddLicenceHolderLink(LicenceHolderLink licenceHolderLink)
        {
            _licenceHolderLinks ??= new List<LicenceHolderLink>();
            _licenceHolderLinks.Add(licenceHolderLink);
        }

        public void UpdateOrganisationDetails(EditOrganisationDto organisationDetail)
        {
            IsGroupRegistration = organisationDetail.IsOnBehalfOfGroup;
            LegalAddressType = organisationDetail.LegalAddressType;
            IsNonSchemeParticipant = organisationDetail.IsNonSchemeParticipant;
            SetResponsibleUndertakingDetails(organisationDetail.ResponsibleUndertaking);
            UpdateAddresses(organisationDetail.Addresses);
            IsFossilFuelBoilerSeller = organisationDetail.IsFossilFuelBoilerSeller;
            HeatPumpBrands = organisationDetail.HeatPumpBrands;
            UpdateUsers(organisationDetail.Users);
            SetContactDetails(organisationDetail.CreditContactDetails);
        }

        public void SetResponsibleUndertakingDetails(ResponsibleUndertakingDto responsibleUndertaking)
        {
            Name = responsibleUndertaking.Name;
            CompaniesHouseNumber = responsibleUndertaking.CompaniesHouseNumber;
        }

        public void SetContactDetails(CreditContactDetailsDto contact)
        {
            ContactName = contact.Name;
            ContactEmail = contact.Email;
            ContactTelephoneNumber = contact.TelephoneNumber;
        }

        public void Activate()
        {
            Status = OrganisationConstants.Status.Active;
        }

        public void Archive()
        {
            Status = OrganisationConstants.Status.Archived;
        }

        public void AddAddresses(List<CreateOrganisationAddressDto> organisationAddresses, string? createdBy = null)
        {
            var addresses = organisationAddresses.Select(a => new OrganisationAddress(a, createdBy)).ToList();
            _addresses.AddRange(addresses);
        }

        public void UpdateAddresses(List<EditOrganisationAddressDto> updatedAddresses)
        {
            var updatedLegalAddress = updatedAddresses.SingleOrDefault(x => x.IsUsedAsLegalCorrespondence);
            var legalAddress = Addresses.SingleOrDefault(x => x.Type == OrganisationAddressConstants.Type.LegalCorrespondenceAddress);
            var officeAddress = Addresses.Single(x => x.Type == OrganisationAddressConstants.Type.OfficeAddress);

            if (updatedLegalAddress == null && legalAddress != null)
            {
                ((ICollection<OrganisationAddress>)Addresses).Remove(legalAddress);
            }
            else if (legalAddress != null)
            {
                legalAddress.SetOrganisationAddressDetails(updatedAddresses.Single(x => x.IsUsedAsLegalCorrespondence));
            }

            officeAddress.SetOrganisationAddressDetails(updatedAddresses.Single(x => !x.IsUsedAsLegalCorrespondence));
        }

        public void AddResponsibleOfficer(List<CreateManufacturerUserDto> organisationUsers, List<ChmmRole> roles, string? createdBy = null)
        {
            var responsibleOfficer = organisationUsers.Single(u => u.IsResponsibleOfficer);
            var user = new ChmmUser(responsibleOfficer, roles, createdBy: createdBy);

            ResponsibleOfficerId = user.Id;
            if (organisationUsers.Count == 1)
            {
                ApplicantId = user.Id;
            }

            _chmmUsers.Add(user);
        }

        public void AddUser(CreateManufacturerUserDto organisationUser, List<ChmmRole> roles, string? createdBy = null)
        {
            if (organisationUser != null)
            {
                var user = new ChmmUser(organisationUser, roles, createdBy: createdBy);
                _chmmUsers.Add(user);
            }
        }

        public void AddApplicant(List<CreateManufacturerUserDto> organisationUsers, List<ChmmRole> roles, string? createdBy = null)
        {
            var applicant = organisationUsers.SingleOrDefault(u => !u.IsResponsibleOfficer);
            if (applicant != null)
            {
                var user = new ChmmUser(applicant, roles, createdBy: createdBy);
                ApplicantId = user.Id;
                _chmmUsers.Add(user);
            }
        }

        public void UpdateUsers(List<EditManufacturerUserDto> organisationUsers)
        {
            foreach (var organisationUser in organisationUsers)
            {
                var user = ChmmUsers.Single(u => u.Id == organisationUser.Id);
                user.SetManufacturerUserDetails(organisationUser);
            }
        }

        public void ClearOrganisationStructureFiles()
        {
            _organisationStructureFiles = new List<OrganisationStructureFile>();
        }

        public void AddOrganisationStructureFile(AwsFileDto file, string? createdBy = null)
        {
            if (_organisationStructureFiles == null)
                _organisationStructureFiles = new List<OrganisationStructureFile>();

            _organisationStructureFiles.Add(new OrganisationStructureFile(file, createdBy));
        }

        public void AddAccountDecisionFile(AwsFileDto file)
        {
            if (_organisationDecisionFiles == null)
                _organisationDecisionFiles = new List<OrganisationDecisionFile>();

            _organisationDecisionFiles.Add(new OrganisationDecisionFile(file));
        }

        public void UpdateCreditContactDetails(string? name, string? email, string? telephoneNumber)
        {
            ContactName = name;
            ContactEmail = email;
            ContactTelephoneNumber = telephoneNumber;
        }

        public void UpdateDetails(string name, string? companiesHouseNumber)
        {
            Name = name;
            CompaniesHouseNumber = companiesHouseNumber;
        }

        public void UpdateFossilFuelSeller(bool isFossilFuelBoilerSeller)
        {
            IsFossilFuelBoilerSeller = isFossilFuelBoilerSeller;
        }

        public void UpdateHeatPumpDetails(string[]? heatPumpBrands)
        {
            HeatPumpBrands = heatPumpBrands;
        }

        public void UpdateSchemeParticipation(bool isNonSchemeParticipant)
        {
            IsNonSchemeParticipant = isNonSchemeParticipant;
        }

        public void UpdateApplicant(string name, string jobTitle, string telephoneNumber)
        {
            ChmmUsers.First(i => i.Id == ApplicantId).UpdateDetails(name, jobTitle, telephoneNumber);
        }

        /// <summary>
        /// This is currently a placeholder as you aren't allowed to remove users from the organisation
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void RemoveResponsibleOfficerIfExists()
        {
            // Do nothing if the applicant is already the responsible officer.
            if (ApplicantId == ResponsibleOfficerId)
            {
                return;
            }

            // If the applicant isn't the responsible officer and should be, remove them and change the id
            if (ApplicantId != ResponsibleOfficerId)
            {
                // I don't think we're allowed to do this
                return;
            }
        }

        /// <summary>
        /// If the responsible officer exists, they can be edited
        /// If the responsible officer doesn't exist, we aren't allowed to add them.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="jobTitle"></param>
        /// <param name="telephoneNumber"></param>
        public void UpdateSeniorResponsibleOfficerIfExists(string name, string jobTitle, string telephoneNumber)
        {
            // If the applicant is the responsible office and shouldn't be, add them.
            if (ApplicantId == ResponsibleOfficerId)
            {
                // I don't think we're allowed to do this
                return;
            }

            // If the applicant isn't the responsbile officer and still shoudldn't be
            if (ApplicantId != ResponsibleOfficerId)
            {
                // Update the responsible officer details
                var user = _chmmUsers.First(i => i.Id == ResponsibleOfficerId);
                user.UpdateDetails(name, jobTitle, telephoneNumber);
            }
        }

        /// <summary>
        /// If the legal correspondence address exists, remove it
        /// </summary>
        public void RemoveLegalCorrespondenceAddressIfExists(string legalAddressType)
        {
            LegalAddressType = legalAddressType;

            var address = Addresses.FirstOrDefault(i => i.Type == OrganisationAddressConstants.Type.LegalCorrespondenceAddress);
            if (address != null)
            {
                _addresses.Remove(address);
            }
        }

        /// <summary>
        /// If the legal correspondence address exists, update it, otherwise add it
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <param name="postcode"></param>
        public OrganisationAddress? UpdateLegalCorrespondenceAddress(string legalAddressType, string lineOne, string? lineTwo, string city, string? county, string postcode)
        {
            LegalAddressType = legalAddressType;

            var address = Addresses.FirstOrDefault(i => i.Type == OrganisationAddressConstants.Type.LegalCorrespondenceAddress);
            if (address != null)
            {
                address.UpdateDetails(lineOne, lineTwo, city, county, postcode);
            }
            return address;
        }

        /// <summary>
        /// Update the registered office adddress
        /// Based on the registration process, there should always be a registered office address
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <param name="postcode"></param>
        /// <exception cref="InvalidOperationException">if no office address exists</exception>
        public void UpdateRegisteredOfficeAddress(string lineOne, string? lineTwo, string city, string? county, string postcode)
        {
            Addresses.Single(x => x.Type == OrganisationAddressConstants.Type.OfficeAddress).UpdateDetails(lineOne, lineTwo, city, county, postcode);
        }

        public void UpdateOrganisationStructure(bool isOnBehalfOfGroup)
        {
            IsGroupRegistration = isOnBehalfOfGroup;
        }

        public void UpdateSeniorResponsibleOfficerAssigned(Guid userId)
        {
            ResponsibleOfficerId = userId;
        }
        #endregion
    }
}
