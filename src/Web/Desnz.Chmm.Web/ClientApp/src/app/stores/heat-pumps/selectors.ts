import { AppState } from '..';

export const selectHeatPumpInstallations = (state: AppState) =>
  state.heatPumpInstallations;
