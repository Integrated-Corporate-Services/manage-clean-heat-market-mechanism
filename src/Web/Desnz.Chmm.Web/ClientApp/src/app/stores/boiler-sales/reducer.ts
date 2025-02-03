import { createReducer, on } from '@ngrx/store';
import {
  BoilerSalesState,
  SubmitAnnualState,
  SubmitQuarterlyState,
} from './state';
import * as BoilerSalesActions from './actions';
import { BoilerSalesSummaryQuarter } from './dtos/boiler-sales-summary';
import { QuarterlyBoilerSalesDto } from './dtos/quarterly-boiler-sales';
import { MultiFileUploadState } from 'src/app/shared/store/MultiFileUploadState';
import * as _ from 'lodash';
import { formatSchemeYearQuarterDate } from 'src/app/shared/date-utils';

const defaultHttpState = {
  loading: false,
  errorMessage: null,
  data: null,
};

const defaultMultiFileUploadState: MultiFileUploadState = {
  files: null,
  fileNames: [],
  uploadingFiles: false,
  retrievingFiles: false,
  deletingFile: false,
  error: null,
};

const defaultSubmitAnnualState: SubmitAnnualState = {
  gas: null,
  oil: null,
  verificationStatement: defaultMultiFileUploadState,
  supportingEvidence: defaultMultiFileUploadState,
  isEditing: false,
  loading: false,
  error: null,
};

const defaultSubmitQuarterlyState: SubmitQuarterlyState = {
  gas: null,
  oil: null,
  isEditing: false,
  supportingEvidence: defaultMultiFileUploadState,
  loading: false,
  error: null,
};

const defaultState: BoilerSalesState = {
  boilerSalesSummary: defaultHttpState,
  submitAnnual: defaultSubmitAnnualState,
  approveAnnual: defaultHttpState,
  submitQuarterly: defaultSubmitQuarterlyState,
  editQuarterly: defaultSubmitQuarterlyState,
};

