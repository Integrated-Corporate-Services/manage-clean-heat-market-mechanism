export type IsUsedAsLegalCorrespondence =
  | 'Yes'
  | 'No'
  | 'IsNonSchemeParticipant'
  | null;

export interface Address {
  id?: string;
  lineOne: string;
  lineTwo: string | null;
  city: string;
  county: string | null;
  postcode: string;
  isUsedAsLegalCorrespondence?: IsUsedAsLegalCorrespondence;
}
