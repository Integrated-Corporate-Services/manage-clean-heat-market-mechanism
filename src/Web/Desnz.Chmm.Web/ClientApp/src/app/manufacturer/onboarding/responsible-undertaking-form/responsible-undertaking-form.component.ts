import { Component, Input, OnDestroy, OnInit } from '@angular/core';
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
  storeResponsibleUndertaking,
} from 'src/app/stores/onboarding/actions';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { Observable, Subscription } from 'rxjs';
import { ResponsibleUndertaking } from '../models/responsible-undertaking';
import {
  selectOrganisationDetailsLoading,
  selectResponsibleUndertaking,
} from 'src/app/stores/onboarding/selectors';
import { OnboardingFormMode } from '../models/mode';

export interface ResponsibleUndertakingForm {
  name: FormControl<string>;
  hasCompaniesHouseNumber: FormControl<string>;
  companiesHouseNumber: FormControl<string | null>;
}

@Component({
  selector: 'responsible-undertaking-form',
  templateUrl: './responsible-undertaking-form.component.html',
  styleUrls: ['./responsible-undertaking-form.component.css'],
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, AsyncPipe],
})
export class ResponsibleUndertakingFormComponent implements OnInit, OnDestroy {
  @Input() mode: OnboardingFormMode = 'default';
  @Input() organisationId?: string;

  form: FormGroup<ResponsibleUndertakingForm>;
  errors: { [key: string]: string } = {};

  loading$: Observable<boolean>;
  responsibleUndertaking$: Observable<ResponsibleUndertaking | null>;
  subscription = Subscription.EMPTY;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider
  ) {
    this.responsibleUndertaking$ = this.store.select(
      selectResponsibleUndertaking
    );
    this.loading$ = this.store.select(selectOrganisationDetailsLoading);

    this.form = new FormGroup({
      name: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(160)],
      }),
      hasCompaniesHouseNumber: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      companiesHouseNumber: new FormControl<string | null>(null, {
        validators: [Validators.maxLength(100)],
      }),
    });
  }

  ngOnInit() {
    this.subscription = this.responsibleUndertaking$.subscribe(
      (responsibleUndertaking) => {
        if (responsibleUndertaking) {
          this.form.patchValue({ ...responsibleUndertaking });
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
          '/organisation/register/organisation-structure';
    }
  }

  onNoCompaniesHouseNo() {
    this.form.patchValue({ companiesHouseNumber: null });
  }

  onSubmit() {
    this.errors = {};
    this.validateCompaniesHouseNo();
    if (this.form.valid && !this.errors['companiesHouseNumber']) {
      let responsibleUndertaking = this.form.getRawValue();
      this.store.dispatch(
        storeResponsibleUndertaking({
          responsibleUndertaking: {
            name: responsibleUndertaking.name,
            hasCompaniesHouseNumber:
              responsibleUndertaking.hasCompaniesHouseNumber,
            companiesHouseNumber: responsibleUndertaking.companiesHouseNumber,
          },
          mode: this.mode,
        })
      );
    } else {
      this.setValidationErrorMessages();
    }
  }

  setValidationErrorMessages() {
    Object.keys(this.form.controls).forEach((key) => {
      let validationErrors = this.form.get(key)?.errors;
      if (validationErrors && validationErrors['required']) {
        switch (key) {
          case 'name':
            this.errors[key] =
              'Enter name of the organisation which is the Responsible Undertaking';
            break;
          case 'hasCompaniesHouseNumber':
            this.errors[key] =
              'Select if the Responsible Undertaking has a Companies House Number';
            break;
        }
      }
      if (validationErrors && validationErrors['maxlength']) {
        switch (key) {
          case 'name':
            this.errors[key] =
              'Name of the organisation which is the Responsible Undertaking cannot exceed 160 characters in length';
            break;
          case 'companiesHouseNumber':
            this.errors[key] =
              'Companies House Number cannot exceed 160 characters in length';
            break;
        }
      }
    });
  }

  validateCompaniesHouseNo() {
    if (
      this.form.controls.hasCompaniesHouseNumber.value === 'Yes' &&
      !this.form.controls.companiesHouseNumber.value
    ) {
      this.errors['companiesHouseNumber'] = 'Enter Companies House number';
    }
  }

  ngOnDestroy() {
    this.backLinkProvider.clear();
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
