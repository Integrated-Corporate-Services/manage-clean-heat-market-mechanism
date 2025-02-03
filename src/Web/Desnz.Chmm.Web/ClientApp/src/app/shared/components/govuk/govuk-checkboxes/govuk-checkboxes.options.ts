export interface GovukCheckboxes {
  id: string;
  label: string;
  hint: string;
  options: GovukCheckboxOption[];
}

export interface GovukCheckboxOption {
  value: number | string;
  display: string;
}
