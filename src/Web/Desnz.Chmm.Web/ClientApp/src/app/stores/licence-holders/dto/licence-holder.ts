export interface LicenceHolderDto {
  id: string;
  mcsManufacturerId: number;
  name: string;
  startDate: string;
  endDate: string | null;
  organisationName: string;
}
