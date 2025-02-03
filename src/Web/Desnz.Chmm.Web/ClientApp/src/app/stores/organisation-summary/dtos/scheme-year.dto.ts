import { SchemeYearQuarterDto } from './scheme-year-quarter.dto';

export interface SchemeYearDto {
  id: string;
  previousSchemeYearId: string;
  name: string;
  year: string;
  startDate: string;
  endDate: string;
  tradingWindowStartDate: string;
  tradingWindowEndDate: string;
  creditGenerationWindowStartDate: string;
  creditGenerationWindowEndDate: string;
  surrenderDayDate: string;
  quarters: SchemeYearQuarterDto[];
  quarterOne: SchemeYearQuarterDto;
  quarterTwo: SchemeYearQuarterDto;
  quarterThree: SchemeYearQuarterDto;
  quarterFour: SchemeYearQuarterDto;
}
