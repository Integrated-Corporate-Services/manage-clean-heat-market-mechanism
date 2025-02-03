using AutoMapper;
using Desnz.Chmm.Identity.Api.Constants;
using Desnz.Chmm.Identity.Common.Dtos;
using Desnz.Chmm.Identity.Api.Entities;
using Desnz.Chmm.Identity.Common.Dtos.OrganisationAddress;
using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Common.Dtos;
using Desnz.Chmm.Identity.Common.Dtos.LicenceHolder;

namespace Desnz.Chmm.Identity.Api.AutoMapper
{
    public class IdentityAutoMapperProfile : Profile
    {
        public IdentityAutoMapperProfile()
        {
            CreateMap<ChmmRole, RoleDto>();

            CreateMap<ChmmUser, ChmmUserDto>();

            CreateMap<ChmmUser, ViewManufacturerUserDto>();

            CreateMap<ChmmUser, EditManufacturerUserDto>()
                .ForMember(e => e.IsResponsibleOfficer, c => c.Ignore());

            CreateMap<Organisation, ViewOrganisationDto>()
                .ForMember(e => e.LicenceHolders, c => c.Ignore());

            CreateMap<LicenceHolder, LicenceHolderDto>();

            CreateMap<LicenceHolder, ViewOrganisationLicenceHolderDto>();

            CreateMap<OrganisationStructureFile, AwsFileDto>();

            CreateMap<OrganisationDecisionFile, AwsFileDto>();

            CreateMap<Organisation, GetEditableOrganisationDto>()
                .ForMember(e => e.IsOnBehalfOfGroup, c => c.MapFrom(o => o.IsGroupRegistration))
                .ForMember(e => e.ResponsibleUndertaking, c => c.MapFrom(o => new ResponsibleUndertakingDto()
                {
                    Name = o.Name,
                    CompaniesHouseNumber = o.CompaniesHouseNumber
                }))
                .ForMember(o => o.CreditContactDetails, s => s.MapFrom(o => new CreditContactDetailsDto()
                {
                    Name = o.ContactName,
                    Email = o.ContactEmail,
                    TelephoneNumber = o.ContactTelephoneNumber,
                }))
                .ForMember(o => o.OrganisationStructureFileNames, p => p.MapFrom(f => f.OrganisationStructureFiles.Select(i => i.FileName)))
                .ForMember(e => e.Users, c => c.MapFrom(o => o.ChmmUsers.Where(u => u.Id == o.ApplicantId || u.Id == o.ResponsibleOfficerId)));

            CreateMap<Organisation, EditOrganisationDto>()
                .ForMember(e => e.IsOnBehalfOfGroup, c => c.MapFrom(o => o.IsGroupRegistration))
                .ForMember(e => e.ResponsibleUndertaking, c => c.MapFrom(o => new ResponsibleUndertakingDto()
                {
                    Name = o.Name,
                    CompaniesHouseNumber = o.CompaniesHouseNumber
                }))
                .ForMember(o => o.CreditContactDetails, s => s.MapFrom(o => new CreditContactDetailsDto()
                {
                    Name = o.ContactName,
                    Email = o.ContactEmail,
                    TelephoneNumber = o.ContactTelephoneNumber,
                }))
                .ForMember(e => e.Users, c => c.MapFrom(o => o.ChmmUsers.Where(u => u.Id == o.ApplicantId || u.Id == o.ResponsibleOfficerId)));

            CreateMap<OrganisationAddress, EditOrganisationAddressDto>()
                .ForMember(e => e.IsUsedAsLegalCorrespondence, c => c.MapFrom(a => a.Type == OrganisationAddressConstants.Type.LegalCorrespondenceAddress));

            CreateMap<LicenceHolderInformationDto, LicenceHolder>()
                .ForMember(e => e.McsManufacturerId, c => c.MapFrom(s => s.McsManufacturerId))
                .ForMember(e => e.Name, c => c.MapFrom(s => s.McsManufacturerName))
                .ForMember(e => e.Id, c => c.Ignore())
                .ForMember(e => e.CreationDate, c => c.Ignore())
                .ForMember(e => e.CreatedBy, c => c.Ignore())
                .ForMember(e => e.LicenceHolderLinks, c => c.Ignore());
        }
    }
}
