import * as moment from 'moment';
import { SchemeYearQuarterDto } from '../stores/organisation-summary/dtos/scheme-year-quarter.dto';

export const formatSchemeYearQuarterDate = (
  schemeYearQuarter: SchemeYearQuarterDto
): string => {
  return `${moment(schemeYearQuarter.startDate).format('DD MMM')} to ${moment(
    schemeYearQuarter.endDate
  ).format('DD MMM')}`;
};
