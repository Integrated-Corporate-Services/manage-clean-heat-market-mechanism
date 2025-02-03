import { boilerSalesStatus } from 'src/app/types/chmm-types';

export interface AnnualBoilerSalesDto {
  id: string;
  creationDate: string;
  createdBy: string;
  schemeYearId: string;
  organisationId: string;
  gas: number;
  oil: number;
  status: boilerSalesStatus;
  files: AnnualBoilerSalesFileDto[];
  changes: AnnualBoilerSalesChangeDto[];
}

export interface AnnualBoilerSalesFileDto {
  annualBoilerSalesId: string;
  fileKey: string;
  fileName: string;
  type: 'Verification statement' | 'Supporting evidence';
}

export interface AnnualBoilerSalesChangeDto {
  annualBoilerSalesId: string;
  oldGas: number | null;
  oldOil: number | null;
  oldStatus: string | null;
  newGas: number;
  newOil: number;
  newStatus: string;
  note: string;
}
