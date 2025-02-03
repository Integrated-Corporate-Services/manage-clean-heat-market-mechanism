import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, AsyncPipe, NgFor } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { Store } from '@ngrx/store';
import { SchemeYearConfigurationDto } from '../../../../stores/scheme-year-configuration/dtos/scheme-year-configuration.dto';
import { HttpState } from '../../../../stores/http-state';
import { SchemeYearDto } from '../../../../stores/scheme-year-configuration/dtos/scheme-year.dto';
import { BackLinkProvider } from '../../../../navigation/back-link/back-link.provider';
import { selectSchemeYear, selectSchemeYearConfiguration, selectUpdateSchemeYearConfigurationCommand } from '../../../../stores/scheme-year-configuration/selectors';
import { getSchemeYear, storeSchemeYearConfiguration } from '../../../../stores/scheme-year-configuration/actions';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { formConstants } from '../../../../shared/constants';
import { UpdateSchemeYearConfigurationCommand } from '../../../../stores/scheme-year-configuration/commands/update-scheme-year-configuration.command';

export interface SchemeConfigurationEditForm {
  percentageCap: FormControl<string>;
  targetMultiplier: FormControl<string>;
  gasBoilerSalesThreshold: FormControl<string>;
  oilBoilerSalesThreshold: FormControl<string>;
  targetRate: FormControl<string>;
  creditCarryOverPercentage: FormControl<string>;
  alternativeRenewableSystemFuelTypeWeightingValue: FormControl<string>;
  alternativeFossilFuelSystemFuelTypeWeightingValue: FormControl<string>;
}

