import {
  AfterViewChecked,
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { NgIf, NgClass, AsyncPipe } from '@angular/common';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Store } from '@ngrx/store';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import {
  editAccount,
  storeUserDetails,
} from 'src/app/stores/onboarding/actions';
import { Observable, Subscription } from 'rxjs';
import { UserDetails } from '../models/user-details';
import {
  selectOrganisationDetailsLoading,
  selectUserDetails,
} from 'src/app/stores/onboarding/selectors';
import { OnboardingFormMode } from '../models/mode';

export interface UserDetailsForm {
  id: FormControl;
  fullName: FormControl<string>;
  jobTitle: FormControl<string>;
  organisation: FormControl<string | null>;
  emailAddress: FormControl<string>;
  telephoneNumber: FormControl<string>;
  confirmation: FormControl<boolean | null>;
}

@Component({
  selector: 'user-details-form',
  templateUrl: './user-details-form.component.html',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, NgClass, AsyncPipe],
})
export class UserDetailsFormComponent
  implements OnInit, AfterViewInit, AfterViewChecked, OnDestroy
{
  @Input() mode: OnboardingFormMode = 'default';
  @Input() organisationId?: string;
  @Input() showHeadingAndLegend?: boolean;
  @Input() showSubmitBtn?: boolean;
  @Input() disableEmail?: boolean = true;

  @Output() init = new EventEmitter();

  form: FormGroup<UserDetailsForm>;
  errors: { [key: string]: string } = {};

  loading$: Observable<boolean>;
  userDetails$: Observable<UserDetails | null>;
  subscription = Subscription.EMPTY;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    this.userDetails$ = this.store.select(selectUserDetails);
    this.loading$ = this.store.select(selectOrganisationDetailsLoading);

    this.form = new FormGroup({
      id: new FormControl(null),
      fullName: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(100)],
      }),
      jobTitle: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(100)],
      }),
      organisation: new FormControl<string | null>(null, {
        validators: [Validators.maxLength(100)],
      }),
      emailAddress: new FormControl('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.maxLength(100),
          Validators.email,
        ],
      }),
      telephoneNumber: new FormControl('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.pattern('^[0-9]*$'),
          Validators.maxLength(100),
        ],
      }),
      confirmation: new FormControl<boolean | null>(null),
    });
  }

  ngOnInit() {
    this.mode = this.mode ?? 'default';

    if (this.disableEmail && (this.mode == 'approve' || this.mode == 'edit')) {
      this.form.controls.emailAddress.disable();
    }

    this.subscription = this.userDetails$.subscribe((userDetails) => {
      if (userDetails) {
        this.form.patchValue({
          ...userDetails,
        });
      }
    });

    this.showHeadingAndLegend = this.showHeadingAndLegend ?? true;
    this.showSubmitBtn = this.showHeadingAndLegend ?? true;

    if (this.showHeadingAndLegend) {
      this.setBackLinkNavigation();
      this.form.controls.confirmation.addValidators([Validators.requiredTrue]);
    } else {
      this.form.controls.organisation.addValidators([Validators.required]);
    }
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
        this.backLinkProvider.link = '/organisation/register/heat-pump-brands';
    }
  }

  ngAfterViewInit() {
    this.init.emit();
  }

  ngAfterViewChecked(): void {
    this.changeDetectorRef.detectChanges();
  }

  onSubmit() {
    this.errors = {};
    if (this.form.valid) {
      let userDetails = this.form.getRawValue();
      this.store.dispatch(
        storeUserDetails({
          userDetails: userDetails,
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
          case 'fullName':
            this.errors[key] = 'Enter full name';
            break;
          case 'jobTitle':
            this.errors[key] = 'Enter job title';
            break;
          case 'organisation':
            this.errors[key] = 'Enter organisation';
            break;
          case 'emailAddress':
            this.errors[key] = 'Enter email address';
            break;
          case 'telephoneNumber':
            this.errors[key] = 'Enter telephone number';
            break;
          case 'confirmation':
            this.errors[
              key
            ] = `Confirm that you are directly employed by the organisation
            registering with the Clean Heat Market Mechanism
            scheme and/or have the authority to submit this
            registration.`;
            break;
        }
      }
      if (validationErrors && validationErrors['maxlength']) {
        switch (key) {
          case 'fullName':
            this.errors[key] =
              'Full name cannot exceed 100 characters in length';
            break;
          case 'jobTitle':
            this.errors[key] =
              'Job title cannot exceed 100 characters in length';
            break;
          case 'organisation':
            this.errors[key] =
              'Organisation cannot exceed 100 characters in length';
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
      if (validationErrors && validationErrors['email']) {
        this.errors[key] =
          'Enter an email address in the correct format, like name@example.com ';
      }
      if (validationErrors && validationErrors['pattern']) {
        this.errors[key] =
          'Enter a telephone number in the correct format, like 07700900982 or 4407700900982';
      }
    });
  }

  clearErrorMessages() {
    this.errors = {};
  }

  ngOnDestroy() {
    this.backLinkProvider.clear();
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
