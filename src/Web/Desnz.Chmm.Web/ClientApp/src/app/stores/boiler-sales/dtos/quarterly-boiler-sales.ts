import { boilerSalesStatus } from 'src/app/types/chmm-types';

export interface QuarterlyBoilerSalesDto {
  id: string;
  creationDate: string;
  createdBy: string;
  schemeYearQuarterId: string;
  organisationId: string;
  gas: number;
  oil: number;
  status: boilerSalesStatus;
  files: QuarterlyBoilerSalesFileDto[];
  changes: QuarterlyBoilerSalesChangeDto[];
}

export interface QuarterlyBoilerSalesChangeDto {
  quarterlyBoilerSalesId: string;
  oldGas: number | null;
  oldOil: number | null;
  oldStatus: string | null;
  newGas: number;
  newOil: number;
  newStatus: string;
  note: string;
}

export interface QuarterlyBoilerSalesFileDto {
  quarterlyBoilerSalesId: string;
  fileKey: string;
  fileName: string;
}
