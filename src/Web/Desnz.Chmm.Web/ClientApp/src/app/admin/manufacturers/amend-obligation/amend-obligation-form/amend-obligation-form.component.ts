import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BackLinkProvider } from 'src/app/navigation/back-link/back-link.provider';
import { FormControl, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { NgIf } from '@angular/common';
import { AmendObligationForm } from '../models/amend-obligation-form.model';
import { AddingOrRemoving } from '../models/adding-or-removing.type';
import { Store } from '@ngrx/store';
import { storeObligationAmendment } from 'src/app/stores/amend-obligation/actions';
import { ObligationAmendment } from '../models/amend-obligation.model';
import { Observable, Subscription } from 'rxjs';
import { selectEditObligationAmendment } from 'src/app/stores/amend-obligation/selectors';
import { selectObligationSummaryData } from 'src/app/stores/organisation-summary/selectors';
import { formConstants, numberFormat } from 'src/app/shared/constants';
import { getSchemeYearConfigurationSummary } from 'src/app/stores/organisation-summary/actions';

@Component({
  selector: 'amend-obligation-form',
  templateUrl: './amend-obligation-form.component.html',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgIf],
})
export class AmendObligationFormComponent implements OnInit, OnDestroy {
  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;

  form: FormGroup<AmendObligationForm>;
  errors: { [key: string]: string } = {};

  obligationAmendment$: Observable<ObligationAmendment | null>;
  subscription: Subscription = Subscription.EMPTY;
  finalObligations: number = 0;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider
  ) {
    this.obligationAmendment$ = this.store.select(
      selectEditObligationAmendment
    );

    backLinkProvider.clear();

    this.form = new FormGroup({
      addingOrRemoving: new FormControl<AddingOrRemoving>('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      amount: new FormControl('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.pattern(formConstants.validation.greaterThanZeroRegex),
          this.negativeBalanceAmountValidator()
        ],
      }),
    });
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    this.subscription = this.obligationAmendment$.subscribe(
      (obligationAmendment) => {
        if (obligationAmendment !== null) {
          this.form.patchValue({
            ...obligationAmendment,
          });
        }
      }
    );

    this.store.select(selectObligationSummaryData).subscribe((obligationSummary) => {
      if (obligationSummary !== null) {
        this.finalObligations = obligationSummary.finalObligations;
      } else {
        this.store.dispatch(getSchemeYearConfigurationSummary({
          organisationId: this.organisationId,
          schemeYearId: this.schemeYearId,
        }));
      }
    });

    const amountControl = this.form.get('amount');

    this.form.get('addingOrRemoving')?.valueChanges.subscribe(() => {
      amountControl?.updateValueAndValidity();
    });
  }

  negativeBalanceAmountValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const amount: number = control.value;
      const addingOrRemovingControl = this.form?.get('addingOrRemoving');
      const addingOrRemovingValue = addingOrRemovingControl ? addingOrRemovingControl.value as string : '';

      if (addingOrRemovingValue === 'Removing' && (this.finalObligations - amount < 0)) {
        return { customValidation: true };
      }
  
      return null;
    };
  }

  onSubmit() {
    this.errors = {};
    if (this.form.valid) {
      const obligationAmendment: ObligationAmendment = this.form.getRawValue();
      this.store.dispatch(
        storeObligationAmendment({
          organisationId: this.organisationId,
          schemeYearId: this.schemeYearId,
          obligationAmendment: obligationAmendment,
        })
      );
    } else {
      this.setValidationErrorMessages();
    }
  }

  setValidationErrorMessages() {
    Object.keys(this.form.controls).forEach((key) => {
      const validationErrors = this.form.get(key)?.errors;
      if (validationErrors && validationErrors['required']) {
        switch (key) {
          case 'amount':
            this.errors[key] = 'Enter the amount to adjust the obligation by';
            break;
          case 'addingOrRemoving':
            this.errors[key] = 'Select if you are adding or removing from the obligation';
            break;
        }
      }
      if (validationErrors && validationErrors['pattern']) {
        this.errors[key] = 'The amount you want to adjust the obligation by must be a whole number greater than zero';
      }
      if (validationErrors && validationErrors['customValidation']) {
        this.errors[key] = 'The maximum that you can remove from the target is ' + this.finalObligations + ' otherwise it will go below 0. Enter ' + this.finalObligations + ' or less.';
      }
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
