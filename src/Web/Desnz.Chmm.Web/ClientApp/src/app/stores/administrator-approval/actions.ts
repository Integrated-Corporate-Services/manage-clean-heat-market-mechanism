import { createAction, props } from '@ngrx/store';
import { OrganisationApprovalCommentsDto } from './dtos/organisation-approval-comments.dto';

export const getApprovalComments = createAction(
  '[Administrator approval] Get approval comments',
  props<{
    organisationId: string;
  }>()
);

export const onGetApprovalComments = createAction(
  '[Administrator approval] On: Get approval comments',
  props<{
    organisationApprovalCommentsDto: OrganisationApprovalCommentsDto;
  }>()
);

export const onGetApprovalCommentsError = createAction(
  '[Administrator approval] On Error: Get approval comments',
  props<{
    error: string;
  }>()
);
