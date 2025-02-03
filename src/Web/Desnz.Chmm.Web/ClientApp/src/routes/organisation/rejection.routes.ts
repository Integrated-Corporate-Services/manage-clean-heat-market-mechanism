import { Route } from '@angular/router';
import { RejectAccountComponent } from 'src/app/admin/reject-manufacturer/reject-account/reject-account.component';
import { RejectedAccountComponent } from 'src/app/admin/reject-manufacturer/rejected-account/rejected-account.component';
import { ConfirmAccountRejectionComponent } from 'src/app/admin/reject-manufacturer/confirm-account-rejection/confirm-account-rejection.component';

export const rejectionRoutes: Route[] = [
  {
    path: '',
    component: RejectAccountComponent,
  },
  {
    path: 'confirm',
    component: ConfirmAccountRejectionComponent,
  },
  {
    path: 'confirmation',
    component: RejectedAccountComponent,
  },
];
