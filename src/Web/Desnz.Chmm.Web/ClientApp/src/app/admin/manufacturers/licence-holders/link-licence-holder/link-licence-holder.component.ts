import { Component, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Observable } from 'rxjs';
import { HttpState } from '../../../../stores/http-state';
import { LicenceHolderDto } from '../../../../stores/licence-holders/dto/licence-holder';
import {
  selectFirstSchemeYear,
  selectLinkLicenceHolderFormValue,
  selectUnlinkedLicenceHolders,
} from '../../../../stores/licence-holders/selectors';
import {
  getFirstSchemeYear,
  getUnlinkedLicenceHolders,
  storeLinkLicenceHolderFormValue,
} from '../../../../stores/licence-holders/actions';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import * as moment from 'moment';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { hasErrors } from 'src/app/shared/form-utils';

export type linkStart = 'startOfScheme' | 'specificDate';

export interface LinkLicenceHolderFormValue {
  selectedLicenceHolderId: string | null;
  linkStart: linkStart | null;
  day: string | null;
  month: string | null;
  year: string | null;
}

export interface LinkLicenceHolderForm {
  selectedLicenceHolderId: FormControl<string | null>;
  linkStart: FormControl<linkStart | null>;
  day: FormControl<string | null>;
  month: FormControl<string | null>;
  year: FormControl<string | null>;
}

@Component({
  selector: 'link-licence-holder',
  templateUrl: './link-licence-holder.component.html',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgFor, NgIf, AsyncPipe],
})
export class LinkLicenceHolderComponent implements OnInit {
  private destroyRef = inject(DestroyRef);

  @Input({ required: true }) organisationId!: string;

  firstSchemeYearStartDate: moment.Moment | null = null;
  form: FormGroup<LinkLicenceHolderForm>;
  errors: { [key: string]: string | null } = {};

  unlinkedLicenceHolders$: Observable<HttpState<LicenceHolderDto[]>>;

  constructor(private store: Store) {
    this.unlinkedLicenceHolders$ = this.store.select(
      selectUnlinkedLicenceHolders
    );
    this.store
      .select(selectFirstSchemeYear)
      .pipe(takeUntilDestroyed())
      .subscribe((currentSchemeYear) => {
        if (currentSchemeYear.data !== null) {
          this.firstSchemeYearStartDate = moment(
            currentSchemeYear.data.startDate,
            'YYYY-MM-DD'
          );
        }
      });

    this.form = new FormGroup({
      selectedLicenceHolderId: new FormControl<string | null>(null, {
        validators: [Validators.required],
      }),
      linkStart: new FormControl<linkStart | null>(null, {
        validators: [Validators.required],
      }),
      day: new FormControl<string | null>(null),
      month: new FormControl<string | null>(null),
      year: new FormControl<string | null>(null),
    });
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    this.store.dispatch(getFirstSchemeYear());
    this.store.dispatch(getUnlinkedLicenceHolders());
    this.store
      .select(selectLinkLicenceHolderFormValue)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((linkLicenceHolderFormValue) => {
        if (linkLicenceHolderFormValue) {
          this.form.patchValue({ ...linkLicenceHolderFormValue });
        }
      });
  }

  link() {
    this.errors = {};
    this.setValidationErrorMessages();
    if (!hasErrors(this.errors)) {
      const linkLicenceHolderFormValue: LinkLicenceHolderFormValue =
        this.form.getRawValue();
      this.store.dispatch(
        storeLinkLicenceHolderFormValue({
          organisationId: this.organisationId,
          linkLicenceHolderFormValue: linkLicenceHolderFormValue,
        })
      );
    }
  }

  setValidationErrorMessages() {
    Object.keys(this.form.controls).forEach((key) => {
      const validationErrors = this.form.get(key)?.errors;
      if (validationErrors && validationErrors['required']) {
        switch (key) {
          case 'selectedLicenceHolderId':
            this.errors[key] = 'Select licence holder to link';
            break;
          case 'linkStart':
            this.errors[key] =
              'Select if the link starts from the start of the scheme year or from a specific date';
            break;
        }
      }
    });
    if (this.form.controls.linkStart.value === 'specificDate') {
      this.validateStartDate(
        this.form.controls.day.value,
        this.form.controls.month.value,
        this.form.controls.year.value
      );
    }
  }

  validateStartDate(
    day: string | null,
    month: string | null,
    year: string | null
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
      const startDate = moment([year, Number(month) - 1, day]);

      if (!startDate.isValid()) {
        this.errors['startDate'] = 'Link start date must be a real date';
      }

      if (startDate.isBefore(this.firstSchemeYearStartDate)) {
        this.errors[
          'startDate'
        ] = `The start date of the link must be after the start of the scheme on ${this.firstSchemeYearStartDate?.format(
          'DD/MM/YYYY'
        )}`;
      }
    }
  }
}
