import {
  Component,
  DestroyRef,
  Input,
  OnDestroy,
  OnInit,
  inject,
} from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { BackLinkProvider } from 'src/app/navigation/back-link/back-link.provider';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { GovukFileUploadComponent } from 'src/app/shared/components/govuk/govuk-file-upload/govuk-file-upload.component';
import {
  selectBoilerSalesQuarters,
  selectQuarterlyBoilerSales,
  selectSubmitQuarterlyLoading,
  selectSubmitQuarterlySupportingEvidence,
} from 'src/app/stores/boiler-sales/selectors';
import { Observable, Subscription, first, forkJoin, tap } from 'rxjs';
import {
  deleteQuarterlySupportingEvidence,
  getQuarterlySupportingEvidence,
  uploadQuarterlySupportingEvidence,
  storeQuarterlyBoilerSales,
  getQuarterlyBoilerSales,
  copyQuarterlyBoilerSalesFilesForEditing,
} from 'src/app/stores/boiler-sales/actions';
import { MultiFileUploadState } from 'src/app/shared/store/MultiFileUploadState';
import { formConstants } from 'src/app/shared/constants';
import { selectSelectedSchemeYear } from 'src/app/stores/scheme-years/selectors';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

export interface BoilerSalesQuarterlyForm {
  gas: FormControl<string>;
  oil: FormControl<string>;
}

