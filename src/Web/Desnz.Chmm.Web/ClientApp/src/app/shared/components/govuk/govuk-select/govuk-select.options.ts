export interface GovukSelect {
  id: string;
  label: string;
  hint: string;
  options: GovukSelectOption[];
}

export interface GovukSelectOption {
  value: number | string;
  display: string;
}
