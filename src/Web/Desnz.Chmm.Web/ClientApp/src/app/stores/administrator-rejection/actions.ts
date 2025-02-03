import { createAction, props } from '@ngrx/store';
import { OrganisationRejectionCommentsDto } from './dtos/organisation-rejection-comments.dto';

export const getRejectionComments = createAction(
  '[Administrator rejection] Get rejection comments',
  props<{
    organisationId: string;
  }>()
);

export const onGetRejectionComments = createAction(
  '[Administrator rejection] On: Get rejection comments',
  props<{
    organisationRejectionCommentsDto: OrganisationRejectionCommentsDto;
  }>()
);

export const onGetRejectionCommentsError = createAction(
  '[Administrator rejection] On Error: Get rejection comments',
  props<{
    error: string;
  }>()
);
