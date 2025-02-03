import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgClass, NgFor, NgIf } from '@angular/common';
import { BehaviorSubject, Observable, filter, map } from 'rxjs';
import { McsState } from 'src/app/stores/mcs/state';
import { selectSchemeYears } from '../../../stores/scheme-year-configuration/selectors';
import { selectMcsState } from 'src/app/stores/mcs/selectors';
import { getSchemeYears } from '../../../stores/scheme-year-configuration/actions';
import { getMcsDownloads } from 'src/app/stores/mcs/actions';
import { HttpState } from '../../../stores/http-state';
import { SchemeYearDto } from '../../../stores/scheme-year-configuration/dtos/scheme-year.dto';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { hasErrors } from 'src/app/shared/form-utils';
import { InstallationRequestSummaryDto } from 'src/app/stores/mcs/dto/installation-request-summary.dto';
import { GovukPaginationComponent } from 'src/app/shared/components/govuk/govuk-pagination/govuk-pagination.component';

export interface LoadDataFormValue {
  selectedSchemeYearIdId: string | null;
}

export interface LoadDataForm {
  selectedSchemeYearIdId: FormControl<string | null>;
}

@Component({
  selector: 'view-mcs-data',
  templateUrl: './view-mcs-data.component.html',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgFor, NgIf, AsyncPipe, NgClass, GovukPaginationComponent],
})
export class ViewMcsDataComponent implements OnInit {
  schemeYears$: Observable<HttpState<SchemeYearDto[]>>;
  mcsState$: Observable<McsState>;

  paginatedDataSubject: BehaviorSubject<InstallationRequestSummaryDto[]>;
  paginatedData$: Observable<InstallationRequestSummaryDto[]>;

  errors: { [key: string]: string | null } = {};
  form: FormGroup<LoadDataForm>;

  constructor(private store: Store) {
    this.schemeYears$ = this.store.select(selectSchemeYears);
    this.mcsState$ = this.store.select(selectMcsState);

    this.paginatedDataSubject = new BehaviorSubject<InstallationRequestSummaryDto[]>([]);    
    this.paginatedData$ = this.paginatedDataSubject.asObservable();

    this.form = new FormGroup({
      selectedSchemeYearIdId: new FormControl<string | null>(null, {
        validators: [Validators.required],
      })
    });
  }

  ngOnInit() {
    const currentYear = new Date().getFullYear();

    this.store.dispatch(getSchemeYears());
    this.store.dispatch(getMcsDownloads({ schemeYearId: null }));

    this.schemeYears$.pipe(
      filter(httpState => !httpState.loading),
      map(httpState => httpState.data),
      map(schemeYears => schemeYears!.find(schemeYear => schemeYear.name === currentYear.toString()))
    ).subscribe(schemeYear => {
      if (schemeYear) {
        const defaultSchemeYearId = schemeYear.id;
        this.form.get('selectedSchemeYearIdId')!.setValue(defaultSchemeYearId);
      }
    });
  }

  loadTable() {
    this.errors = {};
    if (!hasErrors(this.errors)) {
      const linkLicenceHolderFormValue: LoadDataFormValue = this.form.getRawValue();
      this.store.dispatch(getMcsDownloads({ schemeYearId: linkLicenceHolderFormValue.selectedSchemeYearIdId }));
    }
  }

  onPageChange(event: any[]) {
    this.paginatedDataSubject.next(event);
  }
}
