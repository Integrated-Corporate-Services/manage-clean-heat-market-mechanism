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
  storeResponsibleOfficer,
} from 'src/app/stores/onboarding/actions';
import { UserDetails } from '../models/user-details';
import { Observable, Subscription } from 'rxjs';
import {
  selectOrganisationDetailsLoading,
  selectResponsibleOfficer,
} from 'src/app/stores/onboarding/selectors';
import { OnboardingFormMode } from '../models/mode';

export interface ResponsibleOfficerForm {
  isResponsibleOfficer: FormControl<string>;
}

@Component({
  selector: 'responsible-officer-form',
  templateUrl: './responsible-officer-form.component.html',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, UserDetailsFormComponent, AsyncPipe],
})
export class ResponsibleOfficerFormComponent implements OnInit, OnDestroy {
  @Input() mode: OnboardingFormMode = 'default';
  @Input() organisationId?: string;

  @ViewChild(UserDetailsFormComponent)
  userDetailsFormComponent?: UserDetailsFormComponent;

  form: FormGroup<ResponsibleOfficerForm>;
  errors: { [key: string]: string } = {};
  responsibleOfficer: UserDetails | null = null;

  loading$: Observable<boolean>;
  responsibleOfficer$: Observable<UserDetails | null>;
  subscription = Subscription.EMPTY;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider
  ) {
    this.responsibleOfficer$ = this.store.select(selectResponsibleOfficer);
    this.loading$ = this.store.select(selectOrganisationDetailsLoading);

    this.form = new FormGroup({
      isResponsibleOfficer: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
    });
  }

  ngOnInit() {
    this.subscription = this.responsibleOfficer$.subscribe(
      (responsibleOfficer) => {
        if (responsibleOfficer) {
          this.form.patchValue({
            isResponsibleOfficer: responsibleOfficer.isResponsibleOfficer!,
          });
          this.responsibleOfficer = responsibleOfficer;
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
        this.backLinkProvider.link = '/organisation/register/user-details';
    }
  }

  onInitUserDetailsForm() {
    if (this.responsibleOfficer && this.userDetailsFormComponent) {
      this.userDetailsFormComponent.form.patchValue({
        fullName: this.responsibleOfficer.fullName,
        jobTitle: this.responsibleOfficer.jobTitle,
        organisation: this.responsibleOfficer.organisation,
        emailAddress: this.responsibleOfficer.emailAddress,
        telephoneNumber: this.responsibleOfficer.telephoneNumber,
      });
    }
  }

  onProvideResponsibleOfficerDetails() {
    this.userDetailsFormComponent?.form.patchValue({
      fullName: '',
      jobTitle: '',
      organisation: '',
      emailAddress: '',
      telephoneNumber: '',
    });
  }

  onSubmit() {
    this.errors = {};
    if (this.form.valid || this.form.status === 'DISABLED') {
      let isResponsibleOfficer = this.form.getRawValue().isResponsibleOfficer;
      if (isResponsibleOfficer === 'Yes') {
        this.store.dispatch(
          storeResponsibleOfficer({
            mode: this.mode,
            userDetails: { isResponsibleOfficer: isResponsibleOfficer },
          })
        );
      } else if (this.userDetailsFormComponent) {
        this.userDetailsFormComponent.clearErrorMessages();
        let userDetails = this.userDetailsFormComponent.form;
        if (userDetails.valid) {
          let userDatailsFormValue = userDetails.getRawValue();
          this.store.dispatch(
            storeResponsibleOfficer({
              mode: this.mode,
              userDetails: {
                isResponsibleOfficer: isResponsibleOfficer,
                fullName: userDatailsFormValue.fullName,
                jobTitle: userDatailsFormValue.jobTitle,
                organisation: userDatailsFormValue.organisation,
                emailAddress: userDatailsFormValue.emailAddress,
                telephoneNumber: userDatailsFormValue.telephoneNumber,
              },
            })
          );
        } else {
          this.userDetailsFormComponent.setValidationErrorMessages();
        }
      }
    } else {
      this.setValidationErrorMessages();
    }
  }

  setValidationErrorMessages() {
    let validationErrors = this.form.controls.isResponsibleOfficer.errors;
    if (validationErrors && validationErrors['required']) {
      this.errors['isResponsibleOfficer'] =
        'Select if you are the Senior Responsible Officer for your organisation';
    }
  }

  ngOnDestroy() {
    this.backLinkProvider.clear();
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
