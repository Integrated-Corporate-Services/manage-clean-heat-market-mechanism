import { Route } from '@angular/router';
import { SchemeConfigurationViewComponent } from '../../app/admin/scheme-configuration/view/scheme-configuration-view.component';
import { SchemeConfigurationEditFormComponent } from '../../app/admin/scheme-configuration/edit/form/scheme-configuration-edit-form.component';
import { SchemeConfigurationEditCheckAnswersComponent } from '../../app/admin/scheme-configuration/edit/check-answers/scheme-configuration-edit-check-answers.component';
import { SchemeConfigurationEditConfirmationComponent } from '../../app/admin/scheme-configuration/edit/confirmation/scheme-configuration-edit-confirmation.component';

export const schemeYearConfigurationRoutes: Route[] = [
  {
    path: '',
    redirectTo: 'view',
    pathMatch: 'full'
  },
  {
    path: 'view',
    component: SchemeConfigurationViewComponent
  },
  {
    path: 'edit',
    component: SchemeConfigurationEditFormComponent
  },
  {
    path: 'check-answers',
    component: SchemeConfigurationEditCheckAnswersComponent
  },
  {
    path: 'confirmation',
    component: SchemeConfigurationEditConfirmationComponent
  }
];
