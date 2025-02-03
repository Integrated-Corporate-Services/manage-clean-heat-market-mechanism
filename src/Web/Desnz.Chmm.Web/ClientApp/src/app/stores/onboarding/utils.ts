import { AccountApproval } from 'src/app/admin/approve-manufacturer/models/account-approval';
import { ApproveManufacturerOrganisationCommand } from './commands/approve-manufacture-organisation-command';
import { OrganisationStructure } from 'src/app/manufacturer/onboarding/models/organisation-structure';
import { ResponsibleUndertaking } from 'src/app/manufacturer/onboarding/models/responsible-undertaking';
import { Address } from 'src/app/manufacturer/onboarding/models/address';
import { UserDetails } from 'src/app/manufacturer/onboarding/models/user-details';
import { CreditContactDetails } from 'src/app/manufacturer/onboarding/models/contact-details';
import { HeatPumps } from 'src/app/manufacturer/onboarding/models/heat-pump-brands';
import { GetEditableOrganisationDto } from './dtos/get-editable-organisation-dto';
import { EditUserDetailsDto } from './dtos/edit-user-details-dto';
import { ResponsibleUndertakingDto } from './dtos/responsible-undertaking-dto';
import { EditAddressDto } from './dtos/edit-address-dto';
import { CreditContactDetailsDto } from './dtos/credit-contact-details-dto';
import { OrganisationDetails } from 'src/app/manufacturer/onboarding/models/organisation-details';
import { OnboardManufacturerCommand } from './commands/onboard-manufacturer-command';
import { CreateAddressDto } from './dtos/create-address-dto';
import { CreateUserDetailsDto } from './dtos/create-user-details-dto';
import { EditManufacturerOrganisationCommand } from './commands/edit-manufacture-organisation-command';
import { legalAddressType } from './types/legal-address.type';
import { AccountRejection } from 'src/app/admin/reject-manufacturer/models/account-rejection';

export function toOrganisationDetails(
  dto: GetEditableOrganisationDto
): OrganisationDetails {
  let responsibleUndertakingDto = dto.responsibleUndertaking!;
  let legalCorrespondenceAddressDto = dto.addresses.find(
    (a) => a.isUsedAsLegalCorrespondence
  )!;
  let addressDto = dto.addresses.find((a) => !a.isUsedAsLegalCorrespondence)!;
  let organisationStructure: OrganisationStructure = {
    isOnBehalfOfGroup: dto.isOnBehalfOfGroup ? 'Yes' : 'No',
    files: null,
    fileNames: dto.organisationStructureFileNames,
  };
  let responsibleOfficerDto = dto.users.find((u) => u.isResponsibleOfficer)!;
  let applicantDto = dto.users.find((u) => !u.isResponsibleOfficer);

  let responsibleUndertaking: ResponsibleUndertaking = {
    ...responsibleUndertakingDto,
    hasCompaniesHouseNumber: responsibleUndertakingDto.companiesHouseNumber
      ? 'Yes'
      : 'No',
  };
  let heatPumpBrands = dto.heatPumpBrands;

  let address: Address = {
    ...addressDto,
    isUsedAsLegalCorrespondence: 'No',
  };
  let legalCorrespondenceAddress: Address | null = null;

  switch (dto.legalAddressType) {
    case 'Use Registered Office':
      address.isUsedAsLegalCorrespondence = 'Yes';
      break;
    case 'Use Specified Address':
      address.isUsedAsLegalCorrespondence = 'No';
      break;
    case 'No Legal Correspondence Address':
      address.isUsedAsLegalCorrespondence = 'IsNonSchemeParticipant';
      break;
  }

  if (legalCorrespondenceAddressDto) {
    legalCorrespondenceAddress = {
      ...legalCorrespondenceAddressDto,
      isUsedAsLegalCorrespondence: 'No',
    };
  }

  let isFossilFuelBoilerSeller = dto.isFossilFuelBoilerSeller ? 'Yes' : 'No';
  let heatPumps: HeatPumps = {
    isHeatPumpSeller: heatPumpBrands.length > 0 ? 'Yes' : 'No',
    brands: heatPumpBrands,
  };
  let responsibleOfficer: UserDetails | null = null;
  let applicant: UserDetails | null = null;
  if (dto.users.length == 1) {
    responsibleOfficer = {
      ...responsibleOfficerDto,
      fullName: responsibleOfficerDto.name,
      organisation: responsibleOfficerDto.responsibleOfficerOrganisationName,
      emailAddress: responsibleOfficerDto.email,
      isResponsibleOfficer: null,
    };
    applicant = {
      ...responsibleOfficerDto,
      fullName: responsibleOfficerDto.name,
      organisation: responsibleOfficerDto.responsibleOfficerOrganisationName,
      emailAddress: responsibleOfficerDto.email,
      isResponsibleOfficer: 'Yes',
    };
  } else if (applicantDto) {
    responsibleOfficer = {
      ...responsibleOfficerDto,
      fullName: responsibleOfficerDto.name,
      organisation: responsibleOfficerDto.responsibleOfficerOrganisationName,
      emailAddress: responsibleOfficerDto.email,
      isResponsibleOfficer: null,
    };
    applicant = {
      ...applicantDto,
      fullName: applicantDto.name,
      organisation: applicantDto.responsibleOfficerOrganisationName,
      emailAddress: applicantDto.email,
      isResponsibleOfficer: 'No',
    };
  }
  let creditContactDetailsDto = dto.creditContactDetails!;
  let creditContactDetails: CreditContactDetails = {
    ...creditContactDetailsDto,
    hasOptedIn: creditContactDetailsDto.name ? 'Yes' : 'No',
    emailAddress: creditContactDetailsDto.email,
  };

  return {
    id: dto.id,
    status: dto.status,
    organisationStructure: organisationStructure,
    responsibleUndertaking: responsibleUndertaking,
    address: address,
    legalCorrespondenceAddress: legalCorrespondenceAddress,
    isFossilFuelBoilerSeller: isFossilFuelBoilerSeller,
    heatPumps: heatPumps,
    applicant: applicant,
    responsibleOfficer: responsibleOfficer,
    creditContactDetails: creditContactDetails,
    isNonSchemeParticipant: dto.isNonSchemeParticipant,
  };
}