@Component({
  selector: 'boiler-sales-quarterly-form',
  templateUrl: './boiler-sales-quarterly-form.component.html',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    ReactiveFormsModule,
    RouterLink,
    GovukFileUploadComponent,
    AsyncPipe,
  ],
})
export class BoilerSalesQuarterlyFormComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  @Input() organisationId!: string;
  @Input() schemeYearId!: string;
  @Input() schemeYearQuarterId!: string;
  @Input() mode!: 'submit' | 'edit';
  @Input() returning: string = 'false';

  isEditing: boolean = false;

  year: string | null = null;
  quarter?: string | null = null;

  supportingEvidenceFiles: FileList | null = null;
  supportingEvidenceFileNames: string[] = [];

  form: FormGroup<BoilerSalesQuarterlyForm>;
  errors: { [key: string]: string } = {};

  boilerSales$: Observable<{
    gas: string | null;
    oil: string | null;
  }>;
  supportingEvidence$: Observable<MultiFileUploadState>;
  loading$: Observable<boolean>;

  existingQuarterlySubscription = Subscription.EMPTY;
  subscription = Subscription.EMPTY;
  validationErrorMessagesSub = Subscription.EMPTY;

  constructor(private store: Store, backLinkProvider: BackLinkProvider) {
    this.boilerSales$ = this.store.select(selectQuarterlyBoilerSales);
    this.loading$ = this.store.select(selectSubmitQuarterlyLoading);
    this.supportingEvidence$ = this.store.select(
      selectSubmitQuarterlySupportingEvidence
    );

    backLinkProvider.clear();

    this.form = new FormGroup({
      gas: new FormControl('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.pattern(formConstants.validation.greaterThanZeroRegex),
        ],
      }),
      oil: new FormControl('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.pattern(formConstants.validation.greaterThanZeroRegex),
        ],
      }),
    });
  }

  ngOnInit() {
    this.validateInputs();
    this.isEditing = this.mode === 'edit';
    
    this.store
    .select(selectSelectedSchemeYear)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe((schemeYear) => {
      if (schemeYear) {
        this.year = schemeYear.year;
        this.quarter = schemeYear.quarters.find(
          (q) => q.id === this.schemeYearQuarterId
        )?.name;
      }
    });

    if (this.isEditing && this.returning != 'true') {
        this.existingQuarterlySubscription = this.store
          .select(selectBoilerSalesQuarters)
          .subscribe((result) => {
            let quarter = result.find(
              (q) => q.schemeYearQuarterId === this.schemeYearQuarterId
            );
            if (!quarter) return;
            this.form.patchValue({
              gas: `${quarter.gas}`,
              oil: `${quarter.oil}`,
            });
          });
        this.store.dispatch(
          getQuarterlyBoilerSales({
            organisationId: this.organisationId,
            schemeYearId: this.schemeYearId,
          })
        );
    }

    this.subscription = this.boilerSales$.subscribe((boilerSales) => {
      if (boilerSales.gas !== null && boilerSales.oil !== null) {
        this.form.patchValue({
          gas: boilerSales.gas,
          oil: boilerSales.oil,
        });
      }
    });

    if (this.isEditing) {
      this.store.dispatch(
        copyQuarterlyBoilerSalesFilesForEditing({
          organisationId: this.organisationId,
          schemeYearId: this.schemeYearId,
          schemeYearQuarterId: this.schemeYearQuarterId
        })
      );
    } else {
      this.store.dispatch(
        getQuarterlySupportingEvidence({
          organisationId: this.organisationId,
          schemeYearQuarterId: this.schemeYearQuarterId!,
          schemeYearId: this.schemeYearId,
          isEditing: this.isEditing
        })
      );
    }
  }

  onSupportingEvidenceChange(fileList: FileList) {
    this.store.dispatch(
      uploadQuarterlySupportingEvidence({
        organisationId: this.organisationId,
        schemeYearQuarterId: this.schemeYearQuarterId!,
        fileList: { ...fileList },
        schemeYearId: this.schemeYearId,
        isEditing: this.isEditing
      })
    );
  }

  onDeleteSupportingEvidence(fileName: string) {
    this.store.dispatch(
      deleteQuarterlySupportingEvidence({
        organisationId: this.organisationId,
        schemeYearQuarterId: this.schemeYearQuarterId!,
        fileName,
        schemeYearId: this.schemeYearId,
        isEditing: this.isEditing
      })
    );
  }

  onSubmit() {
    this.clearErrorMessages();
    this.validationErrorMessagesSub = this.setValidationErrorMessages()
      .pipe(
        first(),
        tap((_) => {
          if (Object.getOwnPropertyNames(this.errors).length === 0) {
            let boilerSales = this.form.getRawValue();
            this.store.dispatch(
              storeQuarterlyBoilerSales({
                organisationId: this.organisationId,
                schemeYearId: this.schemeYearId,
                schemeYearQuarterId: this.schemeYearQuarterId!,
                boilerSales,
                isEditing: this.isEditing
              })
            );
          }
        })
      )
      .subscribe();
  }

  setValidationErrorMessages() {
    let gasOilPromise = new Promise((resolve) => {
      Object.keys(this.form.controls).forEach((key) => {
        let validationErrors = this.form.get(key)?.errors;
        if (validationErrors && validationErrors['required']) {
          switch (key) {
            case 'gas':
              this.errors[key] = 'Enter number of gas boiler sales';
              break;
            case 'oil':
              this.errors[key] = 'Enter number of oil boiler sales';
              break;
            default:
              break;
          }
        }
        if (validationErrors && validationErrors['pattern']) {
          switch (key) {
            case 'gas':
              this.errors[key] =
                'Number of gas boiler sales must be whole number greater than zero';
              break;
            case 'oil':
              this.errors[key] =
                'Number of oil boiler sales must be whole number greater than zero';
              break;
            default:
              break;
          }
        }
      });
      resolve(0);
    });

    let supportingEvidenceObservable = this.supportingEvidence$.pipe(
      first(),
      tap((supportingEvidenceFileState) => {
        if ((supportingEvidenceFileState.fileNames?.length || 0) === 0) {
          this.errors['supportingEvidence'] =
            'Choose at least one file to upload as supporting evidence';
        }
      })
    );

    return forkJoin({ gasOilPromise, supportingEvidenceObservable });
  }

  clearErrorMessages() {
    this.errors = {};
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
    if (this.existingQuarterlySubscription) {
      this.existingQuarterlySubscription.unsubscribe();
    }
    if (this.validationErrorMessagesSub) {
      this.validationErrorMessagesSub.unsubscribe();
    }
  }

  validateInputs() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    if (this.schemeYearId === null || this.schemeYearId === undefined) {
      throw TypeError('schemeYearId cannot be null or undefined');
    }
  }
}
