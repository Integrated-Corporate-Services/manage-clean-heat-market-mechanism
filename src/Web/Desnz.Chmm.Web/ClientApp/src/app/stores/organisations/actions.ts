import { createAction, props } from '@ngrx/store';
import { Organisation } from 'src/app/navigation/models/organisation';

export const getOrganisationsAvailableForTransfer = createAction(
  '[Organisations] Get organisations available for transfer',
  props<{ organisationId: string }>()
);

export const onGetOrganisationsAvailableForTransfer = createAction(
  '[Organisations] On: Get organisations available for transfer',
  props<{ organisations: Organisation[] }>()
);

export const onGetOrganisationsAvailableForTransferError = createAction(
  '[Organisations] On Error: Get organisations available for transfer',
  props<{ message: string }>()
);