export const boilerSalesReducer = createReducer(
  defaultState,
  on(
    BoilerSalesActions.clearBoilerSalesSummary,
    (state, _): BoilerSalesState => {
      return {
        ...state,
        boilerSalesSummary: defaultHttpState,
      };
    }
  ),
  on(
    BoilerSalesActions.approveAnnualBoilerSales,
    (state, _): BoilerSalesState => {
      return {
        ...state,
        approveAnnual: {
          ...state.approveAnnual,
          loading: true,
          errorMessage: null,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onApproveAnnualBoilerSales,
    (state, _): BoilerSalesState => {
      return {
        ...state,
        approveAnnual: {
          ...state.approveAnnual,
          loading: false,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onApproveAnnualBoilerSalesError,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        approveAnnual: {
          ...state.approveAnnual,
          loading: false,
          errorMessage: action.error,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onGetAnnualBoilerSales,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        boilerSalesSummary: {
          ...state.boilerSalesSummary,
          loading: false,
          data: {
            year: action.schemeYear.year,
            schemeYearId: action.annualBoilerSales.schemeYearId,
            quarters: [],
            total: {
              gas: 0,
              oil: 0,
            },
            ...state.boilerSalesSummary.data,
            annual: {
              gas: action.annualBoilerSales.gas,
              oil: action.annualBoilerSales.oil,
              status: action.annualBoilerSales.status,
              evidenceFiles: _.filter(
                action.annualBoilerSales.files,
                (file) => {
                  return file.type === 'Supporting evidence';
                }
              ),
              verificationFiles: _.filter(
                action.annualBoilerSales.files,
                (file) => {
                  return file.type === 'Verification statement';
                }
              ),
            },
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onErrorGetAnnualBoilerSales,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        boilerSalesSummary: {
          ...state.boilerSalesSummary,
          loading: false,
          errorMessage: action.error,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onGetQuarterlyBoilerSales,
    (state, action): BoilerSalesState => {
      let defaultQuarter: QuarterlyBoilerSalesDto = {
        changes: [],
        createdBy: '',
        creationDate: '',
        files: [],
        gas: 0,
        id: '',
        oil: 0,
        organisationId: '',
        schemeYearQuarterId: '',
        status: 'N/A',
      };

      let quarterOneDto =
        action.quarterlyBoilerSales.length > 0
          ? action.quarterlyBoilerSales[0]
          : defaultQuarter;
      let quarterTwoDto =
        action.quarterlyBoilerSales.length > 1
          ? action.quarterlyBoilerSales[1]
          : defaultQuarter;
      let quarterThreeDto =
        action.quarterlyBoilerSales.length > 2
          ? action.quarterlyBoilerSales[2]
          : defaultQuarter;
      let quarterFourDto =
        action.quarterlyBoilerSales.length > 3
          ? action.quarterlyBoilerSales[3]
          : defaultQuarter;

      let transform = (
        dto: QuarterlyBoilerSalesDto,
        name: string,
        dates: string
      ): BoilerSalesSummaryQuarter => {
        return {
          id: dto.id,
          name,
          dates,
          gas: dto.gas,
          oil: dto.oil,
          status: dto.status,
          files: dto.files,
          schemeYearQuarterId: dto.schemeYearQuarterId,
        };
      };

      let quarterOne = transform(
        quarterOneDto,
        action.schemeYear.quarterOne.name,
        formatSchemeYearQuarterDate(action.schemeYear.quarterOne)
      );
      let quarterTwo = transform(
        quarterTwoDto,
        action.schemeYear.quarterTwo.name,
        formatSchemeYearQuarterDate(action.schemeYear.quarterTwo)
      );
      let quarterThree = transform(
        quarterThreeDto,
        action.schemeYear.quarterThree.name,
        formatSchemeYearQuarterDate(action.schemeYear.quarterThree)
      );
      let quarterFour = transform(
        quarterFourDto,
        action.schemeYear.quarterFour.name,
        formatSchemeYearQuarterDate(action.schemeYear.quarterFour)
      );

      return {
        ...state,
        boilerSalesSummary: {
          ...state.boilerSalesSummary,
          loading: false,
          data: {
            year: action.schemeYear.year,
            schemeYearId: action.schemeYear.id,
            annual: {
              gas: 0,
              oil: 0,
              status: 'N/A',
              evidenceFiles: null,
              verificationFiles: null,
            },
            ...state.boilerSalesSummary.data,
            quarters: [quarterOne, quarterTwo, quarterThree, quarterFour],
            total: {
              gas:
                quarterOneDto.gas +
                quarterTwoDto.gas +
                quarterThreeDto.gas +
                quarterFourDto.gas,
              oil:
                quarterOneDto.oil +
                quarterTwoDto.oil +
                quarterThreeDto.oil +
                quarterFourDto.oil,
            },
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onErrorGetQuarterlyBoilerSales,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        boilerSalesSummary: {
          ...state.boilerSalesSummary,
          loading: false,
          errorMessage: action.error,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.getAnnualVerificationStatement,
    (state, _): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          verificationStatement: {
            ...state.submitAnnual.verificationStatement,
            retrievingFiles: true,
            error: null,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.deleteAnnualVerificationStatement,
    (state, _): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          verificationStatement: {
            ...state.submitAnnual.verificationStatement,
            deletingFile: true,
            error: null,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onGetAnnualVerificationStatement,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          verificationStatement: {
            ...state.submitAnnual.verificationStatement,
            retrievingFiles: false,
            uploadingFiles: false,
            fileNames: action.fileNames,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onAnnualVerificationStatementError,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          verificationStatement: {
            ...state.submitAnnual.verificationStatement,
            retrievingFiles: false,
            uploadingFiles: false,
            error: action.error,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.getAnnualSupportingEvidence,
    (state): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          supportingEvidence: {
            ...state.submitAnnual.supportingEvidence,
            retrievingFiles: true,
            error: null,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.deleteAnnualSupportingEvidence,
    (state): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          supportingEvidence: {
            ...state.submitAnnual.supportingEvidence,
            deletingFile: true,
            error: null,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onGetAnnualSupportingEvidence,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          supportingEvidence: {
            ...state.submitAnnual.supportingEvidence,
            retrievingFiles: false,
            uploadingFiles: false,
            fileNames: action.fileNames,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onAnnualSupportingEvidenceError,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          supportingEvidence: {
            ...state.submitAnnual.supportingEvidence,
            retrievingFiles: false,
            uploadingFiles: false,
            error: action.error,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.uploadAnnualVerificationStatement,
    (state): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          verificationStatement: {
            ...state.submitAnnual.verificationStatement,
            uploadingFiles: true,
            error: null,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.uploadAnnualSupportingEvidence,
    (state): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          supportingEvidence: {
            ...state.submitAnnual.supportingEvidence,
            uploadingFiles: true,
            error: null,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.storeAnnualBoilerSales,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          gas: action.boilerSales.gas,
          oil: action.boilerSales.oil,
          isEditing: action.isEditing,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.submitAnnualBoilerSales,
    (state, _): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          loading: true,
          error: null,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onSubmitAnnualBoilerSales,
    (state, _): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          loading: false,
          oil: null,
          gas: null,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onSubmitAnnualBoilerSalesError,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        submitAnnual: {
          ...state.submitAnnual,
          loading: false,
          error: action.error,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.getQuarterlySupportingEvidence,
    (state): BoilerSalesState => {
      return {
        ...state,
        submitQuarterly: {
          ...state.submitQuarterly,
          supportingEvidence: {
            ...state.submitQuarterly.supportingEvidence,
            retrievingFiles: true,
            error: null,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.deleteQuarterlySupportingEvidence,
    (state): BoilerSalesState => {
      return {
        ...state,
        submitQuarterly: {
          ...state.submitQuarterly,
          supportingEvidence: {
            ...state.submitQuarterly.supportingEvidence,
            deletingFile: true,
            error: null,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onGetQuarterlySupportingEvidence,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        submitQuarterly: {
          ...state.submitQuarterly,
          supportingEvidence: {
            ...state.submitQuarterly.supportingEvidence,
            retrievingFiles: false,
            uploadingFiles: false,
            fileNames: action.fileNames,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onGetQuarterlySupportingEvidenceError,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        submitQuarterly: {
          ...state.submitQuarterly,
          supportingEvidence: {
            ...state.submitQuarterly.supportingEvidence,
            retrievingFiles: false,
            uploadingFiles: false,
            error: action.error,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.uploadQuarterlySupportingEvidence,
    (state): BoilerSalesState => {
      return {
        ...state,
        submitQuarterly: {
          ...state.submitQuarterly,
          supportingEvidence: {
            ...state.submitQuarterly.supportingEvidence,
            uploadingFiles: true,
            error: null,
          },
        },
      };
    }
  ),
  on(
    BoilerSalesActions.storeQuarterlyBoilerSales,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        submitQuarterly: {
          ...state.submitQuarterly,
          gas: action.boilerSales.gas,
          oil: action.boilerSales.oil,
          isEditing: action.isEditing,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.submitQuarterlyBoilerSales,
    (state, _): BoilerSalesState => {
      return {
        ...state,
        submitQuarterly: {
          ...state.submitQuarterly,
          loading: true,
          error: null,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onSubmitQuarterlyBoilerSales,
    (state, _): BoilerSalesState => {
      return {
        ...state,
        submitQuarterly: {
          ...state.submitQuarterly,
          loading: false,
          oil: null,
          gas: null,
        },
      };
    }
  ),
  on(
    BoilerSalesActions.onSubmitQuarterlyBoilerSalesError,
    (state, action): BoilerSalesState => {
      return {
        ...state,
        submitQuarterly: {
          ...state.submitQuarterly,
          loading: false,
          error: action.error,
        },
      };
    }
  )
);
