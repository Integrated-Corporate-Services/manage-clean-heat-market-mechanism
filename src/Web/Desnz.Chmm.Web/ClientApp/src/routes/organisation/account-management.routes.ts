import { Route } from '@angular/router';
import { ManufacturerUsersTableComponent } from '../../app/manufacturer/account-management/manufacturer-users-table/manufacturer-users-table.component';
import { ManufacturerInviteUserComponent } from '../../app/manufacturer/account-management/invite-user/manufacturer-invite-user.component';
import { ManufacturerConfirmInviteUserComponent } from '../../app/manufacturer/account-management/confirm-invite-user/manufacturer-confirm-invite-user.component';
import { ManufacturerInvitedUserComponent } from '../../app/manufacturer/account-management/invited-user/manufacturer-invited-user.component';
import { ManufacturerEditUserComponent } from '../../app/manufacturer/account-management/edit-user/manufacturer-edit-user.component';
import { ManufacturerDeactivateUserComponent } from '../../app/manufacturer/account-management/deactivate-user/manufacturer-deactivate-user.component';
import { ManufacturerDeactivatedUserComponent } from '../../app/manufacturer/account-management/deactivated-user/manufacturer-deactivated-user.component';
import { ManufacturerViewUserComponent } from '../../app/manufacturer/account-management/edit-user/manufacturer-view-user.component';
import { ManufacturerEditUserCheckAnswersComponent } from '../../app/manufacturer/account-management/edit-user-check-answers/manufacturer-edit-user-check-answers.component';
import { ManufacturerEditUserConfirmationComponent } from '../../app/manufacturer/account-management/edit-user-confirm/manufacturer-edit-user-confirmation.component';

export const accountManagementRoutes: Route[] = [
  {
    path: '',
    component: ManufacturerUsersTableComponent,
  },
  {
    path: 'invited',
    component: ManufacturerInvitedUserComponent,
  },
  {
    path: 'invite',
    component: ManufacturerInviteUserComponent,
  },
  {
    path: 'confirm-invite',
    component: ManufacturerConfirmInviteUserComponent,
  },
  {
    path: ':userId',
    children: [
      {
        path: 'view',
        component: ManufacturerViewUserComponent
      },
      {
        path: 'edit',
        component: ManufacturerEditUserComponent
      },
      {
        path: 'check-answers',
        component: ManufacturerEditUserCheckAnswersComponent
      },
      {
        path: 'confirmation',
        component: ManufacturerEditUserConfirmationComponent
      },
      {
        path: 'deactivate',
        component: ManufacturerDeactivateUserComponent
      },
      {
        path: 'deactivated',
        component: ManufacturerDeactivatedUserComponent
      }
    ]
  }
];
