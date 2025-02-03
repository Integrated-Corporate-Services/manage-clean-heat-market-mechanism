export interface LicenceHolderLinkDto {
  id: string;
  licenceHolderId: string;
  licenceHolderName: string;
  organisationId: string;
  organisationName: string;
  startDate: string;
  endDate: string | null;
}
