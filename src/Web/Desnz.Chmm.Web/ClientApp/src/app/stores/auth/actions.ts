import { createAction, props } from '@ngrx/store';
import { WhoAmI } from './services/authentication.service';

export const getWhoAmI = createAction('[Auth] Get who am I');
export const onGetWhoAmI = createAction(
  '[Auth] On get who am I',
  props<{ whoAmI: WhoAmI | null }>()
);
export const checkBypassStartPage = createAction(
  '[Auth] Check whether to bypass start page'
);
