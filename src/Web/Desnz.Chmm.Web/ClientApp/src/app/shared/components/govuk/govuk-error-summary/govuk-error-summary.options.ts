export interface GovukErrorSummary {
  title: string;
  errors: GovukErrorSummaryError[];
}

export interface GovukErrorSummaryError {
  message: string;
}
