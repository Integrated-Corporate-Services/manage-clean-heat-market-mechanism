import { Route } from '@angular/router';
import { AmendObligationFormComponent } from 'src/app/admin/manufacturers/amend-obligation/amend-obligation-form/amend-obligation-form.component';
import { AmendObligationCheckAnswersComponent } from 'src/app/admin/manufacturers/amend-obligation/check-answers/amend-obligation-check-answers.component';
import { AmendObligationConfirmationComponent } from 'src/app/admin/manufacturers/amend-obligation/confirmation/amend-obligation-confirmation.component';

export const amendObligationRoutes: Route[] = [
  {
    path: '',
    component: AmendObligationFormComponent,
  },
  {
    path: 'check-answers',
    component: AmendObligationCheckAnswersComponent,
  },
  {
    path: 'confirmation',
    component: AmendObligationConfirmationComponent,
  },
];
