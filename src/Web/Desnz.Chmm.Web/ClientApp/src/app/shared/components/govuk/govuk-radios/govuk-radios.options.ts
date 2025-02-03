export interface GovukRadios {
  id: string;
  label: string;
  hint: string;
  options: GovukRadiosOption[];
}

export interface GovukRadiosOption {
  value: number | string;
  display: string;
}
