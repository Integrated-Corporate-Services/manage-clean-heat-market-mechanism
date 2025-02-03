import { ObligationAmendment } from 'src/app/admin/manufacturers/amend-obligation/models/amend-obligation.model';

export interface EditObligationState {
  obligationAmendment: ObligationAmendment | null;
  errorMessage: string | null;
  loading: boolean;
}

export interface AmendObligationState {
  organisationId: string | null;
  editObligation: EditObligationState;
}
