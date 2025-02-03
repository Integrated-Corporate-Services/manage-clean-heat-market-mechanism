import { Component, Input, OnDestroy } from '@angular/core';
import { AsyncPipe, NgIf } from '@angular/common';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Store } from '@ngrx/store';
import {
  editAccount,
  storeIsFossilFuelBoilerSeller,
} from 'src/app/stores/onboarding/actions';
import { RouterLink } from '@angular/router';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { Observable, Subscription } from 'rxjs';
import {
  selectIsFossilFuelBoilerSeller,
  selectOrganisationDetailsLoading,
} from 'src/app/stores/onboarding/selectors';
import { OnboardingFormMode } from '../models/mode';

export interface SellFossilFuelBoilersForm {
  isFossilFuelBoilerSeller: FormControl<string>;
}

@Component({
  selector: 'sell-fossil-fuel-boilers-form',
  templateUrl: './sell-fossil-fuel-boilers-form.component.html',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, RouterLink, AsyncPipe],
})
export class SellFossilFuelBoilersFormComponent implements OnDestroy {
  @Input() mode: OnboardingFormMode = 'default';
  @Input() organisationId?: string;

  form: FormGroup<SellFossilFuelBoilersForm>;
  errors: { [key: string]: string } = {};

  loading$: Observable<boolean>;
  isFossilFuelBoilerSeller$: Observable<string | null>;
  subscription = Subscription.EMPTY;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider
  ) {
    this.isFossilFuelBoilerSeller$ = this.store.select(
      selectIsFossilFuelBoilerSeller
    );
    this.loading$ = this.store.select(selectOrganisationDetailsLoading);

    this.form = new FormGroup({
      isFossilFuelBoilerSeller: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
    });
  }

  ngOnInit() {
    this.subscription = this.isFossilFuelBoilerSeller$.subscribe(
      (isFossilFuelBoilerSeller) => {
        if (isFossilFuelBoilerSeller) {
          this.form.patchValue({ isFossilFuelBoilerSeller });
        }
      }
    );

    this.mode = this.mode ?? 'default';
    this.setBackLinkNavigation();
  }

  setBackLinkNavigation() {
    switch (this.mode) {
      case 'submit':
        this.backLinkProvider.link = '/organisation/register/check-answers';
        break;
      case 'approve':
        this.backLinkProvider.link = `/organisation/${this.organisationId}/edit/check-answers`;
        this.backLinkProvider.queryParams = { mode: this.mode };
        break;
      case 'edit':
        this.backLinkProvider.link = `/organisation/${this.organisationId}/edit/check-answers`;
        this.backLinkProvider.queryParams = { mode: this.mode };
        break;
      case 'default':
        this.backLinkProvider.link =
          '/organisation/register/legal-correspondence-address';
    }
  }

  onSubmit() {
    this.errors = {};
    if (this.form.valid) {
      let isFossilFuelBoilerSeller =
        this.form.getRawValue().isFossilFuelBoilerSeller;
      this.store.dispatch(
        storeIsFossilFuelBoilerSeller({
          isFossilFuelBoilerSeller: isFossilFuelBoilerSeller,
          mode: this.mode,
        })
      );
    } else {
      this.setValidationErrorMessages();
    }
  }

  setValidationErrorMessages() {
    let validationErrors = this.form.controls.isFossilFuelBoilerSeller.errors;
    if (validationErrors && validationErrors['required']) {
      this.errors['isFossilFuelBoilerSeller'] =
        'Select if you sell fossil fuel boilers';
    }
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
    this.backLinkProvider.clear();
  }
}
