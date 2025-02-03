import { HttpState } from '../http-state';
import { OrganisationRejectionCommentsDto } from './dtos/organisation-rejection-comments.dto';

export interface AdministratorRejectionState {
  rejectionComments: HttpState<OrganisationRejectionCommentsDto>;
}
