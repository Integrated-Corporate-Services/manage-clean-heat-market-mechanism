import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, Subscription } from 'rxjs';
import { BackLinkProvider } from 'src/app/navigation/back-link/back-link.provider';
import {
  editAccount,
  storeIsOnBehalfOfGroup,
} from 'src/app/stores/onboarding/actions';
import {
  selectOrganisationDetailsLoading,
  selectOrganisationStructure,
} from 'src/app/stores/onboarding/selectors';
import { OnboardingFormMode } from '../models/mode';
import { GovukFileUploadComponent } from 'src/app/shared/components/govuk/govuk-file-upload/govuk-file-upload.component';
import { OrganisationStructure } from '../models/organisation-structure';
import {
  getFileNames,
  setFileValidationErrors,
} from 'src/app/shared/file-utils';

export interface OrganisationStructureForm {
  isOnBehalfOfGroup: FormControl<string>;
  files: FormControl<any>;
}

@Component({
  selector: 'organisation-structure-form',
  templateUrl: './organisation-structure-form.component.html',
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
export class OrganisationStructureFormComponent implements OnInit {
  @Input() mode: OnboardingFormMode = 'default';
  @Input() organisationId?: string;

  form: FormGroup<OrganisationStructureForm>;
  errors: { [key: string]: string } = {};

  loading$: Observable<boolean>;
  organisationStructure$: Observable<OrganisationStructure | null>;
  subscription = Subscription.EMPTY;

  files: FileList | null = null;
  fileNames: string[] = [];

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider
  ) {
    this.organisationStructure$ = this.store.select(
      selectOrganisationStructure
    );
    this.loading$ = this.store.select(selectOrganisationDetailsLoading);
    this.form = new FormGroup({
      isOnBehalfOfGroup: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      files: new FormControl(null),
    });
  }

  ngOnInit() {
    this.subscription = this.organisationStructure$.subscribe(
      (organisationStructure) => {
        if (organisationStructure) {
          this.form.patchValue({
            isOnBehalfOfGroup: organisationStructure.isOnBehalfOfGroup,
          });
          this.files = organisationStructure.files;
          this.fileNames = organisationStructure.fileNames ?? [];
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
    }
  }

  onFileChange(event: Event) {
    let fileInput = event.currentTarget as HTMLInputElement;
    this.files = fileInput.files;
    this.fileNames = getFileNames(this.files);
  }

  onSubmit() {
    this.errors = {};
    this.validateFiles();
    if (this.form.valid && !this.errors['files']) {
      let isOnBehalfOfGroup = this.form.getRawValue().isOnBehalfOfGroup;
      this.store.dispatch(
        storeIsOnBehalfOfGroup({
          organisationStructure: {
            isOnBehalfOfGroup: isOnBehalfOfGroup,
            files: this.files ? { ...this.files } : null,
            fileNames: this.fileNames,
          },
          mode: this.mode,
        })
      );
    } else {
      this.setValidationErrorMessages();
    }
  }

  validateFiles() {
    this.validateFileControl();
    setFileValidationErrors(this.files, this.errors);
  }

  validateFileControl() {
    if (this.mode === 'default' || this.mode == 'submit') {
      if (
        this.form.controls.isOnBehalfOfGroup.value === 'Yes' &&
        (!this.files || this.fileNames.length === 0)
      ) {
        this.errors['files'] = 'Select a file';
      }
    }
  }

  setValidationErrorMessages() {
    let validationErrors = this.form.controls.isOnBehalfOfGroup.errors;
    if (validationErrors && validationErrors['required']) {
      this.errors['isOnBehalfOfGroup'] =
        'Select if this registration is on behalf of a group of organisations';
    }
  }

  ngOnDestroy() {
    this.backLinkProvider.clear();
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
