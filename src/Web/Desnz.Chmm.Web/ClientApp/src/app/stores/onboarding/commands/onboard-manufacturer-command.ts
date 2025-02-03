import { CreateOrganisationDto } from '../dtos/create-organisation-dto';

export interface OnboardManufacturerCommand {
  organisationDetails: CreateOrganisationDto;
  organisationStructureFiles: FileList | null;
}
