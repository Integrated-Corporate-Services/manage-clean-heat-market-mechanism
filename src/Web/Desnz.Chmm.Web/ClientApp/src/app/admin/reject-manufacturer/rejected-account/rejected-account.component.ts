import { Component, Input, OnInit } from '@angular/core';
import { NgIf, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { selectManufacturers } from '../../../stores/account-management/selectors';
import { Observable, filter, mergeMap } from 'rxjs';
import { ViewOrganisationDto } from '../../../stores/onboarding/dtos/view-organisation-dto';
import { HttpState } from '../../../stores/http-state';
import { getManufacturers } from '../../../stores/account-management/actions';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';

@Component({
  selector: 'rejected-manufacturer-account',
  templateUrl: './rejected-account.component.html',
  standalone: true,
  imports: [NgIf, AsyncPipe, RouterLink],
})
export class RejectedAccountComponent implements OnInit {
  @Input() organisationId?: string;

  manufacturer$: Observable<ViewOrganisationDto> | null = null;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider
  ) {
    this.store.dispatch(getManufacturers());
  }

  ngOnInit() {
    this.backLinkProvider.clear();
    this.manufacturer$ = this.store.select(selectManufacturers).pipe(
      filter((response: HttpState<ViewOrganisationDto[]>) => !!response.data),
      mergeMap((response: HttpState<ViewOrganisationDto[]>) => response.data!),
      filter(
        (manufacturer: ViewOrganisationDto) =>
          manufacturer.id == this.organisationId
      )
    );
  }
}
