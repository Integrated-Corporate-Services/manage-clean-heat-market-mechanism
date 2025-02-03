import { Route } from '@angular/router';
import { ApproveAccountComponent } from 'src/app/admin/approve-manufacturer/approve-account/approve-account.component';
import { ApprovedAccountComponent } from 'src/app/admin/approve-manufacturer/approved-account/approved-account.component';
import { ConfirmAccountApprovalComponent } from 'src/app/admin/approve-manufacturer/confirm-account-approval/confirm-account-approval.component';

export const approvalRoutes: Route[] = [
  {
    path: '',
    component: ApproveAccountComponent,
  },
  {
    path: 'confirm',
    component: ConfirmAccountApprovalComponent,
  },
  {
    path: 'confirmation',
    component: ApprovedAccountComponent,
  },
];
