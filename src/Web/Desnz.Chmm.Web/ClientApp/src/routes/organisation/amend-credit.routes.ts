import { Route } from '@angular/router';
import { AmendCreditFormComponent } from 'src/app/admin/manufacturers/amend-credit/amend-credit-form/amend-credit-form.component';
import { AmendCreditCheckAnswersComponent } from 'src/app/admin/manufacturers/amend-credit/check-answers/amend-credit-check-answers.component';
import { AmendCreditConfirmationComponent } from 'src/app/admin/manufacturers/amend-credit/confirmation/amend-credit-confirmation.component';

export const amendCreditRoutes: Route[] = [
  {
    path: '',
    component: AmendCreditFormComponent,
  },
  {
    path: 'check-answers',
    component: AmendCreditCheckAnswersComponent,
  },
  {
    path: 'confirmation',
    component: AmendCreditConfirmationComponent,
  },
];
