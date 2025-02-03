import { HttpState } from '../http-state';
import { UpdateSchemeYearConfigurationCommand } from './commands/update-scheme-year-configuration.command';
import { SchemeYearConfigurationDto } from './dtos/scheme-year-configuration.dto';
import { SchemeYearDto } from './dtos/scheme-year.dto';

export interface SchemeYearConfigurationState {
  // List of all scheme years
  schemeYears: HttpState<SchemeYearDto[]>;
  // Scheme year selected for editing
  schemeYear: HttpState<SchemeYearDto>;
  // Existing configuration for scheme year being edited
  schemeYearConfiguration: HttpState<SchemeYearConfigurationDto>;
  // Command for updating scheme year
  updateSchemeYearConfigurationCommand: UpdateSchemeYearConfigurationCommand | null;
}
