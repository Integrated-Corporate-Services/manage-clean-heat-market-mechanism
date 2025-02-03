import { AppState } from '..';

export const selectAdministratorApprovalState = (state: AppState) =>
  state.administratorApprovalState;