export function toOnboardManufacturerCommand(
  organisationDetails: OrganisationDetails
): OnboardManufacturerCommand {
  let organisationStructure = organisationDetails.organisationStructure!;
  let responsibleUndertaking = organisationDetails.responsibleUndertaking!;
  let address = organisationDetails.address!;
  let legalCorrespondenceAddress =
    organisationDetails.legalCorrespondenceAddress;
  let isFossilFuelBoilerSeller = organisationDetails.isFossilFuelBoilerSeller!;
  let heatPumps = organisationDetails.heatPumps!;
  let applicant = organisationDetails.applicant!;
  let responsibleOfficer = organisationDetails.responsibleOfficer!;
  let creditContactDetails = organisationDetails.creditContactDetails!;

  let usersDto: CreateUserDetailsDto[] = [];
  let applicantDto: CreateUserDetailsDto = {
    name: applicant.fullName,
    jobTitle: applicant.jobTitle,
    responsibleOfficerOrganisationName: applicant.organisation,
    email: applicant.emailAddress,
    telephoneNumber: applicant.telephoneNumber,
    isResponsibleOfficer:
      applicant.isResponsibleOfficer === 'Yes' ? true : false,
  };
  usersDto.push(applicantDto);
  if (!applicantDto.isResponsibleOfficer) {
    usersDto.push({
      name: responsibleOfficer.fullName,
      jobTitle: responsibleOfficer.jobTitle,
      organisation: responsibleOfficer.organisation,
      responsibleOfficerOrganisationName: responsibleOfficer.organisation,
      email: responsibleOfficer.emailAddress,
      telephoneNumber: responsibleOfficer.telephoneNumber,
      isResponsibleOfficer: true,
    });
  }
  let responsibleUndertakingDto: ResponsibleUndertakingDto = {
    name: responsibleUndertaking.name,
    companiesHouseNumber:
      organisationDetails.responsibleUndertaking!.companiesHouseNumber,
  };

  let addressesDto: CreateAddressDto[] = [
    {
      lineOne: address.lineOne,
      lineTwo: address.lineTwo,
      city: address.city,
      county: address.county,
      postcode: address.postcode,
      isUsedAsLegalCorrespondence: false,
    },
  ];
  if (legalCorrespondenceAddress) {
    addressesDto.push({
      lineOne: legalCorrespondenceAddress.lineOne,
      lineTwo: legalCorrespondenceAddress.lineTwo,
      city: legalCorrespondenceAddress.city,
      county: legalCorrespondenceAddress.county,
      postcode: legalCorrespondenceAddress.postcode,
      isUsedAsLegalCorrespondence: true,
    });
  }
  let creditContactDetailsDto: CreditContactDetailsDto = {
    ...creditContactDetails,
    email: creditContactDetails.emailAddress,
  };

  return {
    organisationDetails: {
      isOnBehalfOfGroup:
        organisationStructure.isOnBehalfOfGroup === 'Yes' ? true : false,
      responsibleUndertaking: responsibleUndertakingDto,
      addresses: addressesDto,
      isFossilFuelBoilerSeller:
        isFossilFuelBoilerSeller === 'Yes' ? true : false,
      heatPumpBrands: heatPumps.brands,
      users: usersDto,
      creditContactDetails: creditContactDetailsDto,
      isNonSchemeParticipant: true,
      legalAddressType: deriveLegalAddressType(address, organisationDetails),
    },
    organisationStructureFiles: organisationStructure.files,
  };
}

