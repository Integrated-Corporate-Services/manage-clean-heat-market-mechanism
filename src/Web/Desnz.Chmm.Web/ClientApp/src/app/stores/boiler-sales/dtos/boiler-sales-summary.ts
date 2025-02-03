import { boilerSalesStatus } from 'src/app/types/chmm-types';
import { AnnualBoilerSalesFileDto } from './annual-boiler-sales';
import { QuarterlyBoilerSalesFileDto } from './quarterly-boiler-sales';

interface BoilerSalesSummary {
  year: string;
  annual: BoilerSalesSummaryAnnual;
  quarters: BoilerSalesSummaryQuarter[];
  total: BoilerSalesSummaryTotal;
  schemeYearId: string;
}

interface BoilerSalesSummaryAnnual {
  gas: number | null;
  oil: number | null;
  status: boilerSalesStatus;
  verificationFiles: AnnualBoilerSalesFileDto[] | null;
  evidenceFiles: AnnualBoilerSalesFileDto[] | null;
}

interface BoilerSalesSummaryQuarter {
  id: string | null;
  name: string;
  dates: string;
  gas: number | null;
  oil: number | null;
  files: QuarterlyBoilerSalesFileDto[] | null;
  status: boilerSalesStatus;
  schemeYearQuarterId: string;
}

interface BoilerSalesSummaryTotal {
  gas: number;
  oil: number;
}

export {
  BoilerSalesSummary,
  BoilerSalesSummaryAnnual,
  BoilerSalesSummaryQuarter,
  BoilerSalesSummaryTotal,
};
