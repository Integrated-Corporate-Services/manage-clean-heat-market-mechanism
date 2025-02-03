import { Injectable } from "@angular/core";
import { AbstractControl, AsyncValidatorFn, FormControl, ValidationErrors } from "@angular/forms";
import { Observable, map, of, withLatestFrom } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class BalanceValidator {
  public init(balance: Observable<number | null>): AsyncValidatorFn {
    return (form: FormControl): Observable<ValidationErrors> => {
      // Get form controls
      let amount = form.get('amount');
      let addingOrRemoving = form.get('addingOrRemoving');

      // Check whether we need to validate balance removal
      if (amount == null
        || amount.value == null
        || isNaN(amount.value)
        || addingOrRemoving == null
        || addingOrRemoving.value == null
        || addingOrRemoving.value != 'Removing')
        return of({});

      // Compare value entered to their balance
      return of(Number(amount.value)).pipe(
        withLatestFrom(balance),
        map(([input, max]) => {
          if (max == null) return {};
          // Build error message if input is greater than balance
          return (input > max) ? {
            range: (() => {
              let maxString = new Intl.NumberFormat('en-GB').format(max);
              return `The maximum number of credits available to remove is ${maxString}. Enter ${maxString} or less.`;
            })()
          } : {};
        }));
    };
  }
}
