import { createAction, props } from '@ngrx/store';
import { SchemeYearDto } from './dtos/scheme-year.dto';
import { SchemeYearConfigurationDto } from './dtos/scheme-year-configuration.dto';
import { UpdateSchemeYearConfigurationCommand } from './commands/update-scheme-year-configuration.command';

// Get scheme years

export const getSchemeYears = createAction(
  '[Scheme year configuration] Get scheme years'
);

export const onGetSchemeYears = createAction(
  '[Scheme year configuration] On: Get scheme years',
  props<{ schemeYears: SchemeYearDto[] }>()
);

export const onGetSchemeYearsError = createAction(
  '[Scheme year configuration] On Error: Get scheme years',
  props<{ message: string }>()
);

// Get scheme year

export const getSchemeYear = createAction(
  '[Scheme year configuration] Get scheme year',
  props<{ schemeYearId: string }>()
);

export const onGetSchemeYear = createAction(
  '[Scheme year configuration] On: Get scheme year',
  props<{ schemeYear: SchemeYearDto, schemeYearConfiguration: SchemeYearConfigurationDto }>()
);

export const onGetSchemeYearError = createAction(
  '[Scheme year configuration] On Error: Get scheme year',
  props<{ message: string }>()
);

// Edit scheme year configuration

export const storeSchemeYearConfiguration = createAction(
  '[Scheme year configuration] Store scheme year configuration',
  props<{ schemeYearConfiguration: UpdateSchemeYearConfigurationCommand }>()
);

export const saveSchemeYearConfiguration = createAction(
  '[Scheme year configuration] Save scheme year configuration'
); 
