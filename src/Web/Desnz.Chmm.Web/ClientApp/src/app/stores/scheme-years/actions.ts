import { createAction, props } from '@ngrx/store';
import { SchemeYearDto } from '../organisation-summary/dtos/scheme-year.dto';

export const getSchemeYears = createAction('[Scheme years] Get scheme years');

export const getPreviousSchemeYear = createAction(
  '[Scheme years] Get previous scheme year',
  props<{ schemeYearId: string }>()
);


export const goToSchemeYear = createAction(
  '[Scheme years] Go to scheme year',
  props<{ schemeYear: SchemeYearDto; organisationId: string }>()
);

export const onGetSchemeYears = createAction(
  '[Scheme years] On: Get scheme years',
  props<{ schemeYears: SchemeYearDto[] }>()
);

export const onGetSchemeYearsError = createAction(
  '[Scheme years] On Error: Get scheme years',
  props<{ message: string }>()
);

export const setSelectedSchemeYear = createAction(
  '[Scheme years] Set selected scheme year',
  props<{ schemeYear: SchemeYearDto }>()
);
