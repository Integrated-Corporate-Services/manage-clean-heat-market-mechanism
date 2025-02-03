import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import {
  FormArray,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Store } from '@ngrx/store';
import { AddressFormComponent } from '../address-form/address-form.component';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import {
  editAccount,
  storeHeatPumpBrands,
} from 'src/app/stores/onboarding/actions';
import { Observable, Subscription } from 'rxjs';
import { HeatPumps } from '../models/heat-pump-brands';
import {
  selectHeatPumps,
  selectOrganisationDetailsLoading,
} from 'src/app/stores/onboarding/selectors';
import { OnboardingFormMode } from '../models/mode';

export interface HeatPumpBrandsForm {
  isHeatPumpSeller: FormControl<string>;
  heatPumps: FormArray<FormGroup<HeatPumpBrandForm>>;
}

export interface HeatPumpBrandForm {
  heatPumpBrand: FormControl<string>;
}

@Component({
  selector: 'heat-pump-brands-form',
  templateUrl: './heat-pump-brands-form.component.html',
  standalone: true,
  imports: [NgIf, NgFor, ReactiveFormsModule, AddressFormComponent, AsyncPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeatPumpBrandsFormComponent implements OnInit, OnDestroy {
  @Input() mode: OnboardingFormMode = 'default';
  @Input() organisationId?: string;

  form: FormGroup<HeatPumpBrandsForm>;
  heatPumps: FormArray<FormGroup<HeatPumpBrandForm>>;
  errors: { [key: string]: string } = {};

  loading$: Observable<boolean>;
  heatPumps$: Observable<HeatPumps | null>;
  subscription = Subscription.EMPTY;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider
  ) {
    this.heatPumps$ = this.store.select(selectHeatPumps);
    this.loading$ = this.store.select(selectOrganisationDetailsLoading);

    this.form = new FormGroup({
      isHeatPumpSeller: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      heatPumps: new FormArray<FormGroup<HeatPumpBrandForm>>([]),
    });
    this.heatPumps = this.form.controls.heatPumps as FormArray<
      FormGroup<HeatPumpBrandForm>
    >;
  }

  ngOnInit() {
    this.subscription = this.heatPumps$.subscribe((heatPumps) => {
      if (heatPumps && this.heatPumps.length === 0) {
        this.form.patchValue({
          isHeatPumpSeller: heatPumps.isHeatPumpSeller,
        });
        heatPumps.brands.forEach((brand, idx) => {
          this.addHeatPumpBrandControl(brand);
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
          '/organisation/register/fossil-fuel-boilers';
    }
  }

  onNoHeatPumps(value: string) {
    if (value === 'No') {
      this.removeHeatPumpBrandControls();
    } else if (value === 'Yes' && this.heatPumps.length === 0) {
      this.addHeatPumpBrandControl();
    }
  }

  removeHeatPumpBrandControls() {
    this.form.controls.heatPumps = new FormArray<FormGroup<HeatPumpBrandForm>>(
      []
    );
    this.heatPumps = this.form.controls.heatPumps;
    this.form.updateValueAndValidity();
  }

  addHeatPumpBrandControl(value?: string) {
    let newHeatPumpBrand = new FormGroup({
      heatPumpBrand: new FormControl(value ?? '', {
        nonNullable: true,
        validators: [Validators.required],
      }),
    });
    this.form.controls.heatPumps.push(newHeatPumpBrand);
    this.heatPumps = this.form.controls.heatPumps;
    this.form.updateValueAndValidity();
  }

  onAddHeatPumpBrand() {
    this.addHeatPumpBrandControl();
  }

  onRemoveHeatPumpBrand(idx: number) {
    this.heatPumps.removeAt(idx);
    if (this.heatPumps.length === 0) {
      this.addHeatPumpBrandControl();
    }
  }

  onSubmit() {
    this.errors = {};
    this.form.updateValueAndValidity();

    if (this.form.valid) {
      let heatPumps = this.form.getRawValue();
      this.store.dispatch(
        storeHeatPumpBrands({
          heatPumps: {
            isHeatPumpSeller: heatPumps.isHeatPumpSeller,
            brands: heatPumps.heatPumps
              .filter((h) => h.heatPumpBrand)
              .map((h) => h.heatPumpBrand),
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
      let control = this.form.get(key);
      let validationErrors = control?.errors;
      if (
        (validationErrors && validationErrors['required']) ||
        !control?.valid
      ) {
        switch (key) {
          case 'isHeatPumpSeller':
            this.errors[key] = 'Select if you supply heat pumps';
            break;
          case 'heatPumps':
            this.setHeatPumpBrandsValidationErrorMessages(
              this.form.controls[key]
            );
            break;
        }
      }
    });
  }

  setHeatPumpBrandsValidationErrorMessages(
    form: FormArray<FormGroup<HeatPumpBrandForm>>
  ) {
    for (let idx = 0; idx < form.controls.length; idx++) {
      let validationErrors = form.controls[idx].controls.heatPumpBrand.errors;
      if (validationErrors && validationErrors['required']) {
        this.errors[`heatPumpBrand${idx}`] = 'Enter heat pump brand';
      }
    }
  }

  ngOnDestroy() {
    this.backLinkProvider.clear();
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
