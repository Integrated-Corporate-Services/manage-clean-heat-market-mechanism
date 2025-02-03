import { Component, Input, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { FormControl } from '@angular/forms';
import { selectUserBeingEdited } from 'src/app/stores/account-management/selectors';
import { Observable } from 'rxjs';
import { ChmmUser } from 'src/app/stores/account-management/dtos/chmm-user';
import { RouterLink } from '@angular/router';
import {
  clearUserBeingEdited,
  inviteAdminUser,
  updateUserDetails,
} from 'src/app/stores/account-management/actions';
import { AdminListStatusComponent } from '../admin-list-status/admin-list-status.component';
import { HttpState } from 'src/app/stores/http-state';

export interface UserForm {
  name: FormControl<string>;
  email: FormControl<string>;
  permission: FormControl<string>;
}

@Component({
  selector: 'check-user-details',
  templateUrl: './check-user-details.component.html',
  standalone: true,
  imports: [NgIf, AsyncPipe, RouterLink, AdminListStatusComponent],
})
export class CheckUserDetailsComponent implements OnInit {
  @Input() edit: string = 'false';

  user$: Observable<HttpState<ChmmUser | null>>;

  constructor(private store: Store) {
    this.user$ = this.store.select(selectUserBeingEdited);
  }

  ngOnInit() {}

  onSave() {
    if (this.edit === 'true') {
      this.store.dispatch(updateUserDetails());
    } else {
      this.store.dispatch(inviteAdminUser());
    }
  }
}
