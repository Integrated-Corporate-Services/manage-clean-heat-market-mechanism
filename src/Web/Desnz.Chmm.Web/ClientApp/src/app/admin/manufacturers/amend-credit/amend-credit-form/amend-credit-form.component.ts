import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BackLinkProvider } from 'src/app/navigation/back-link/back-link.provider';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NgIf } from '@angular/common';
import { AmendCreditForm } from '../models/amend-credit-form.model';
import { AddingOrRemoving } from '../models/adding-or-removing.type';
import { Store } from '@ngrx/store';
import { storeCreditAmendment } from 'src/app/stores/credit/actions';
import { CreditAmendment } from '../models/amend-credit.model';
import { Observable, Subscription } from 'rxjs';
import {
  selectCreditBalance,
  selectEditCreditAmendment,
} from 'src/app/stores/credit/selectors';
import { formConstants } from 'src/app/shared/constants';
import { BalanceValidator } from './balance.validator';

@Component({
  selector: 'amend-credit-form',
  templateUrl: './amend-credit-form.component.html',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgIf],
  providers: [BalanceValidator],
})
export class AmendCreditFormComponent implements OnInit, OnDestroy {
  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;

  form: FormGroup<AmendCreditForm>;
  errors: { [key: string]: string } = {};

  creditBalance$: Observable<number | null>;

  creditAmendment$: Observable<CreditAmendment | null>;
  creditAmendmentSub: Subscription = Subscription.EMPTY;
  creditBalanceSub: Subscription = Subscription.EMPTY;

  constructor(
    private store: Store,
    backLinkProvider: BackLinkProvider,
    balanceValidator: BalanceValidator
  ) {
    this.creditBalance$ = this.store.select(selectCreditBalance);
    this.creditAmendment$ = this.store.select(selectEditCreditAmendment);

    backLinkProvider.clear();

    this.form = new FormGroup(
      {
        addingOrRemoving: new FormControl<AddingOrRemoving>('', {
          nonNullable: true,
          validators: [Validators.required],
        }),
        amount: new FormControl('', {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.pattern(
              formConstants.validation.positiveNumberOrHalfRegex
            ),
          ],
        }),
      },
      null,
      balanceValidator.init(this.creditBalance$)
    );

    this.creditBalanceSub = this.creditBalance$.subscribe();
  }

  ngOnInit() {
    this.creditAmendmentSub = this.creditAmendment$.subscribe(
      (creditAmendment) => {
        if (creditAmendment !== null) {
          this.form.patchValue({
            ...creditAmendment,
          });
        }
      }
    );
  }

  onSubmit() {
    this.errors = {};
    if (this.form.valid) {
      const creditAmendment: CreditAmendment = this.form.getRawValue();
      this.store.dispatch(
        storeCreditAmendment({
          organisationId: this.organisationId,
          schemeYearId: this.schemeYearId,
          creditAmendment: creditAmendment,
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
            this.errors[key] =
              'Enter the amount to adjust the credit balance by';
            break;
          case 'addingOrRemoving':
            this.errors[key] = 'Select if you are adding or removing credits';
            break;
        }
      }
      if (validationErrors && validationErrors['pattern']) {
        this.errors[key] =
          'The amount to adjust the credit balance by must be greater than zero and either a whole, or a half number';
      }
    });
    if (this.form.errors && this.form.errors['range']) {
      this.errors['amount'] = this.form.errors['range'];
    }
  }

  ngOnDestroy() {
    if (this.creditAmendmentSub) {
      this.creditAmendmentSub.unsubscribe();
    }
    if (this.creditBalanceSub) {
      this.creditBalanceSub.unsubscribe();
    }
  }
}
