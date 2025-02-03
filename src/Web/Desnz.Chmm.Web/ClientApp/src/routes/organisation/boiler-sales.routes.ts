import { Route } from '@angular/router';
import { ConfirmBoilerSalesApprovalComponent } from 'src/app/admin/manufacturers/boiler-sales/confirm-boiler-sales-approval/confirm-account-approval.component';
import { authGuard } from 'src/app/authentication/auth.guard';
import { BoilerSalesAnnualCheckAnswersComponent } from 'src/app/manufacturer/boiler-sales/annual/check-answers/boiler-sales-annual-check-answers.component';
import { BoilerSalesAnnualConfirmationComponent } from 'src/app/manufacturer/boiler-sales/annual/confirmation/boiler-sales-annual-confirmation.component';
import { BoilerSalesAnnualFormComponent } from 'src/app/manufacturer/boiler-sales/annual/form/boiler-sales-annual-form.component';
import { BoilerSalesQuarterlyConfirmationComponent } from 'src/app/manufacturer/boiler-sales/quarterly/confirmation/boiler-sales-quarterly-confirmation.component';
import { BoilerSalesQuarterlyFormComponent } from 'src/app/manufacturer/boiler-sales/quarterly/form/boiler-sales-quarterly-form.component';
import { BoilerSalesQuarterlyCheckAnswersComponent } from 'src/app/manufacturer/boiler-sales/quarterly/check-answers/boiler-sales-quarterly-check-answers.component';
import { BoilerSalesSummaryComponent } from 'src/app/manufacturer/boiler-sales/summary/boiler-sales-summary.component';

export const boilerSalesRoutes: Route[] = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'summary',
  },
  {
    path: 'summary',
    canActivate: [
      authGuard([
        'Regulatory Officer',
        'Senior Technical Officer',
        'Principal Technical Officer',
        'Manufacturer',
      ]),
    ],
    component: BoilerSalesSummaryComponent,
  },
  {
    path: 'annual',
    children: [
      {
        path: 'submit',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
            'Manufacturer',
          ]),
        ],
        component: BoilerSalesAnnualFormComponent,
      },
      {
        path: 'edit',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        component: BoilerSalesAnnualFormComponent,
      },
      {
        path: 'check-answers',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
            'Manufacturer',
          ]),
        ],
        component: BoilerSalesAnnualCheckAnswersComponent,
      },
      {
        path: 'confirmation',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
            'Manufacturer',
          ]),
        ],
        component: BoilerSalesAnnualConfirmationComponent,
      },
      {
        path: 'approve',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        component: ConfirmBoilerSalesApprovalComponent,
      },
    ],
  },
  {
    path: 'quarter/:schemeYearQuarterId/:mode',
    canActivateChild: [
      authGuard([
        'Regulatory Officer',
        'Senior Technical Officer',
        'Principal Technical Officer',
        'Manufacturer',
      ]),
    ],
    children: [
      {
        path: '',
        component: BoilerSalesQuarterlyFormComponent
      },
      {
        path: 'check-answers',
        component: BoilerSalesQuarterlyCheckAnswersComponent,
      },
      {
        path: 'confirmation',
        component: BoilerSalesQuarterlyConfirmationComponent,
      },
    ],
  },
];
