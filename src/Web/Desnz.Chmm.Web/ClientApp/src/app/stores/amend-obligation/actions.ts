import { createAction, props } from '@ngrx/store';
import { ObligationAmendment } from 'src/app/admin/manufacturers/amend-obligation/models/amend-obligation.model';

export const storeObligationAmendment = createAction(
  '[Obligation] Store obligation amendment',
  props<{
    organisationId: string;
    schemeYearId: string;
    obligationAmendment: ObligationAmendment;
  }>()
);

export const submitObligationAmendment = createAction(
  '[Obligation] Submit obligation amendment',
  props<{ schemeYearId: string }>()
);

export const onSubmitObligationAmendment = createAction(
  '[Obligation] On: Submit obligation amendment',
  props<{ schemeYearId: string }>()
);

export const clearObligationAmendment = createAction(
  '[Obligation] Clear obligation amendment'
);

export const onSubmitObligationAmendmentError = createAction(
  '[Obligation] On Error: Submit obligation amendment error',
  props<{ message: string }>()
);
