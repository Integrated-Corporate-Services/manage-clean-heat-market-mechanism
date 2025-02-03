import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { ChmmRole } from 'src/app/stores/account-management/dtos/chmm-role';
import {
  selectRoles,
  selectUserBeingEdited,
  selectUsers,
} from 'src/app/stores/account-management/selectors';
import { RouterLink } from '@angular/router';
import { ChmmUser } from 'src/app/stores/account-management/dtos/chmm-user';
import {
  activateUser,
  clearUserBeingEdited,
  deactivateUser,
  getAdminRoles,
  getUser,
  getUsers,
  storeUserFormValue,
} from 'src/app/stores/account-management/actions';
import { AdminListStatusComponent } from '../admin-list-status/admin-list-status.component';
import { HttpState } from 'src/app/stores/http-state';

export interface UserForm {
  name: FormControl<string>;
  email: FormControl<string>;
  roleId: FormControl<string>;
  status: FormControl<string>;
}

@Component({
  selector: 'user-form',
  templateUrl: './user-form.component.html',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    AsyncPipe,
    ReactiveFormsModule,
    RouterLink,
    AdminListStatusComponent,
  ],
})
export class UserFormComponent implements OnInit, OnDestroy {
  // Indicates whether an existing user is being edited
  @Input() userId: string | null = null;
  // Indicates whether data for an existing user should be fetched from the server
  @Input() fetchData: string = 'false';

  users: ChmmUser[] | null = null;
  user: ChmmUser | null = null;

  user$: Observable<HttpState<ChmmUser | null>>;
  roles$: Observable<HttpState<ChmmRole[]>>;
  users$: Observable<HttpState<ChmmUser[]>>;

  userSub: Subscription = Subscription.EMPTY;
  usersSub: Subscription = Subscription.EMPTY;
  rolesSub: Subscription = Subscription.EMPTY;

  form: FormGroup<UserForm>;
  nameError: string | null = null;
  emailError: string | null = null;
  permissionError: string | null = null;

  constructor(private store: Store) {
    this.user$ = this.store.select(selectUserBeingEdited);
    this.roles$ = this.store.select(selectRoles);
    this.users$ = this.store.select(selectUsers);

    this.form = new FormGroup({
      name: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.maxLength(100)],
      }),
      email: new FormControl('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.email,
          Validators.maxLength(100),
        ],
      }),
      roleId: new FormControl('', {
        nonNullable: true,
        validators: Validators.required,
      }),
      status: new FormControl('Active', {
        nonNullable: true,
        validators: Validators.required,
      }),
    });
  }

  ngOnInit() {
    this.store.dispatch(getAdminRoles());
    // this.store.dispatch(getUsers());

    this.rolesSub = this.roles$.subscribe((roles) => {
      if (!roles.data) return;
      this.form.patchValue({
        roleId: roles.data[0].id
      });
    });

    this.userSub = this.user$.subscribe((user) => {
      if (user.data) {
        this.user = user.data;

        this.form.patchValue({
          name: user.data.name,
          email: user.data.email,
          roleId: user.data.chmmRoles[0].id,
          status: user.data.status,
        });
      }
    });

    // this.usersSub = this.users$.subscribe((users) => {
    //   this.users = users.data;
    // });

    if (this.userId) {
      this.form.controls.email.disable();

      if (this.fetchData === 'true') {
        this.store.dispatch(getUser({ userId: this.userId }));
      }
    }
  }

  onSubmit() {
    this.nameError = null;
    this.emailError = null;
    this.permissionError = null;

    let emailControl = this.form.controls.email;
    // let emailAlreadyInUse = false;
    if (this.users) {
      const emailValue = emailControl.value.toUpperCase();
      // emailAlreadyInUse =
      //   !this.userId &&
      //   this.users.find((u) => u.email.toUpperCase() == emailValue)
      //     ? true
      //     : false;
    }

    if (this.form.valid) {
      this.store.dispatch(
        storeUserFormValue({
          userFormValue: this.form.getRawValue(),
          edit: this.userId ? true : false,
        })
      );
    } else {
      let nameControlErrors = this.form.controls.name.errors;
      if (nameControlErrors) {
        if (nameControlErrors['required']) {
          this.nameError = 'Enter name';
        } else if (nameControlErrors['maxlength']) {
          this.nameError = 'Name cannot exceed 100 characters in length';
        }
      }

      let emailControlErrors = emailControl.errors;
      if (emailControlErrors) {
        if (emailControlErrors['required']) {
          this.emailError = 'Enter email address';
        } else if (emailControlErrors['maxlength']) {
          this.emailError = 'Email cannot exceed 100 characters in length';
        } else if (emailControlErrors['email']) {
          this.emailError =
            'Enter an email address in the correct format, like name@example.com';
        }
      }

      // if (emailAlreadyInUse) {
      //   this.emailError = 'Email must not already be in use';
      // }

      if (this.form.controls.roleId.errors) {
        this.permissionError = 'Select a permission level';
      }
    }
  }

  onCancel() {
    this.store.dispatch(clearUserBeingEdited());
  }

  onActivate() {
    this.store.dispatch(activateUser());
  }

  onDeactivate() {
    this.store.dispatch(deactivateUser());
  }

  ngOnDestroy() {
    if (this.userSub) {
      this.userSub.unsubscribe();
    }
    if (this.usersSub) {
      this.usersSub.unsubscribe();
    }
    if (this.rolesSub) {
      this.rolesSub.unsubscribe();
    }
  }
}
