import { createAction, props } from '@ngrx/store';
import { SelectedLink } from './reducer';

export const toggleServiceNav = createAction(
  '[Navigation] Toggle service nav',
  props<{ show: boolean }>()
);

export const toggleSecondaryNav = createAction(
  '[Navigation] Toggle secondary nav',
  props<{ show: boolean }>()
);

export const selectLink = createAction(
  '[Navigation] Select link',
  props<{ selectedLink: SelectedLink }>()
);

export const toggleSchemeYearSelector = createAction(
  '[Navigation] Toggle scheme year selector',
  props<{ show: boolean }>()
);
