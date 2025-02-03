export interface GovukTable {
  caption: string;
  header: Partial<GovukTableHeaderCell>[];
  body: Partial<GovukTableBodyCell>[][];
}

export interface GovukTableHeaderCell {
  value: string;
}

export interface GovukTableBodyCell {
  value: string;
}
