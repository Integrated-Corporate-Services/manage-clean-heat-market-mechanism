import { HttpState } from '../http-state';
import { OrganisationApprovalCommentsDto } from './dtos/organisation-approval-comments.dto';

export interface AdministratorApprovalState {
  approvalComments: HttpState<OrganisationApprovalCommentsDto>;
}
