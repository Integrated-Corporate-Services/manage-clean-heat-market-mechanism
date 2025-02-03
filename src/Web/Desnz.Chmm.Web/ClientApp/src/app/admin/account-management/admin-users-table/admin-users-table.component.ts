import { Component, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { selectAdminUsers } from '../../../stores/account-management/selectors';
import {
  clearUserBeingEdited,
  getAdminUsers,
} from '../../../stores/account-management/actions';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { ChmmUser } from 'src/app/stores/account-management/dtos/chmm-user';
import { AdminListStatusComponent } from '../admin-list-status/admin-list-status.component';
import { HttpState } from 'src/app/stores/http-state';

@Component({
  selector: 'admin-users-table',
  templateUrl: './admin-users-table.component.html',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe,
    AdminListStatusComponent,
  ],
})
export class AdminUsersTableComponent implements OnInit {
  admins$: Observable<HttpState<ChmmUser[]>>;

  constructor(private store: Store) {
    this.admins$ = this.store.select(selectAdminUsers);
  }

  ngOnInit() {
    this.store.dispatch(getAdminUsers());
  }

  onInvite() {
    this.store.dispatch(clearUserBeingEdited());
  }
}
