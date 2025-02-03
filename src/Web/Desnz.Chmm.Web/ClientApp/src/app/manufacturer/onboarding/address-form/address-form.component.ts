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
import { editAccount, storeAddress } from 'src/app/stores/onboarding/actions';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { Observable, Subscription } from 'rxjs';
import { Address } from '../models/address';
import {
  selectAddress,
  selectOrganisationDetailsLoading,
} from 'src/app/stores/onboarding/selectors';
import { OnboardingFormMode } from '../models/mode';

export interface AddressForm {
  id: FormControl;
  lineOne: FormControl<string>;
  lineTwo: FormControl<string | null>;
  city: FormControl<string>;
  county: FormControl<string | null>;
  postcode: FormControl<string>;
}

@Component({
  selector: 'address-form',
  templateUrl: './address-form.component.html',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, NgClass, AsyncPipe],
})
export class AddressFormComponent
  implements OnInit, AfterViewInit, AfterViewChecked, OnDestroy
{
  @Input() mode: OnboardingFormMode = 'default';
  @Input() organisationId?: string;
  @Input() showHeadingAndLegend?: boolean;
  @Input() showSubmitBtn?: boolean;

  @Output() init = new EventEmitter();

  form: FormGroup<AddressForm>;
  errors: { [key: string]: string } = {};
  postcodeRegex = new RegExp(
    '^[a-zA-Z]{1,2}[0-9][a-zA-Z0-9]? [0-9][a-zA-Z]{2}$'
  );

  loading$: Observable<boolean>;
  address$: Observable<Address | null>;
  subscription = Subscription.EMPTY;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    this.address$ = this.store.select(selectAddress);
    this.loading$ = this.store.select(selectOrganisationDetailsLoading);

    this.form = new FormGroup({
      id: new FormControl(null),
      lineOne: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(255)],
      }),
      lineTwo: new FormControl<string | null>(null, {
        validators: [Validators.maxLength(255)],
      }),
      city: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(100)],
      }),
      county: new FormControl<string | null>(null, {
        validators: [Validators.maxLength(100)],
      }),
      postcode: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(100)],
      }),
    });
  }

  ngOnInit() {
    this.subscription = this.address$.subscribe((address) => {
      if (address) {
        this.form.patchValue({
          ...address,
        });
      }
    });

    this.showHeadingAndLegend = this.showHeadingAndLegend ?? true;
    this.showSubmitBtn = this.showHeadingAndLegend ?? true;
    this.mode = this.mode ?? 'default';

    if (this.showHeadingAndLegend) {
      this.setBackLinkNavigation();
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
        this.backLinkProvider.link =
          '/organisation/register/responsible-undertaking';
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
      let address = this.form.getRawValue();
      this.store.dispatch(
        storeAddress({
          address: address,
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
          case 'lineOne':
            this.errors[key] = 'Enter address line one';
            break;
          case 'lineTwo':
            this.errors[key] = 'Enter address line two';
            break;
          case 'city':
            this.errors[key] = 'Enter town or city';
            break;
          case 'county':
            this.errors[key] = 'Enter county';
            break;
          case 'postcode':
            this.errors[key] = 'Enter postcode';
            break;
        }
      }
      if (validationErrors && validationErrors['maxlength']) {
        switch (key) {
          case 'lineOne':
            this.errors[key] =
              'Address line one cannot exceed 255 characters in length';
            break;
          case 'lineTwo':
            this.errors[key] =
              'Address line two cannot exceed 255 characters in length';
            break;
          case 'city':
            this.errors[key] =
              'Town or city cannot exceed 100 characters in length';
            break;
          case 'county':
            this.errors[key] = 'County cannot exceed 100 characters in length';
            break;
          case 'postcode':
            this.errors[key] =
              'Postcode cannot exceed 100 characters in length';
            break;
        }
      }
    });
  }

  validatePostcode() {
    let postcode = this.form.controls.postcode.value;
    if (!this.postcodeRegex.test(postcode)) {
      this.errors['postcode'] =
        'Enter a full UK postcode in the correct format, like SW1H 0NE';
    }
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
