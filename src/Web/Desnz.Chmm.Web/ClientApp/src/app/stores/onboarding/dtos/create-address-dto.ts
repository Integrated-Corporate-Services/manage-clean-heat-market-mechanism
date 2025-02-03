export interface CreateAddressDto {
  lineOne: string;
  lineTwo: string | null;
  city: string;
  county: string | null;
  postcode: string;
  isUsedAsLegalCorrespondence: boolean;
}
