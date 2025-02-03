export interface ViewOrganisationDto {
  id: string;
  name: string;
  status: string;
  licenceHolders: ViewOrganisationLicenceHolderDto[] | null | undefined;
  isNonSchemeParticipant: boolean;
}

export interface ViewOrganisationLicenceHolderDto {
  name: string;
}
