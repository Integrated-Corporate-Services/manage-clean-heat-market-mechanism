export interface SubmitOnboardingDto {
  organisationDetailsJson: OrganisationDetailsDto;
  organisationStructureFiles: FileList | null;
}

export interface OrganisationDetailsDto {
  id?: string | null;
  status?: string | null;
  isOnBehalfOfGroup: boolean | null;
  responsibleUndertaking: ResponsibleUndertakingDto | null;
  address: AddressDto | null;
  legalCorrespondenceAddress: AddressDto | null;
  isFossilFuelBoilerSeller: boolean | null;
  heatPumps: HeatPumpsDto | null;
  applicant: UserDetailsDto | null;
  responsibleOfficer: UserDetailsDto | null;
  creditContactDetails: CreditContactDetailsDto | null;
}

export interface ResponsibleUndertakingDto {
  name: string;
  hasCompaniesHouseNumber: boolean;
  companiesHouseNumber: string | null;
}

export interface AddressDto {
  lineOne: string;
  lineTwo: string | null;
  city: string;
  county: string | null;
  postcode: string;
  isUsedAsLegalCorrespondence?: boolean | null;
}

export interface HeatPumpsDto {
  isHeatPumpSeller: boolean;
  brands: string[];
}

export interface UserDetailsDto {
  name: string;
  jobTitle: string;
  organisation?: string | null;
  responsibleOfficerOrganisationName: string | null;
  email: string;
  telephoneNumber: string;
  isResponsibleOfficer?: boolean | null;
}

export interface CreditContactDetailsDto {
  hasOptedIn: boolean;
  name: string | null;
  emailAddress: string | null;
  telephoneNumber: string | null;
}
