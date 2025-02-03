import { Route } from '@angular/router';
import { ChangeIsNonSchemeParticipantCheckAnswersComponent } from 'src/app/admin/manufacturers/change-is-non-scheme-participant/change-is-non-scheme-participant-check-answers/change-is-non-scheme-participant-check-answers.component';
import { ChangeIsNonSchemeParticipantConfirmationComponent } from 'src/app/admin/manufacturers/change-is-non-scheme-participant/change-is-non-scheme-participant-confirmation/change-is-non-scheme-participant-confirmation.component';
import { ChangeIsNonSchemeParticipantFormComponent } from 'src/app/admin/manufacturers/change-is-non-scheme-participant/change-is-non-scheme-participant-form/change-is-non-scheme-participant-form.component';

export const changeManufacturerTypeRoutes: Route[] = [
  {
    path: '',
    component: ChangeIsNonSchemeParticipantFormComponent,
  },
  {
    path: 'check-answers',
    component: ChangeIsNonSchemeParticipantCheckAnswersComponent,
  },
  {
    path: 'confirmation',
    component: ChangeIsNonSchemeParticipantConfirmationComponent,
  },
];
