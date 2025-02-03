import { Address } from './address';
import { CreditContactDetails } from './contact-details';
import { HeatPumps } from './heat-pump-brands';
import { OrganisationStructure } from './organisation-structure';
import { ResponsibleUndertaking } from './responsible-undertaking';
import { UserDetails } from './user-details';

export interface OrganisationDetails {
  id?: string | null;
  status?: string | null;
  organisationStructure: OrganisationStructure | null;
  responsibleUndertaking: ResponsibleUndertaking | null;
  address: Address | null;
  legalCorrespondenceAddress: Address | null;
  isFossilFuelBoilerSeller: string | null;
  heatPumps: HeatPumps | null;
  applicant: UserDetails | null;
  responsibleOfficer: UserDetails | null;
  creditContactDetails: CreditContactDetails | null;
  isNonSchemeParticipant: boolean | null;
}
