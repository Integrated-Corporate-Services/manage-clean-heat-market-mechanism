import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import {
  editLinkedLicenceHolder,
  getUnlinkedLicenceHolders,
} from '../../../../stores/licence-holders/actions';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import {
    selectEditLinkedLicenceHolderError,
  selectEditLinkedLicenceHolderFormValue,
  selectEditLinkedLicenceHolderLoading,
  selectEndOfLink,
  selectSelectedLicenceHolderLink,
} from '../../../../stores/licence-holders/selectors';
import { getManufacturers } from '../../../../stores/account-management/actions';
import { EditLinkedLicenceHolderFormValue } from '../edit-linked-licence-holder/edit-linked-licence-holder.component';
import { Organisation } from 'src/app/navigation/models/organisation';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';
import { navigationOrganisationSessionKey } from 'src/app/shared/constants';
import { LicenceHolderLinkDto } from 'src/app/stores/licence-holders/dto/licence-holder-link.dto';

@Component({
  selector: 'confirm-edit-linked-licence-holder',
  templateUrl: './confirm-edit-linked-licence-holder.component.html',
  standalone: true,
  imports: [FormsModule, RouterLink, NgIf, AsyncPipe],
})
export class ConfirmEditLinkedLicenceHolderComponent implements OnInit {
  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) licenceHolderId!: string;

  selectEndOfLink$: Observable<string | null>;
  licenceHolderLink$: Observable<LicenceHolderLinkDto | null>;
  editLinkedLicenceHolderFormValue$: Observable<EditLinkedLicenceHolderFormValue | null>;
  loading$: Observable<boolean>;
  error$: Observable<any>;
  organisation: Organisation | null;

  constructor(
    private store: Store,
    private sessionStorageService: SessionStorageService
  ) {
    this.selectEndOfLink$ = this.store.select(
      selectEndOfLink
    )
    this.licenceHolderLink$ = this.store.select(
      selectSelectedLicenceHolderLink
    );
    this.editLinkedLicenceHolderFormValue$ = this.store.select(
      selectEditLinkedLicenceHolderFormValue
    );
    this.loading$ = this.store.select(selectEditLinkedLicenceHolderLoading);
    this.error$ = this.store.select(selectEditLinkedLicenceHolderError);
    this.organisation = this.sessionStorageService.getObject<Organisation>(
      navigationOrganisationSessionKey
    );
  }

  ngOnInit() {
    this.store.dispatch(getManufacturers());
    this.store.dispatch(getUnlinkedLicenceHolders());
  }

  edit() {
    this.store.dispatch(
      editLinkedLicenceHolder({
        organisationId: this.organisationId,
        licenceHolderId: this.licenceHolderId,
      })
    );
  }
}