export function toApproveManufacturerOrganisationCommand(
  organisationDetails: OrganisationDetails,
  accountApproval: AccountApproval
): ApproveManufacturerOrganisationCommand {
  return {
    organisationId: organisationDetails.id!,
    comment: accountApproval?.comments || '',
  };
}

export function toRejectManufacturerOrganisationCommand(
  organisationDetails: OrganisationDetails,
  accountRejection: AccountRejection
): ApproveManufacturerOrganisationCommand {
  return {
    organisationId: organisationDetails.id!,
    comment: accountRejection?.comments || '',
  };
}

export function toEditManufacturerOrganisationCommand(
  organisationDetails: OrganisationDetails,
  accountApproval: AccountApproval
): EditManufacturerOrganisationCommand {
  let organisationStructure = organisationDetails.organisationStructure!;
  let responsibleUndertaking = organisationDetails.responsibleUndertaking!;
  let address = organisationDetails.address!;
  let legalCorrespondenceAddress =
    organisationDetails.legalCorrespondenceAddress!;
  let isFossilFuelBoilerSeller = organisationDetails.isFossilFuelBoilerSeller!;
  let heatPumps = organisationDetails.heatPumps!;
  let applicant = organisationDetails.applicant!;
  let responsibleOfficer = organisationDetails.responsibleOfficer!;
  let creditContactDetails = organisationDetails.creditContactDetails!;

  let usersDto: EditUserDetailsDto[] = [];
  let applicantDto: EditUserDetailsDto = {
    id: applicant.id!,
    name: applicant.fullName,
    jobTitle: applicant.jobTitle,
    responsibleOfficerOrganisationName: applicant.organisation,
    email: applicant.emailAddress,
    telephoneNumber: applicant.telephoneNumber,
    isResponsibleOfficer:
      applicant.isResponsibleOfficer === 'Yes' ? true : false,
  };
  usersDto.push(applicantDto);
  if (!applicantDto.isResponsibleOfficer) {
    usersDto.push({
      id: responsibleOfficer.id!,
      name: responsibleOfficer.fullName,
      jobTitle: responsibleOfficer.jobTitle,
      organisation: responsibleOfficer.organisation,
      responsibleOfficerOrganisationName: responsibleOfficer.organisation,
      email: responsibleOfficer.emailAddress,
      telephoneNumber: responsibleOfficer.telephoneNumber,
      isResponsibleOfficer: true,
    });
  }
  let responsibleUndertakingDto: ResponsibleUndertakingDto = {
    name: responsibleUndertaking.name,
    companiesHouseNumber:
      organisationDetails.responsibleUndertaking!.companiesHouseNumber,
  };

  let addressesDto: EditAddressDto[] = [
    {
      id: address.id!,
      lineOne: address.lineOne,
      lineTwo: address.lineTwo,
      city: address.city,
      county: address.county,
      postcode: address.postcode,
      isUsedAsLegalCorrespondence: false,
    },
  ];
  if (legalCorrespondenceAddress) {
    addressesDto.push({
      id: legalCorrespondenceAddress.id!,
      lineOne: legalCorrespondenceAddress.lineOne,
      lineTwo: legalCorrespondenceAddress.lineTwo,
      city: legalCorrespondenceAddress.city,
      county: legalCorrespondenceAddress.county,
      postcode: legalCorrespondenceAddress.postcode,
      isUsedAsLegalCorrespondence: true,
    });
  }
  let creditContactDetailsDto: CreditContactDetailsDto = {
    name: creditContactDetails.name,
    telephoneNumber: creditContactDetails.telephoneNumber,
    email: creditContactDetails.emailAddress,
  };

  return {
    organisationDetails: {
      id: organisationDetails.id!,
      isOnBehalfOfGroup:
        organisationStructure.isOnBehalfOfGroup === 'Yes' ? true : false,
      responsibleUndertaking: responsibleUndertakingDto,
      addresses: addressesDto,
      isFossilFuelBoilerSeller:
        isFossilFuelBoilerSeller === 'Yes' ? true : false,
      heatPumpBrands: heatPumps.brands,
      users: usersDto,
      creditContactDetails: creditContactDetailsDto,
      isNonSchemeParticipant: organisationDetails.isNonSchemeParticipant,
      legalAddressType: deriveLegalAddressType(address, organisationDetails),
    },
    organisationStructureFiles: organisationStructure.files,
    comment: accountApproval?.comments,
  };
}

const deriveLegalAddressType = (
  address: Address,
  organisationDetails: OrganisationDetails
) => {
  let legalAddressType: legalAddressType =
    address.isUsedAsLegalCorrespondence === 'Yes'
      ? 'Use Registered Office'
      : 'Use Specified Address';
  if (organisationDetails.isNonSchemeParticipant) {
    legalAddressType = 'No Legal Correspondence Address';
  }
  return legalAddressType;
};
