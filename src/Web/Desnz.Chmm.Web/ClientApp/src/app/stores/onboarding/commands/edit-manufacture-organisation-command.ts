import { EditOrganisationDto } from '../dtos/edit-organisation-dto';

export interface EditManufacturerOrganisationCommand {
  organisationDetails: EditOrganisationDto;
  organisationStructureFiles: FileList | null;
  comment: string | null;
}
