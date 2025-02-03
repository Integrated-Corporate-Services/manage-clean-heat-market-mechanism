import { Component, Input, OnDestroy, ViewChild } from '@angular/core';
import { AsyncPipe, NgIf } from '@angular/common';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Store } from '@ngrx/store';
import { AddressFormComponent } from '../address-form/address-form.component';
import {
  editAccount,
  storeIsNonSchemeParticipant,
  storeLegalCorrespondenceAddress,
} from 'src/app/stores/onboarding/actions';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { Observable, Subscription } from 'rxjs';
import { Address, IsUsedAsLegalCorrespondence } from '../models/address';
import {
  selectLegalCorrespondenceAddress,
  selectOrganisationDetailsLoading,
} from 'src/app/stores/onboarding/selectors';
import { OnboardingFormMode } from '../models/mode';

export interface AddressForm {
  lineOne: FormControl<string>;
  lineTwo: FormControl<string | null>;
  city: FormControl<string>;
  county: FormControl<string | null>;
  postcode: FormControl<string>;
}

export interface LegalCorrespondenceForm {
  isUsedAsLegalCorrespondence: FormControl<IsUsedAsLegalCorrespondence>;
}

@Component({
  selector: 'legal-correspondence-form',
  templateUrl: './legal-correspondence-form.component.html',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, AddressFormComponent, AsyncPipe],
})
export class LegalCorrespondenceFormComponent implements OnDestroy {
  @Input() mode: OnboardingFormMode = 'default';
  @Input() organisationId?: string;

  @ViewChild(AddressFormComponent) addressFormComponent?: AddressFormComponent;

  form: FormGroup<LegalCorrespondenceForm>;
  errors: { [key: string]: string } = {};
  legalCorrespondenceAddress: Address | null = null;

  loading$: Observable<boolean>;
  legalCorrespondenceAddress$: Observable<Address | null>;
  subscription = Subscription.EMPTY;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider
  ) {
    this.legalCorrespondenceAddress$ = this.store.select(
      selectLegalCorrespondenceAddress
    );
    this.loading$ = this.store.select(selectOrganisationDetailsLoading);

    this.form = new FormGroup({
      isUsedAsLegalCorrespondence: new FormControl<IsUsedAsLegalCorrespondence>(
        null,
        {
          validators: [Validators.required],
        }
      ),
    });
  }

  ngOnInit() {
    this.subscription = this.legalCorrespondenceAddress$.subscribe(
      (legalCorrespondenceAddress) => {
        if (legalCorrespondenceAddress) {
          this.form.patchValue({
            isUsedAsLegalCorrespondence:
              legalCorrespondenceAddress.isUsedAsLegalCorrespondence!,
          });
          this.legalCorrespondenceAddress = legalCorrespondenceAddress;
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
          '/organisation/register/registered-office-address';
    }
  }

  onInitAddressForm() {
    if (this.legalCorrespondenceAddress) {
      this.addressFormComponent?.form.patchValue({
        lineOne: this.legalCorrespondenceAddress.lineOne,
        lineTwo: this.legalCorrespondenceAddress.lineTwo,
        city: this.legalCorrespondenceAddress.city,
        county: this.legalCorrespondenceAddress.county,
        postcode: this.legalCorrespondenceAddress.postcode,
      });
    }
  }

  onProvideAnotherAddress() {
    this.addressFormComponent?.form.patchValue({
      lineOne: '',
      lineTwo: null,
      city: '',
      county: null,
      postcode: '',
    });
  }

  onSubmit() {
    this.errors = {};
    if (this.form.valid) {
      const value = this.form.getRawValue();

      const isUsedAsLegalCorrespondence = value.isUsedAsLegalCorrespondence;
      switch (isUsedAsLegalCorrespondence) {
        case 'Yes':
          this.store.dispatch(
            storeLegalCorrespondenceAddress({
              mode: this.mode,
              address: {
                isUsedAsLegalCorrespondence: isUsedAsLegalCorrespondence,
              },
            })
          );
          break;
        case 'No':
          if (this.addressFormComponent) {
            this.validateAndStoreNewAddress(
              this.addressFormComponent,
              isUsedAsLegalCorrespondence
            );
          }
          break;
        case 'IsNonSchemeParticipant':
          this.store.dispatch(
            storeIsNonSchemeParticipant({
              mode: this.mode,
              isNonSchemeParticipant: true,
            })
          );
          break;
      }
    } else {
      this.setValidationErrorMessages();
    }
  }

  setValidationErrorMessages() {
    const validationErrors =
      this.form.controls.isUsedAsLegalCorrespondence.errors;
    if (validationErrors && validationErrors['required']) {
      this.errors['isUsedAsLegalCorrespondence'] =
        'Select if this address should be used for legal correspondence';
    }
  }

  validateAndStoreNewAddress(
    addressFormComponent: AddressFormComponent,
    isUsedAsLegalCorrespondence: IsUsedAsLegalCorrespondence | null
  ) {
    addressFormComponent.clearErrorMessages();
    addressFormComponent.validatePostcode();

    const address = addressFormComponent.form;
    if (address.valid && !addressFormComponent.errors['postcode']) {
      const addressFormValue = address.getRawValue();
      this.store.dispatch(
        storeLegalCorrespondenceAddress({
          mode: this.mode,
          address: {
            lineOne: addressFormValue.lineOne,
            lineTwo: addressFormValue.lineTwo,
            city: addressFormValue.city,
            county: addressFormValue.county,
            postcode: addressFormValue.postcode,
            isUsedAsLegalCorrespondence: isUsedAsLegalCorrespondence,
          },
        })
      );
    } else {
      addressFormComponent.setValidationErrorMessages();
    }
  }

  ngOnDestroy() {
    this.backLinkProvider.clear();
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
