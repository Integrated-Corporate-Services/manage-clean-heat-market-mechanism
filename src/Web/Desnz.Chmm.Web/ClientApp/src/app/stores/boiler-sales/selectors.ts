import { createSelector } from '@ngrx/store';
import { BoilerSalesState } from './state';
import { AppState } from '..';
import { MultiFileUploadState } from 'src/app/shared/store/MultiFileUploadState';

export const selectBoilerSalesState = (state: AppState) => state.boilerSales;

// export const selectBoilerSalesOverallSummary = createSelector(
//   selectBoilerSalesState,
//   (state: BoilerSalesState) => state.overallSummary
// );

export const selectApproveAnnualState = createSelector(
  selectBoilerSalesState,
  (state: BoilerSalesState) => state.approveAnnual
);

export const selectBoilerSalesSummary = createSelector(
  selectBoilerSalesState,
  (state: BoilerSalesState) => state.boilerSalesSummary
);

export const selectSubmitAnnual = createSelector(
  selectBoilerSalesState,
  (state: BoilerSalesState) => state.submitAnnual
);

export const selectSubmitAnnualVerificationStatement = createSelector(
  selectBoilerSalesState,
  (state: BoilerSalesState) => state.submitAnnual.verificationStatement
);

export const selectSubmitAnnualSupportingEvidence = createSelector(
  selectBoilerSalesState,
  (state: BoilerSalesState) => state.submitAnnual.supportingEvidence
);

export const selectSubmitAnnualLoading = createSelector(
  selectSubmitAnnualVerificationStatement,
  selectSubmitAnnualSupportingEvidence,
  (
    verificationStatement: MultiFileUploadState,
    supportingEvidence: MultiFileUploadState
  ) => {
    return (
      verificationStatement.uploadingFiles ||
      verificationStatement.deletingFile ||
      verificationStatement.retrievingFiles ||
      supportingEvidence.uploadingFiles ||
      supportingEvidence.deletingFile ||
      supportingEvidence.retrievingFiles
    );
  }
);

export const selectSubmitQuarterly = createSelector(
  selectBoilerSalesState,
  (state: BoilerSalesState) => state.submitQuarterly
);

export const selectSubmitQuarterlySupportingEvidence = createSelector(
  selectBoilerSalesState,
  (state: BoilerSalesState) => state.submitQuarterly.supportingEvidence
);

export const selectQuarterlyBoilerSales = createSelector(
  selectBoilerSalesState,
  (state: BoilerSalesState) => state.submitQuarterly
);

export const selectEditQuarterlyBoilerSales = createSelector(
  selectBoilerSalesState,
  (state) => {
    return {
      gas: state.editQuarterly.gas,
      oil: state.editQuarterly.oil
    }
  }
);

export const selectSubmitQuarterlyLoading = createSelector(
  selectSubmitQuarterlySupportingEvidence,
  (supportingEvidence: MultiFileUploadState) => {
    return (
      supportingEvidence.uploadingFiles ||
      supportingEvidence.deletingFile ||
      supportingEvidence.retrievingFiles
    );
  }
);

export const selectBoilerSalesQuarters = createSelector(
  selectBoilerSalesSummary,
  (summary) => {
    return summary.data?.quarters!;
  }
);
