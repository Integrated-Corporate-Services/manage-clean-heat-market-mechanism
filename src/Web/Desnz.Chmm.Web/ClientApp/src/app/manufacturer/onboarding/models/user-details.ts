export interface UserDetails {
  id?: string;
  fullName: string;
  jobTitle: string;
  organisation: string | null;
  emailAddress: string;
  telephoneNumber: string;
  confirmation?: boolean | null;
  isResponsibleOfficer?: string | null;
}
