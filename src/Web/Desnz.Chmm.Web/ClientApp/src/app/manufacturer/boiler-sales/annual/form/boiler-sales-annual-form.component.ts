import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import {
  Router,
  RouterLink,
} from '@angular/router';
import { BackLinkProvider } from 'src/app/navigation/back-link/back-link.provider';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { GovukFileUploadComponent } from 'src/app/shared/components/govuk/govuk-file-upload/govuk-file-upload.component';
import {
  selectSubmitAnnualVerificationStatement,
  selectSubmitAnnualSupportingEvidence,
  selectSubmitAnnualLoading,
  selectSubmitAnnual,
  selectBoilerSalesSummary,
} from 'src/app/stores/boiler-sales/selectors';
import { Observable, Subscription, first, forkJoin, tap } from 'rxjs';
import {
  copyAnnualBoilerSalesFilesForEditing,
  deleteAnnualSupportingEvidence,
  deleteAnnualVerificationStatement,
  getAnnualBoilerSales,
  getAnnualSupportingEvidence,
  getAnnualVerificationStatement,
  storeAnnualBoilerSales,
  uploadAnnualSupportingEvidence,
  uploadAnnualVerificationStatement,
} from 'src/app/stores/boiler-sales/actions';
import { MultiFileUploadState } from 'src/app/shared/store/MultiFileUploadState';
import { formConstants } from 'src/app/shared/constants';
import { SubmitAnnualState } from '../../../../stores/boiler-sales/state';
export interface BoilerSalesAnnualForm {
  gas: FormControl<string>;
  oil: FormControl<string>;
}

@Component({
  selector: 'boiler-sales-annual-form',
  templateUrl: './boiler-sales-annual-form.component.html',
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
export class BoilerSalesAnnualFormComponent implements OnInit, OnDestroy {
  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;
  @Input() returning: string = 'false';

  year: number = 2024;

  verificationStatementFiles: FileList | null = null;
  verificationStatementFileNames: string[] = [];

  supportingEvidenceFiles: FileList | null = null;
  supportingEvidenceFileNames: string[] = [];

  mode: string = 'submit';
  isEditing: boolean = false;
  form: FormGroup<BoilerSalesAnnualForm>;
  errors: { [key: string]: string } = {};

  boilerSales$: Observable<SubmitAnnualState>;
  verificationStatement$: Observable<MultiFileUploadState>;
  supportingEvidence$: Observable<MultiFileUploadState>;
  loading$: Observable<boolean>;

  subscription = Subscription.EMPTY;
  existingAnnualSubscription = Subscription.EMPTY;
  validationErrorMessagesSub = Subscription.EMPTY;

  constructor(
    private store: Store,
    private router: Router,
    backLinkProvider: BackLinkProvider
  ) {
    this.boilerSales$ = this.store.select(selectSubmitAnnual);
    this.loading$ = this.store.select(selectSubmitAnnualLoading);

    this.verificationStatement$ = this.store.select(
      selectSubmitAnnualVerificationStatement
    );
    this.supportingEvidence$ = this.store.select(
      selectSubmitAnnualSupportingEvidence
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
    this.mode = (this.router.url.split('/').pop() || 'submit').split('?')[0];
    this.isEditing = this.mode === 'edit';

    if (this.isEditing && this.returning != 'true') {
      this.existingAnnualSubscription = this.store
        .select(selectBoilerSalesSummary)
        .subscribe((result) => {
          if (!result.data?.annual) return;
          this.form.patchValue({
            gas: `${result.data?.annual.gas}`,
            oil: `${result.data?.annual.oil}`,
          });
        });
      this.store.dispatch(
        getAnnualBoilerSales({
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
        copyAnnualBoilerSalesFilesForEditing({
          organisationId: this.organisationId,
          schemeYearId: this.schemeYearId,
        })
      );
    } else {
      this.store.dispatch(
        getAnnualVerificationStatement({
          organisationId: this.organisationId,
          schemeYearId: this.schemeYearId,
          isEditing: this.isEditing,
        })
      );
      this.store.dispatch(
        getAnnualSupportingEvidence({
          organisationId: this.organisationId,
          schemeYearId: this.schemeYearId,
          isEditing: this.isEditing,
        })
      );
    }
  }

  onVerificationStatementChange(fileList: FileList) {
    this.store.dispatch(
      uploadAnnualVerificationStatement({
        organisationId: this.organisationId,
        fileList: { ...fileList },
        schemeYearId: this.schemeYearId,
        isEditing: this.isEditing,
      })
    );
  }

  onSupportingEvidenceChange(fileList: FileList) {
    this.store.dispatch(
      uploadAnnualSupportingEvidence({
        organisationId: this.organisationId,
        fileList: { ...fileList },
        schemeYearId: this.schemeYearId,
        isEditing: this.isEditing,
      })
    );
  }

  onDeleteVerificationStatement(fileName: string) {
    this.store.dispatch(
      deleteAnnualVerificationStatement({
        organisationId: this.organisationId,
        fileName,
        schemeYearId: this.schemeYearId,
        isEditing: this.isEditing,
      })
    );
  }

  onDeleteSupportingEvidence(fileName: string) {
    this.store.dispatch(
      deleteAnnualSupportingEvidence({
        organisationId: this.organisationId,
        fileName,
        schemeYearId: this.schemeYearId,
        isEditing: this.isEditing,
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
              storeAnnualBoilerSales({
                organisationId: this.organisationId,
                schemeYearId: this.schemeYearId,
                boilerSales,
                isEditing: this.isEditing,
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

    let verificationStatementObservable = this.verificationStatement$.pipe(
      first(),
      tap((verificationStatementFileState) => {
        if ((verificationStatementFileState.fileNames?.length || 0) === 0) {
          this.errors['verificationStatement'] =
            'Choose at least one file to upload for your verification statement';
        }
      })
    );

    return forkJoin({
      gasOilPromise,
      supportingEvidenceObservable,
      verificationStatementObservable,
    });
  }

  clearErrorMessages() {
    this.errors = {};
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
    if (this.validationErrorMessagesSub) {
      this.validationErrorMessagesSub.unsubscribe();
    }
    if (this.existingAnnualSubscription) {
      this.existingAnnualSubscription.unsubscribe();
    }
  }

  validateInputs() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('orgId cannot be null or undefined');
    }

    if (this.schemeYearId === null || this.schemeYearId === undefined) {
      throw TypeError('schemeYearId cannot be null or undefined');
    }
  }
}
