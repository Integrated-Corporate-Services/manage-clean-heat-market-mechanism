import { legalAddressType } from '../types/legal-address.type';
import { CreateAddressDto } from './create-address-dto';
import { CreateUserDetailsDto } from './create-user-details-dto';
import { CreditContactDetailsDto } from './credit-contact-details-dto';
import { ResponsibleUndertakingDto } from './responsible-undertaking-dto';

export interface CreateOrganisationDto {
  isOnBehalfOfGroup: boolean | null;
  responsibleUndertaking: ResponsibleUndertakingDto | null;
  addresses: CreateAddressDto[];
  isFossilFuelBoilerSeller: boolean | null;
  heatPumpBrands: string[];
  users: CreateUserDetailsDto[];
  creditContactDetails: CreditContactDetailsDto | null;
  isNonSchemeParticipant: boolean | null;
  legalAddressType: legalAddressType | null;
}
