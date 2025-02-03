import { Component, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Observable, map } from 'rxjs';
import { HttpState } from '../../../../stores/http-state';
import {
  selectCurrentSchemeYear,
  selectEditLinkedLicenceHolderFormValue,
  selectSelectedLicenceHolderLink,
} from '../../../../stores/licence-holders/selectors';
import { getFirstSchemeYear, storeEditLinkedLicenceHolderFormValue } from '../../../../stores/licence-holders/actions';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import * as moment from 'moment';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { hasErrors } from 'src/app/shared/form-utils';
import { Organisation } from 'src/app/navigation/models/organisation';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';
import { navigationOrganisationSessionKey, previousSchemeYearSessionKey, schemeYearSessionKey } from 'src/app/shared/constants';
import { LicenceHolderLinkDto } from 'src/app/stores/licence-holders/dto/licence-holder-link.dto';
import { getOrganisationsAvailableForTransfer } from 'src/app/stores/organisations/actions';
import { selectAvailableForTransfer } from 'src/app/stores/organisations/selectors';
import { SchemeYearDto } from '../../../../stores/organisation-summary/dtos/scheme-year.dto';

export type transferLink = 'Yes' | 'No';

export interface EditLinkedLicenceHolderFormValue {
  organisationId: string | null;
  organisationName: string | null;
  transferLink: transferLink | null;
  day: string | null;
  month: string | null;
  year: string | null;
}

export interface EditLinkedLicenceHolderForm {
  organisationId: FormControl<string | null>;
  organisationName: FormControl<string | null>;
  transferLink: FormControl<transferLink | null>;
  day: FormControl<string | null>;
  month: FormControl<string | null>;
  year: FormControl<string | null>;
}

