import { MultiFileUploadState } from 'src/app/shared/store/MultiFileUploadState';
import { HttpState } from '../http-state';
import { BoilerSalesSummary } from './dtos/boiler-sales-summary';

export interface SubmitAnnualState {
  gas: string | null;
  oil: string | null;
  verificationStatement: MultiFileUploadState;
  supportingEvidence: MultiFileUploadState;
  isEditing: boolean;
  loading: boolean;
  error?: string | null;
}

export interface SubmitQuarterlyState {
  gas: string | null;
  oil: string | null;
  isEditing: boolean;
  supportingEvidence: MultiFileUploadState;
  loading: boolean;
  error?: string | null;
}

export interface BoilerSalesState {
  boilerSalesSummary: HttpState<BoilerSalesSummary>;
  submitAnnual: SubmitAnnualState;
  approveAnnual: HttpState;
  submitQuarterly: SubmitQuarterlyState;
  editQuarterly: SubmitQuarterlyState;
}
