import { EditAddressDto } from './edit-address-dto';
import { CreditContactDetailsDto } from './credit-contact-details-dto';
import { ResponsibleUndertakingDto } from './responsible-undertaking-dto';
import { EditUserDetailsDto } from './edit-user-details-dto';
import { legalAddressType } from '../types/legal-address.type';

export interface EditOrganisationDto {
  id: string | null;
  status?: string | null;
  isOnBehalfOfGroup: boolean | null;
  responsibleUndertaking: ResponsibleUndertakingDto | null;
  addresses: EditAddressDto[];
  isFossilFuelBoilerSeller: boolean | null;
  heatPumpBrands: string[];
  users: EditUserDetailsDto[];
  creditContactDetails: CreditContactDetailsDto | null;
  isNonSchemeParticipant: boolean | null;
  legalAddressType: legalAddressType | null;
}