@Component({
  selector: 'edit-linked-licence-holder',
  templateUrl: './edit-linked-licence-holder.component.html',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgFor, NgIf, AsyncPipe],
})
export class EditLinkedLicenceHolderComponent implements OnInit {
  private destroyRef = inject(DestroyRef);

  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) licenceHolderId!: string;

  manufacturers$: Observable<HttpState<Organisation[]>> | null = null;
  licenceHolderLink$: Observable<LicenceHolderLinkDto | null>;
  linkStartDate: string | null = null;
  currentSchemeYearStartDate: string | null = null;
  organisation: Organisation | null;
  currentSchemeYear: SchemeYearDto | null;
  previousSchemeYear: SchemeYearDto | null;
  form: FormGroup<EditLinkedLicenceHolderForm>;
  errors: { [key: string]: string | null } = {};

  constructor(
    private store: Store,
    private sessionStorageService: SessionStorageService
  ) {
    this.licenceHolderLink$ = this.store.select(
      selectSelectedLicenceHolderLink
    );
    this.organisation = this.sessionStorageService.getObject<Organisation>(
      navigationOrganisationSessionKey
    );
    this.currentSchemeYear = this.sessionStorageService.getObject<SchemeYearDto>(
      schemeYearSessionKey
    );
    this.previousSchemeYear = this.sessionStorageService.getObject<SchemeYearDto>(
      previousSchemeYearSessionKey
    );
    this.store
      .select(selectCurrentSchemeYear)
      .pipe(takeUntilDestroyed())
      .subscribe((currentSchemeYear) => {
        if (currentSchemeYear.data !== null) {
          this.currentSchemeYearStartDate = currentSchemeYear.data.startDate;
        }
      });

    this.form = new FormGroup({
      organisationId: new FormControl<string | null>(null),
      organisationName: new FormControl<string | null>(null),
      transferLink: new FormControl<transferLink | null>(null, {
        validators: [Validators.required],
      }),
      day: new FormControl<string | null>(null),
      month: new FormControl<string | null>(null),
      year: new FormControl<string | null>(null),
    });
  }

  ngOnInit() {
    this.manufacturers$ = this.store.select(selectAvailableForTransfer);

    this.validateInputs();

    this.store.dispatch(
      getOrganisationsAvailableForTransfer({
        organisationId: this.organisationId,
      })
    );
    this.store.dispatch(getFirstSchemeYear());
    this.store
      .select(selectEditLinkedLicenceHolderFormValue)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((editLinkedLicenceHolderFormValue) => {
        if (editLinkedLicenceHolderFormValue) {
          this.form.patchValue({ ...editLinkedLicenceHolderFormValue });
        }
      });
  }

  validateInputs() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    if (this.licenceHolderId === null || this.licenceHolderId === undefined) {
      throw TypeError('licenceHolderId cannot be null or undefined');
    }
  }

  onTransferLink(value: string) {
    const organisationId = this.form.controls.organisationId;
    if (value === 'Yes') {
      organisationId.addValidators(Validators.required);
    } else {
      this.form.patchValue({ organisationId: null, organisationName: null });
      organisationId.removeValidators(Validators.required);
    }
    organisationId.updateValueAndValidity();
  }

  onSelectOrganisation(organisationId: string, organisations: Organisation[]) {
    const organisation = organisations.find((o) => o.id === organisationId);
    if (organisation) {
      this.form.controls.organisationName.setValue(organisation.name);
    }
  }

  link(startDate: string) {
    this.errors = {};
    this.setValidationErrorMessages(startDate);
    if (!hasErrors(this.errors)) {
      const editLinkedLicenceHolderFormValue: EditLinkedLicenceHolderFormValue =
        this.form.getRawValue();
      this.store.dispatch(
        storeEditLinkedLicenceHolderFormValue({
          organisationId: this.organisationId,
          licenceHolderId: this.licenceHolderId,
          editLinkedLicenceHolderFormValue: editLinkedLicenceHolderFormValue,
        })
      );
    }
  }

  setValidationErrorMessages(startDate: string) {
    Object.keys(this.form.controls).forEach((key) => {
      const validationErrors = this.form.get(key)?.errors;
      if (validationErrors && validationErrors['required']) {
        switch (key) {
          case 'organisationId':
            this.errors[key] = 'Select manufacturer to transfer this link to';
            break;
          case 'transferLink':
            this.errors[key] =
              'Select if you would like to transfer this to another manufacturer';
            break;
        }
      }
    });
    this.validateEndDate(
      this.form.controls.day.value,
      this.form.controls.month.value,
      this.form.controls.year.value,
      startDate
    );
  }

  validateEndDate(
    day: string | null,
    month: string | null,
    year: string | null,
    startDate: string
  ) {
    if (day === null || day === '') {
      this.errors['day'] = 'Enter day';
    }
    if (month === null || month === '') {
      this.errors['month'] = 'Enter month';
    }
    if (year === null || year === '') {
      this.errors['year'] = 'Enter year';
    }

    if (day !== null && month !== null && year !== null) {
      const endDate = moment([year, Number(month) - 1, day]);
      if (!endDate.isValid()) {
        this.errors['endDate'] = 'Link start date must be a real date';
      }

      const commandEndDate = Date.parse(`${year}-${month}-${day}`);
      const surrenderDayDate = Date.parse((this.previousSchemeYear || this.currentSchemeYear)?.surrenderDayDate || '');
      const dateTimeOverride = this.sessionStorageService.getObject<string>('dateTimeOverride');
      const currentDate = dateTimeOverride ? Date.parse(dateTimeOverride) : Date.now();
      if (currentDate > surrenderDayDate && commandEndDate < surrenderDayDate) {
        const surrenderDayDateFormatted = new Intl.DateTimeFormat('en-GB').format(surrenderDayDate);
        this.errors['endDate'] = `End date of the link must be after the surrender day date of the scheme: ${surrenderDayDateFormatted}`;
      }
      const linkStartDate = Date.parse(startDate == "Start of scheme" ? this.currentSchemeYearStartDate! : startDate);
      if (commandEndDate <= linkStartDate) {
        this.errors['endDate'] = `End date of the link must not be before the start date of the link: ${startDate}`;
      }
    }
  }
}
