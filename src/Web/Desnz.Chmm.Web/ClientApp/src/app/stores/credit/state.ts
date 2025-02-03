import { CreditTransfer } from '../../manufacturer/credits/models/credit-transfer';
import { CreditAmendment } from '../../admin/manufacturers/amend-credit/models/amend-credit.model';
import { TransferHistoryDto } from './dtos/transfer-history.dto';

export interface EditCreditState {
  creditAmendment: CreditAmendment | null;
  errorMessage: string | null;
  loading: boolean;
}

export interface CreditState {
  organisationId: string | null;
  creditTransfer: CreditTransfer | null;
  transferHistory: TransferHistoryDto | null;
  creditBalance: number | null;
  isTransferWindowOpened: boolean | null;
  editCredit: EditCreditState;
  errorMessage: string | null;
  loading: boolean;
}
