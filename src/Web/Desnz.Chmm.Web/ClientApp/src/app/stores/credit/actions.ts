import { createAction, props } from '@ngrx/store';
import { CreditTransfer } from '../../manufacturer/credits/models/credit-transfer';
import { CreditAmendment } from '../../admin/manufacturers/amend-credit/models/amend-credit.model';
import { TransferHistoryDto } from './dtos/transfer-history.dto';

export const clearCreditTransfer = createAction(
  '[Credit] Clear credit transfer'
);

export const getCreditBalance = createAction(
  '[Credit] Get credit balance',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onGetCreditBalance = createAction(
  '[Credit] On: Get credit balance',
  props<{ creditBalance: number }>()
);

export const storeCreditTransfer = createAction(
  '[Credit] Store credit transfer',
  props<{
    organisationId: string;
    schemeYearId: string;
    creditTransfer: CreditTransfer;
  }>()
);

export const submitCreditTransfer = createAction(
  '[Credit] Submit credit transfer',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onSubmitCreditTransfer = createAction(
  '[Credit] On: Submit credit transfer',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onError = createAction(
  '[Credit] On Error: On HTTP request error',
  props<{ message: string }>()
);

export const storeCreditAmendment = createAction(
  '[Credit] Store credit amendment',
  props<{
    organisationId: string;
    schemeYearId: string;
    creditAmendment: CreditAmendment;
  }>()
);

export const submitCreditAmendment = createAction(
  '[Credit] Submit credit amendment',
  props<{ schemeYearId: string }>()
);

export const onSubmitCreditAmendment = createAction(
  '[Credit] On: Submit credit amendment',
  props<{ schemeYearId: string }>()
);

export const clearCreditAmendment = createAction(
  '[Credit] Clear credit amendment'
);

export const onSubmitCreditAmendmentError = createAction(
  '[Credit] On Error: Submit credit amendment error',
  props<{ message: string }>()
);

export const getCreditTransferHistory = createAction(
  '[Credit] Get credit transfer history',
  props<{ organisationId: string; schemeYearId: string }>()
);

export const onGetCreditTransferHistory = createAction(
  '[Credit] On: Get credit transfer history',
  props<{ transferHistory: TransferHistoryDto }>()
);
