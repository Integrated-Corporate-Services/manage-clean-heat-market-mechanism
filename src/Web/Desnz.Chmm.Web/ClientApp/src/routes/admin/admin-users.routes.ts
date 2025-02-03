import { Route } from '@angular/router';
import { AdminUsersTableComponent } from 'src/app/admin/account-management/admin-users-table/admin-users-table.component';
import { CheckUserDetailsComponent } from 'src/app/admin/account-management/check-user-details/check-user-details.component';
import { ConfirmStatusChangeComponent } from 'src/app/admin/account-management/confirm-status-change/confirm-status-change.component';
import { ConfirmationComponent } from 'src/app/admin/account-management/confirmation/confirmation.component';
import { UserFormComponent } from 'src/app/admin/account-management/user-form/user-form.component';
import { authGuard } from 'src/app/authentication/auth.guard';

export const adminUsersRoutes: Route[] = [
  {
    path: '',
    canActivate: [
      authGuard([
        'Regulatory Officer',
        'Senior Technical Officer',
        'Principal Technical Officer',
      ]),
    ],
    component: AdminUsersTableComponent,
  },
  {
    path: 'invite',
    canActivateChild: [
      authGuard([
        'Regulatory Officer',
        'Senior Technical Officer',
        'Principal Technical Officer',
      ]),
    ],
    children: [
      {
        path: '',
        component: UserFormComponent,
      },
      {
        path: 'check-answers',
        component: CheckUserDetailsComponent,
      },
      {
        path: 'confirmation',
        component: ConfirmationComponent,
      },
    ],
  },
  {
    path: 'edit/:userId',
    canActivateChild: [
      authGuard([
        'Regulatory Officer',
        'Senior Technical Officer',
        'Principal Technical Officer',
      ]),
    ],
    children: [
      {
        path: '',
        component: UserFormComponent,
      },
      {
        path: 'check-answers',
        component: CheckUserDetailsComponent,
      },
      {
        path: 'confirmation',
        component: ConfirmationComponent,
      },
      {
        path: 'confirm-status-change',
        component: ConfirmStatusChangeComponent,
      },
    ],
  },
];
