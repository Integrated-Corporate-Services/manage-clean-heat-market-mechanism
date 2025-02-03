import { FormControl } from '@angular/forms';
import { AddingOrRemoving } from './adding-or-removing.type';

export interface AmendObligationForm {
  addingOrRemoving: FormControl<AddingOrRemoving>;
  amount: FormControl<string>;
}