@Component({
  selector: 'scheme-configuration-edit-form',
  templateUrl: './scheme-configuration-edit-form.component.html',
  styleUrls: ['./scheme-configuration-edit-form.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgIf, NgFor, AsyncPipe],
})
export class SchemeConfigurationEditFormComponent implements OnInit, OnDestroy {

  @Input({ required: true }) schemeYearId!: string;
  @Input() returning: string = 'false';

  schemeYear$: Observable<HttpState<SchemeYearDto>>;

  form: FormGroup<SchemeConfigurationEditForm>;
  errors: any = {};

  subscription: Subscription = Subscription.EMPTY;

  constructor(private store: Store, backLinkProvider: BackLinkProvider) {
    backLinkProvider.clear();
    this.form = new FormGroup({
      percentageCap: this.createControl(true),
      targetMultiplier: this.createControl(true),
      gasBoilerSalesThreshold: this.createControl(false),
      oilBoilerSalesThreshold: this.createControl(false),
      targetRate: this.createControl(true),
      creditCarryOverPercentage: this.createControl(true),
      alternativeRenewableSystemFuelTypeWeightingValue: this.createControl(true),
      alternativeFossilFuelSystemFuelTypeWeightingValue: this.createControl(true)
    });
    this.schemeYear$ = this.store.select(selectSchemeYear);
  }

  private createControl(isDecimal: boolean) {
    return new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.pattern(isDecimal
          ? formConstants.validation.positiveDecimalRegex
          : formConstants.validation.positiveNumberRegex
        ),
      ],
    });
  }

  private setValues(values: UpdateSchemeYearConfigurationCommand | SchemeYearConfigurationDto | null) {
    if (!values) return;
    this.form.patchValue({
      percentageCap: `${values.percentageCap}`,
      targetMultiplier: `${values.targetMultiplier}`,
      gasBoilerSalesThreshold: `${values.gasBoilerSalesThreshold}`,
      oilBoilerSalesThreshold: `${values.oilBoilerSalesThreshold}`,
      targetRate: `${values.targetRate}`,
      creditCarryOverPercentage: `${values.creditCarryOverPercentage}`,
      alternativeRenewableSystemFuelTypeWeightingValue: `${values.alternativeRenewableSystemFuelTypeWeightingValue}`,
      alternativeFossilFuelSystemFuelTypeWeightingValue: `${values.alternativeFossilFuelSystemFuelTypeWeightingValue}`,
    });
  }

  ngOnInit() {
    if (this.returning == 'true') {
      this.subscription = this.store.select(selectUpdateSchemeYearConfigurationCommand).subscribe(command => {
        this.setValues(command);
      });
    } else {
      this.subscription = this.store.select(selectSchemeYearConfiguration).subscribe(configuration => {
        this.setValues(configuration.data);
      });
    }

    this.store.dispatch(
      getSchemeYear({
        schemeYearId: this.schemeYearId
      })
    );
  }

  ngOnDestroy() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  onSubmit() {
    this.errors = {};
    if (this.form.valid) {
      this.storeValues();
      return;
    }
    Object.keys(this.form.controls).forEach((key) => {
      let validationErrors = this.form.get(key)?.errors;
      if (!validationErrors) return;
      if (validationErrors['required']) {
        switch (key) {
          case 'targetRate': this.errors[key] = 'Enter number for gas & oil boiler target'; break;
          case 'gasBoilerSalesThreshold': this.errors[key] = 'Enter number for gas boiler threshold'; break;
          case 'oilBoilerSalesThreshold': this.errors[key] = 'Enter number for oil boiler threshold'; break;
          case 'percentageCap': this.errors[key] = 'Enter number for low-carbon heat target carry forward percentage'; break;
          case 'targetMultiplier': this.errors[key] = 'Enter number for low-carbon heat target carry forward multiplier'; break;
          case 'creditCarryOverPercentage': this.errors[key] = 'Enter number for credit carry over percentage'; break;
          case 'alternativeRenewableSystemFuelTypeWeightingValue': this.errors[key] = 'Enter number for renewable weighting'; break;
          case 'alternativeFossilFuelSystemFuelTypeWeightingValue': this.errors[key] = 'Enter number for fossil fuel weighting'; break;
          default: break;
        }
      }
      if (validationErrors['pattern']) {
        switch (key) {
          case 'targetRate': this.errors[key] = 'Gas & oil boiler target must be greater than zero and either a whole, or a decimal number'; break;
          case 'gasBoilerSalesThreshold': this.errors[key] = 'Gas boiler threshold must be greater than zero and a whole number'; break;
          case 'oilBoilerSalesThreshold': this.errors[key] = 'Oil boiler threshold must be greater than zero and a whole number'; break;
          case 'percentageCap': this.errors[key] = 'Low-carbon heat target carry forward percentage must be greater than zero and either a whole, or a decimal number'; break;
          case 'targetMultiplier': this.errors[key] = 'Low-carbon heat target carry forward multiplier must be greater than zero and either a whole, or a decimal number'; break;
          case 'creditCarryOverPercentage': this.errors[key] = 'Credit carry over percentage must be greater than zero and either a whole, or a decimal number'; break;
          case 'alternativeRenewableSystemFuelTypeWeightingValue': this.errors[key] = 'Renewable weighting must be greater than zero and either a whole, or a decimal number'; break;
          case 'alternativeFossilFuelSystemFuelTypeWeightingValue': this.errors[key] = 'Fossil fuel weighting must be greater than zero and either a whole, or a decimal number'; break;
          default: break;
        }
      }
    });
  }

  private storeValues() {
    let form = this.form.getRawValue();
    this.store.dispatch(
      storeSchemeYearConfiguration({
        schemeYearConfiguration: {
          schemeYearId: this.schemeYearId,
          percentageCap: Number(form.percentageCap),
          targetMultiplier: Number(form.targetMultiplier),
          gasBoilerSalesThreshold: Number(form.gasBoilerSalesThreshold),
          oilBoilerSalesThreshold: Number(form.oilBoilerSalesThreshold),
          targetRate: Number(form.targetRate),
          creditCarryOverPercentage: Number(form.creditCarryOverPercentage),
          alternativeRenewableSystemFuelTypeWeightingValue: Number(form.alternativeRenewableSystemFuelTypeWeightingValue),
          alternativeFossilFuelSystemFuelTypeWeightingValue: Number(form.alternativeFossilFuelSystemFuelTypeWeightingValue)
        }
      })
    );
  }
}
