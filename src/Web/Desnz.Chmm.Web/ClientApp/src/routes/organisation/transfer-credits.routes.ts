import { Route } from '@angular/router';
import { TransferCreditsCheckAnswersComponent } from 'src/app/manufacturer/credits/check-answers/transfer-credits-check-answers.component';
import { TransferCreditsConfirmationComponent } from 'src/app/manufacturer/credits/confirmation/transfer-credits-confirmation.component';
import { TransferCreditsFormComponent } from 'src/app/manufacturer/credits/transfer-credits-form/transfer-credits-form.component';

export const trasnferCreditsRoutes: Route[] = [
  {
    path: '',
    component: TransferCreditsFormComponent,
  },
  {
    path: 'check-answers',
    component: TransferCreditsCheckAnswersComponent,
  },
  {
    path: 'confirmation',
    component: TransferCreditsConfirmationComponent,
  },
];
