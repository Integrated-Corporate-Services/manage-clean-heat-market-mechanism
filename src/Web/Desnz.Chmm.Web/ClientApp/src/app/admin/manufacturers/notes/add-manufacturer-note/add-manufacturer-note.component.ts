import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { RouterLink, RouterLinkActive } from '@angular/router';
import {
  FormControl,
  FormGroup,
  FormsModule,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { BackLinkProvider } from '../../../../navigation/back-link/back-link.provider';
import {
  clearManufacturerNewNoteFiles,
  deleteManufacturerNewNoteFile,
  getManufacturerNewNoteFiles,
  submitManufacturerNewNote,
  uploadManufacturerNewNoteFile,
} from '../../../../stores/account-management/actions';
import { Observable } from 'rxjs';
import { MultiFileUploadState } from '../../../../shared/store/MultiFileUploadState';
import {
  selectAddManufacturerNoteLoading,
  selectManufacturerNewNoteFiles,
} from '../../../../stores/account-management/selectors';
import { GovukFileUploadComponent } from 'src/app/shared/components/govuk/govuk-file-upload/govuk-file-upload.component';

export interface AddManufacturerNoteForm {
  details: FormControl<string>;
}

@Component({
  selector: 'add-manufacturer-note',
  templateUrl: './add-manufacturer-note.component.html',
  styleUrls: ['./add-manufacturer-note.component.css'],
  standalone: true,
  imports: [
    ReactiveFormsModule,
    GovukFileUploadComponent,
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe,
  ],
})
export class AddManufacturerNoteComponent implements OnInit, OnDestroy {
  @Input() organisationId?: string;
  @Input({ required: true }) schemeYearId!: string;

  loading$: Observable<boolean | undefined | null>;
  files$: Observable<MultiFileUploadState>;
  files: FileList | null = null;
  fileNames: string[] = [];

  form: FormGroup<AddManufacturerNoteForm>;
  errors: { [key: string]: string } = {};

  constructor(private store: Store, backLinkProvider: BackLinkProvider) {
    backLinkProvider.clear();
    this.files$ = this.store.select(selectManufacturerNewNoteFiles);
    this.loading$ = this.store.select(selectAddManufacturerNoteLoading);
    this.form = new FormGroup({
      details: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
    });
  }

  ngOnInit() {
    this.store.dispatch(
      getManufacturerNewNoteFiles({
        organisationId: this.organisationId!,
        schemeYearId: this.schemeYearId,
      })
    );
  }

  ngOnDestroy() {
    this.store.dispatch(
      clearManufacturerNewNoteFiles({
        organisationId: this.organisationId!,
        schemeYearId: this.schemeYearId,
      })
    );
  }

  onFilesChange(fileList: FileList) {
    this.store.dispatch(
      uploadManufacturerNewNoteFile({
        organisationId: this.organisationId!,
        schemeYearId: this.schemeYearId,
        fileList: { ...fileList },
      })
    );
  }

  onDeleteFile(fileName: string) {
    this.store.dispatch(
      deleteManufacturerNewNoteFile({
        organisationId: this.organisationId!,
        schemeYearId: this.schemeYearId,
        fileName,
      })
    );
  }

  onSubmit() {
    this.errors = {};
    if (this.form.valid) {
      let command = this.form.getRawValue();
      this.store.dispatch(
        submitManufacturerNewNote({
          organisationId: this.organisationId!,
          schemeYearId: this.schemeYearId,
          command,
        })
      );
    } else {
      this.errors['details'] = 'Enter note details';
    }
  }
}
