import { Component, Input, OnInit } from '@angular/core';
import { NgIf, NgClass, NgFor, AsyncPipe } from '@angular/common';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Store } from '@ngrx/store';
import { cancelAccountRejection, deleteAccountRejectionFile, getAccountRejectionFiles, storeAccountRejection, uploadAccountRejectionFile } from 'src/app/stores/onboarding/actions';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { MultiFileUploadState } from 'src/app/shared/store/MultiFileUploadState';
import { selectAccountRejectionFiles, selectAccountRejectionLoading } from 'src/app/stores/onboarding/selectors';
import { GovukFileUploadComponent } from 'src/app/shared/components/govuk/govuk-file-upload/govuk-file-upload.component';

export interface AccountRejectionForm {
  comments: FormControl<string | null>;
  confirmation: FormControl<boolean>;
}

@Component({
  selector: 'reject-account',
  templateUrl: './reject-account.component.html',
  standalone: true,
  imports: [NgIf, NgFor, AsyncPipe, ReactiveFormsModule, NgClass, RouterLink, GovukFileUploadComponent],
})
export class RejectAccountComponent implements OnInit {
  @Input() organisationId?: string;
  _edit = false;
  @Input() set edit(value: string) {
    this._edit = value === 'true' ? true : false;
  }
  get edit(): boolean {
    return this._edit;
  }

  form: FormGroup<AccountRejectionForm>;
  errors: { [key: string]: string } = {};

  loading$: Observable<boolean | undefined | null>;
  files$: Observable<MultiFileUploadState>;
  files: FileList | null = null;
  fileNames: string[] = [];

  constructor(private store: Store) {
    this.files$ = this.store.select(selectAccountRejectionFiles);
    this.loading$ = this.store.select(selectAccountRejectionLoading);
    this.form = new FormGroup({
      comments: new FormControl<string | null>('', {
        validators: [Validators.maxLength(255)]
      }),
      confirmation: new FormControl<boolean>(false, {
        nonNullable: true,
        validators: [Validators.requiredTrue],
      }),
    });
  }

  ngOnInit(): void {
    this.store.dispatch(getAccountRejectionFiles({
      organisationId: this.organisationId!
    }));
  }

  onFilesChange(fileList: FileList) {
    this.store.dispatch(
      uploadAccountRejectionFile({
        organisationId: this.organisationId!,
        fileList: { ...fileList },
      })
    );
  }

  onDeleteFile(fileName: string) {
    this.store.dispatch(
      deleteAccountRejectionFile({
        organisationId: this.organisationId!,
        fileName,
      })
    );
  }

  onSubmit() {
    this.setValidationErrorMessages();
    if (Object.getOwnPropertyNames(this.errors).length === 0) {
      let accountRejection = this.form.getRawValue();
      this.store.dispatch(
        storeAccountRejection({
          accountRejection: {
            comments: accountRejection.comments,
          },
          edit: this.edit,
        })
      );
    }
  }

  onCancel() {
    this.store.dispatch(cancelAccountRejection({ edit: this.edit }));
  }

  setValidationErrorMessages() {
    this.errors = {};
    Object.keys(this.form.controls).forEach((key) => {
      const validationErrors = this.form.get(key)?.errors;
      if (validationErrors) {
        if (validationErrors['required']) {
          if (key == 'confirmation') {
            this.errors[key] =
              'Please confirm you have conducted the necessary checks and can confirm this manufacturer is not eligible for an account.';
          }
        } else if (validationErrors['maxlength']) {
          this.errors[key] = 'Enter less than 255 characters';
        }
      }
    });
  }
}
