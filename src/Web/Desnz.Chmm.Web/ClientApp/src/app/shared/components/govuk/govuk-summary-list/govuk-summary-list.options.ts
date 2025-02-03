export interface GovukSummaryList {
  rows: Partial<GovukSummaryListRow>[];
}

export interface GovukSummaryListRow {
  title: string;
  value: string;
  link: string;
}
