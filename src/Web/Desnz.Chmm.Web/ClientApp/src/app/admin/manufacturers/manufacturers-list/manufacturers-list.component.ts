import { Component, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { HttpState } from 'src/app/stores/http-state';
import { selectManufacturers } from '../../../stores/account-management/selectors';
import { getManufacturers } from '../../../stores/account-management/actions';
import { FormsModule } from '@angular/forms';
import { ManufacturersListFilterPipe } from './manufacturers-list-filter.pipe';
import { ManufacturersListStatusComponent } from '../manufacturers-list-status/manufacturers-list-status.component';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { ViewOrganisationDto } from 'src/app/stores/onboarding/dtos/view-organisation-dto';

@Component({
  selector: 'manufacturers-list',
  templateUrl: './manufacturers-list.component.html',
  styleUrls: ['./manufacturers-list.component.css'],
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe,
    ManufacturersListFilterPipe,
    ManufacturersListStatusComponent,
  ],
})
export class ManufacturersListComponent implements OnInit {
  manufacturers$: Observable<HttpState<ViewOrganisationDto[]>>;

  search: string = '';
  showArchived: boolean = false;

  constructor(private store: Store, backLinkProvider: BackLinkProvider) {
    backLinkProvider.clear();
    this.manufacturers$ = this.store.select(selectManufacturers);
  }

  ngOnInit() {
    this.store.dispatch(getManufacturers());
  }
}
