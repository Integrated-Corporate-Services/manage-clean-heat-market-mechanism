import { Component, Input, OnInit } from '@angular/core';
import { AsyncPipe, NgIf } from '@angular/common';
import { FormControl } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import {
  activateUser,
  deactivateUser,
} from 'src/app/stores/account-management/actions';
import { Observable } from 'rxjs';
import { ChmmUser } from 'src/app/stores/account-management/dtos/chmm-user';
import { selectUserBeingEdited } from 'src/app/stores/account-management/selectors';
import { HttpState } from 'src/app/stores/http-state';

export interface UserForm {
  name: FormControl<string>;
  email: FormControl<string>;
  permission: FormControl<string>;
}

@Component({
  selector: 'confirm-status-change',
  templateUrl: './confirm-status-change.component.html',
  standalone: true,
  imports: [NgIf, AsyncPipe, RouterLink],
})
export class ConfirmStatusChangeComponent implements OnInit {
  user$: Observable<HttpState<ChmmUser | null>>;

  constructor(private store: Store) {
    this.user$ = this.store.select(selectUserBeingEdited);
  }

  ngOnInit() {}

  onStatusChange(status: string) {
    if (status === 'Active') {
      this.store.dispatch(deactivateUser());
    } else if (status === 'Inactive') {
      this.store.dispatch(activateUser());
    }
  }
}
