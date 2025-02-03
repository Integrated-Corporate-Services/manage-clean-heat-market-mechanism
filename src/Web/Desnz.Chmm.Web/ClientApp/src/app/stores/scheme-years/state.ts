import { HttpState } from '../http-state';
import { SchemeYearDto } from '../organisation-summary/dtos/scheme-year.dto';

export interface SchemeYearsState {
  schemeYears: HttpState<SchemeYearDto[]>;
  selectedSchemeYear: SchemeYearDto | null;
}
