export interface EditUserDetailsDto {
  id: string;
  name: string;
  jobTitle: string;
  organisation?: string | null;
  responsibleOfficerOrganisationName: string | null;
  email: string;
  telephoneNumber: string;
  isResponsibleOfficer: boolean;
}
