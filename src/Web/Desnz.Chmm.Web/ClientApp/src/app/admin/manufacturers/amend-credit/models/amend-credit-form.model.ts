import { FormControl } from '@angular/forms';
import { AddingOrRemoving } from './adding-or-removing.type';

export interface AmendCreditForm {
  addingOrRemoving: FormControl<AddingOrRemoving>;
  amount: FormControl<string>;
}
