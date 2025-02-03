import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import {
  getUnlinkedLicenceHolders,
  linkLicenceHolder,
} from '../../../../stores/licence-holders/actions';
import { FormsModule } from '@angular/forms';
import { Observable, filter, mergeMap } from 'rxjs';
import { HttpState } from '../../../../stores/http-state';
import { LicenceHolderDto } from '../../../../stores/licence-holders/dto/licence-holder';
import {
  selectStarOfLink,
  selectUnlinkedLicenceHolders,
} from '../../../../stores/licence-holders/selectors';
import { getManufacturers } from '../../../../stores/account-management/actions';
import { ViewOrganisationDto } from '../../../../stores/onboarding/dtos/view-organisation-dto';
import { selectManufacturers } from '../../../../stores/account-management/selectors';

@Component({
  selector: 'confirm-link-licence-holder',
  templateUrl: './confirm-link-licence-holder.component.html',
  standalone: true,
  imports: [FormsModule, RouterLink, NgFor, NgIf, AsyncPipe],
})
export class ConfirmLinkLicenceHolderComponent implements OnInit {
  @Input() organisationId?: string;
  @Input() licenceHolderId?: string;

  manufacturer$: Observable<ViewOrganisationDto>;
  licenceHolder$: Observable<LicenceHolderDto>;
  selectStarOfLink$: Observable<string | null>;

  constructor(private store: Store) {
    this.licenceHolder$ = this.store.select(selectUnlinkedLicenceHolders).pipe(
      filter((x: HttpState<LicenceHolderDto[]>) => !!x.data),
      mergeMap((x: HttpState<LicenceHolderDto[]>) => x.data!),
      filter((x: LicenceHolderDto) => x.id == this.licenceHolderId)
    );

    this.manufacturer$ = this.store.select(selectManufacturers).pipe(
      filter((response: HttpState<ViewOrganisationDto[]>) => !!response.data),
      mergeMap((response: HttpState<ViewOrganisationDto[]>) => response.data!),
      filter(
        (manufacturer: ViewOrganisationDto) =>
          manufacturer.id == this.organisationId
      )
    );

    this.selectStarOfLink$ = this.store.select(selectStarOfLink);
  }

  ngOnInit() {
    this.store.dispatch(getManufacturers());
    this.store.dispatch(getUnlinkedLicenceHolders());
  }

  link() {
    this.store.dispatch(
      linkLicenceHolder({
        licenceHolderId: this.licenceHolderId!,
        organisationId: this.organisationId!,
      })
    );
  }
}
