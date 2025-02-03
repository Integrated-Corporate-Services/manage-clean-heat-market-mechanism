import { createAction, props } from '@ngrx/store';
import { ContactOrganisationDto } from '../onboarding/dtos/contact-organisation-dto';

export const getContactableOrganisations = createAction(
  '[Contacts] Get contactable organisations',
  props<{ organisationId: string }>()
);

export const onGetContactableOrganisations = createAction(
  '[Contacts] On: Get contactable organisations',
  props<{ organisations: ContactOrganisationDto[] }>()
);

export const onGetContactableOrganisationsError = createAction(
  '[Contacts] On Error: Get contactable organisations',
  props<{ message: string }>()
);
