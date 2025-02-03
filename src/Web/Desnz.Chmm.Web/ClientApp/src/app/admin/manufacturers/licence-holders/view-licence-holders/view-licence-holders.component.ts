import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Observable } from 'rxjs';
import { HttpState } from '../../../../stores/http-state';
import { selectLinkedLicenceHolders } from '../../../../stores/licence-holders/selectors';
import {
  clearLinkLicenceHolderFormValue,
  getCurrentSchemeYear,
  selectLinkedLincenceHolder,
} from '../../../../stores/licence-holders/actions';
import { LicenceHolderLinkDto } from 'src/app/stores/licence-holders/dto/licence-holder-link.dto';

@Component({
  selector: 'view-licence-holders',
  templateUrl: './view-licence-holders.component.html',
  styleUrls: ['./view-licence-holders.component.css'],
  standalone: true,
  imports: [RouterLink, NgFor, NgIf, AsyncPipe],
})
export class ViewLicenceHoldersComponent implements OnInit {
  @Input({ required: true }) organisationId!: string;

  licenceHolderLinks$: Observable<HttpState<LicenceHolderLinkDto[]>>;

  constructor(private store: Store) {
    this.licenceHolderLinks$ = this.store.select(selectLinkedLicenceHolders);
  }

  ngOnInit() {
    this.store.dispatch(
      getCurrentSchemeYear({ organisationId: this.organisationId })
    );
  }

  onLink() {
    this.store.dispatch(
      clearLinkLicenceHolderFormValue({ organisationId: this.organisationId })
    );
  }

  onSelectLicenceHolder(licenceHolderLink: LicenceHolderLinkDto) {
    this.store.dispatch(
      selectLinkedLincenceHolder({
        licenceHolderLink: licenceHolderLink,
        organisationId: this.organisationId,
      })
    );
  }
}
