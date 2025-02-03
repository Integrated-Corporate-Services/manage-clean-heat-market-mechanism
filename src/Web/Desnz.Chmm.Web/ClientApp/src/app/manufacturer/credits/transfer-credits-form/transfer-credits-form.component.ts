import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { formConstants } from 'src/app/shared/constants';
import { Observable, Subscription, filter } from 'rxjs';
import { HttpState } from 'src/app/stores/http-state';
import { Organisation } from 'src/app/navigation/models/organisation';
import { selectAvailableForTransfer } from 'src/app/stores/organisations/selectors';
import { getOrganisationsAvailableForTransfer } from 'src/app/stores/organisations/actions';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { CreditTransfer } from 'src/app/manufacturer/credits/models/credit-transfer';
import {
  clearCreditTransfer,
  getCreditBalance,
  storeCreditTransfer,
} from 'src/app/stores/credit/actions';
import { Router } from '@angular/router';
import {
  selectCreditBalance,
  selectCreditTransfer,
} from 'src/app/stores/credit/selectors';
import { selectIsAdmin } from 'src/app/stores/auth/selectors';

export interface TrasferCreditsForm {
  organisationId: FormControl<string>;
  organisationName: FormControl<string>;
  noOfCredits: FormControl<string>;
}

@Component({
  selector: 'transfer-credits-form',
  templateUrl: './transfer-credits-form.component.html',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, AsyncPipe, NgFor],
})
export class TransferCreditsFormComponent implements OnInit, OnDestroy {
  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;

  form: FormGroup<TrasferCreditsForm>;
  errors: { [key: string]: string } = {};

  isAdmin!: boolean;
  isAdminSub: Subscription = Subscription.EMPTY;

  creditBalance: number | null = null;
  creditBalanceSub: Subscription = Subscription.EMPTY;

  organisations$: Observable<HttpState<Organisation[]>>;
  creditTransferSub: Subscription = Subscription.EMPTY;

  constructor(private store: Store, private router: Router) {
    this.organisations$ = this.store.select(selectAvailableForTransfer);
    this.isAdminSub = this.store.select(selectIsAdmin).subscribe((isAdmin) => {
      this.isAdmin = isAdmin;
    });

    this.form = new FormGroup<TrasferCreditsForm>({
      organisationId: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      organisationName: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      noOfCredits: new FormControl('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.min(0.5),
          Validators.pattern(
            formConstants.validation.positiveNumberOrHalfRegex
          ),
        ],
      }),
    });
  }

  ngOnInit() {
    this.validateInputs();

    this.store.dispatch(
      getOrganisationsAvailableForTransfer({
        organisationId: this.organisationId,
      })
    );
    this.store.dispatch(
      getCreditBalance({
        organisationId: this.organisationId,
        schemeYearId: this.schemeYearId,
      })
    );

    this.creditBalanceSub = this.store
      .select(selectCreditBalance)
      .pipe(filter((creditBalance) => creditBalance !== null))
      .subscribe((creditBalance) => {
        this.creditBalance = creditBalance!;
        this.form.controls.noOfCredits.addValidators([
          Validators.max(this.creditBalance),
        ]);
      });

    this.creditTransferSub = this.store
      .select(selectCreditTransfer)
      .pipe(filter((creditTransfer) => creditTransfer !== null))
      .subscribe((creditTransfer) => {
        this.form.patchValue({
          ...creditTransfer,
        });
      });
  }

  onSelectOrganisation(organisationId: string, organisations: Organisation[]) {
    const organisation = organisations.find((o) => o.id === organisationId);
    if (!organisation) {
      throw Error(
        'Could not resolve the selected organisation from the organisations list'
      );
    }
    this.form.controls.organisationName.setValue(organisation.name!);
  }

  validateInputs() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }
  }

  onSubmit() {
    this.errors = {};
    if (this.form.valid) {
      const creditTransfer: CreditTransfer = this.form.getRawValue();
      this.store.dispatch(
        storeCreditTransfer({
          organisationId: this.organisationId,
          schemeYearId: this.schemeYearId,
          creditTransfer: creditTransfer,
        })
      );
    } else {
      this.setValidationErrorMessages();
    }
  }

  setValidationErrorMessages() {
    if (this.creditBalance === null || this.creditBalance === undefined) {
      throw TypeError('creditBalance cannot be null or undefined');
    }

    Object.keys(this.form.controls).forEach((key) => {
      const validationErrors = this.form.get(key)?.errors;
      if (validationErrors && validationErrors['required']) {
        switch (key) {
          case 'organisationId':
            this.errors[key] = 'Select an organisation';
            break;
          case 'noOfCredits':
            this.errors[key] = 'Enter the number of credits to transfer';
            break;
        }
      }
      if (validationErrors && validationErrors['pattern']) {
        this.errors[key] =
          'The number of credits to transfer must be a whole or half number greater than zero';
      }
      if (validationErrors && validationErrors['max']) {
        let maxAllowed = Math.floor(this.creditBalance!);
        this.errors[
          key
        ] = `The maximum number of credits you have available to transfer is ${maxAllowed}. Enter ${maxAllowed} or less`;
      }
      if (validationErrors && validationErrors['min']) {
        let minAllowed = 0.5;
        this.errors[
          key
        ] = `The minimum number of credits available to transfer is ${minAllowed}. Enter ${minAllowed} or more`;
      }
    });
  }

  onCancel() {
    this.store.dispatch(clearCreditTransfer());
    this.router.navigate([
      `/scheme-year/${this.schemeYearId}/organisation/${this.organisationId}/summary`,
    ]);
  }

  ngOnDestroy() {
    if (this.isAdminSub) {
      this.isAdminSub.unsubscribe();
    }
    if (this.creditBalanceSub) {
      this.creditBalanceSub.unsubscribe();
    }
    if (this.creditTransferSub) {
      this.creditTransferSub.unsubscribe();
    }
  }
}
