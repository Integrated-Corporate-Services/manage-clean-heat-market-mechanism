import { createAction, props } from '@ngrx/store';
import { InstallationRequestSummaryDto } from './dto/installation-request-summary.dto';

export const getMcsDownloads = createAction(
  '[MCS Data] Get MCS downloads',
  props<{ schemeYearId: string | null }>()
);

export const onGetMcsDownloads = createAction(
  '[MCS Data] On: Get MCS downloads',
  props<{ downloads: InstallationRequestSummaryDto[] }>()
);

export const onError = createAction(
  '[MCS Data] On Error: On HTTP request error',
  props<{ message: string }>()
);
