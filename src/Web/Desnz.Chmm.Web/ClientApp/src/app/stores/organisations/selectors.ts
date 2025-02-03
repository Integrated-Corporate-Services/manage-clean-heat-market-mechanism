import { AppState } from '..';

export const selectAvailableForTransfer = (state: AppState) =>
  state.availableForTransfer;
