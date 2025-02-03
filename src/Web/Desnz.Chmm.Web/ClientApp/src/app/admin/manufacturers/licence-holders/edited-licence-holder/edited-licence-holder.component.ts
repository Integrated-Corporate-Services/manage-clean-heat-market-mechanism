import { Component, Input } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { EditLinkedLicenceHolderFormValue } from '../edit-linked-licence-holder/edit-linked-licence-holder.component';
import {
  selectEditLinkedLicenceHolderFormValue,
  selectSelectedLicenceHolderLink,
} from 'src/app/stores/licence-holders/selectors';
import { Organisation } from 'src/app/navigation/models/organisation';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';
import { navigationOrganisationSessionKey } from 'src/app/shared/constants';
import { LicenceHolderLinkDto } from 'src/app/stores/licence-holders/dto/licence-holder-link.dto';

@Component({
  selector: 'edited-licence-holder',
  templateUrl: './edited-licence-holder.component.html',
  standalone: true,
  imports: [FormsModule, RouterLink, NgFor, NgIf, AsyncPipe],
})
export class EditedLicenceHolderComponent {
  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) licenceHolderId!: string;

  licenceHolderLink$: Observable<LicenceHolderLinkDto | null>;
  editLinkedLicenceHolderFormValue$: Observable<EditLinkedLicenceHolderFormValue | null>;
  organisation: Organisation | null;

  constructor(
    private store: Store,
    private sessionStorageService: SessionStorageService
  ) {
    this.licenceHolderLink$ = this.store.select(
      selectSelectedLicenceHolderLink
    );
    this.editLinkedLicenceHolderFormValue$ = this.store.select(
      selectEditLinkedLicenceHolderFormValue
    );
    this.organisation = this.sessionStorageService.getObject<Organisation>(
      navigationOrganisationSessionKey
    );
  }
}
