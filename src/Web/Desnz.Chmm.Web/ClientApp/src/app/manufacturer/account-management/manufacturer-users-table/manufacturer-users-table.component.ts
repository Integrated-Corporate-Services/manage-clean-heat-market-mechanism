import { Component, Input, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { selectManufacturerUsers } from '../../../stores/account-management/selectors';
import { clearManufacturerUserBeingEdited, clearManufacturerUserBeingInvited, clearUserBeingEdited, getManufacturerUsers } from '../../../stores/account-management/actions';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ChmmUser } from 'src/app/stores/account-management/dtos/chmm-user';
import { HttpState } from 'src/app/stores/http-state';
import { ManufacturerListStatusComponent } from '../manufacturer-list-status/manufacturer-list-status.component';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { manufacturerUserStatusStyle } from '../styles';
import { selectIsAdmin } from 'src/app/stores/auth/selectors';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'manufacturer-users-table',
  templateUrl: './manufacturer-users-table.component.html',
  styles: [manufacturerUserStatusStyle],
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe,
    ManufacturerListStatusComponent
  ],
})
export class ManufacturerUsersTableComponent implements OnInit {

  @Input({ required: true }) organisationId?: string;
  isAdmin: boolean | null = null;

  accounts$: Observable<HttpState<ChmmUser[]>>;

  constructor(private store: Store, backLinkProvider: BackLinkProvider) {
    backLinkProvider.clear();
    this.accounts$ = this.store.select(selectManufacturerUsers);

    this.store
      .select(selectIsAdmin)
      .pipe(takeUntilDestroyed())
      .subscribe((isAdmin) => {
        this.isAdmin = isAdmin;
      });
  }

  ngOnInit() {
    this.store.dispatch(clearManufacturerUserBeingInvited());
    this.store.dispatch(clearManufacturerUserBeingEdited());
    this.store.dispatch(getManufacturerUsers({
      organisationId: this.organisationId!
    }));
  }

  onInvite() {
    this.store.dispatch(clearUserBeingEdited());
  }
}
