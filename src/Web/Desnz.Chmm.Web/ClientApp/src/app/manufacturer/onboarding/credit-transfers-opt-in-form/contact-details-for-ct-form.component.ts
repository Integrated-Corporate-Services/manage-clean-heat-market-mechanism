import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AsyncPipe, NgIf } from '@angular/common';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Store } from '@ngrx/store';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { UserDetailsFormComponent } from '../user-details-form/user-details-form.component';
import {
  editAccount,
  storeContactDetailsForCt,
} from 'src/app/stores/onboarding/actions';
import { CreditContactDetails } from '../models/contact-details';
import { Observable, Subscription } from 'rxjs';
import {
  selectContactDetails,
  selectOrganisationDetailsLoading,
} from 'src/app/stores/onboarding/selectors';
import { OnboardingFormMode } from '../models/mode';

export interface ContactDetailsForCtForm {
  hasOptedIn: FormControl<string>;
  name: FormControl<string | null>;
  emailAddress: FormControl<string | null>;
  telephoneNumber: FormControl<string | null>;
}

@Component({
  selector: 'contact-details-for-ct-form',
  templateUrl: './contact-details-for-ct-form.component.html',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, UserDetailsFormComponent, AsyncPipe],
})
export class ContactDetailsForCtFormComponent implements OnInit, OnDestroy {
  @Input() mode: OnboardingFormMode = 'default';
  @Input() organisationId?: string;

  form: FormGroup<ContactDetailsForCtForm>;
  errors: { [key: string]: string | null } = {};

  loading$: Observable<boolean>;
  contactDetails$: Observable<CreditContactDetails | null>;
  subscription = Subscription.EMPTY;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider
  ) {
    this.contactDetails$ = this.store.select(selectContactDetails);
    this.loading$ = this.store.select(selectOrganisationDetailsLoading);

    this.form = new FormGroup({
      hasOptedIn: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      name: new FormControl<string | null>(null, {
        validators: [Validators.required, Validators.maxLength(100)],
      }),
      emailAddress: new FormControl<string | null>(null, {
        validators: [Validators.maxLength(100), Validators.email],
      }),
      telephoneNumber: new FormControl<string | null>(null, {
        validators: [Validators.maxLength(100), Validators.pattern('^[0-9]*$')],
      }),
    });
  }

  ngOnInit() {
    this.subscription = this.contactDetails$.subscribe((contactDetails) => {
      if (contactDetails) {
        this.form.patchValue({
          ...contactDetails,
        });
      }
    });

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
          '/organisation/register/responsible-officer-form';
    }
  }

  onNoOptIn() {
    this.form.patchValue({
      name: null,
      emailAddress: null,
      telephoneNumber: null,
    });
  }

  onSubmit() {
    this.setValidationErrorMessages();
    if (!this.hasErrors()) {
      let contactDetails = this.form.getRawValue();
      this.store.dispatch(
        storeContactDetailsForCt(this.mode, {
          hasOptedIn: contactDetails.hasOptedIn,
          name: contactDetails.name,
          emailAddress: contactDetails.emailAddress,
          telephoneNumber: contactDetails.telephoneNumber,
        })
      );
    }
  }

  setValidationErrorMessages() {
    let hasOptedIn = this.validateOptIn();
    if (hasOptedIn === 'Yes') {
      this.validateContactDetails();
    } else {
      this.clearContactDetailsErrors();
    }
  }

  validateOptIn() {
    let optInControl = this.form.controls.hasOptedIn;
    if (optInControl.errors && optInControl.errors['required']) {
      this.errors['hasOptedIn'] =
        'Select if you would like to opt-in for credit transfers';
    } else {
      this.errors['hasOptedIn'] = null;
    }
    return optInControl.value;
  }

  validateContactDetails() {
    Object.keys(this.form.controls).forEach((key) => {
      let validationErrors = this.form.get(key)?.errors;
      if (validationErrors && validationErrors['maxlength']) {
        switch (key) {
          case 'name':
            this.errors[key] =
              'Contact name or department cannot exceed 100 characters in length';
            break;
          case 'emailAddress':
            this.errors[key] =
              'Email address cannot exceed 100 characters in length';
            break;
          case 'telephoneNumber':
            this.errors[key] =
              'Telephone number cannot exceed 100 characters in length';
            break;
        }
      }
      if (validationErrors && validationErrors['required']) {
        switch (key) {
          case 'name':
            this.errors[key] = 'Enter contact name or department';
            break;
        }
      }
      if (validationErrors && validationErrors['email']) {
        this.errors[key] =
          'Enter an email address in the correct format, like name@example.com';
      }
      if (validationErrors && validationErrors['pattern']) {
        this.errors[key] =
          'Enter a telephone number in the correct format, like 07700900982 or 4407700900982';
      }
      if (validationErrors === null) {
        this.errors[key] = null;
      }
    });
    this.validateOneOfEmailOrPostcode();
  }

  validateOneOfEmailOrPostcode() {
    let email = this.form.controls.emailAddress.value;
    let telephoneNumber = this.form.controls.telephoneNumber.value;
    if (!email && !telephoneNumber) {
      this.errors['emailAddress'] = 'Enter email address or telephone number';
      this.errors['telephoneNumber'] =
        'Enter email address or telephone number';
    }
  }

  clearContactDetailsErrors() {
    this.errors['name'] = null;
    this.errors['emailAddress'] = null;
    this.errors['telephoneNumber'] = null;
  }

  hasErrors() {
    return !Object.keys(this.errors).every((key) => this.errors[key] == null);
  }

  ngOnDestroy() {
    this.backLinkProvider.clear();
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
