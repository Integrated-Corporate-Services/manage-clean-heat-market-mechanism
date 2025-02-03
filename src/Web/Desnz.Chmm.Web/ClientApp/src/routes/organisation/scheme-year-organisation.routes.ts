import { Route } from '@angular/router';
import { OrganisationSummaryComponent } from 'src/app/manufacturer/organisation-summary/organisation-summary/organisation-summary.component';
import { TransferHistoryComponent } from '../../app/manufacturer/organisation-summary/transfer-history/transfer-history.component';
import { authGuard } from 'src/app/authentication/auth.guard';
import { TransferContactsComponent } from '../../app/manufacturer/organisation-summary/transfer-contacts/transfer-contacts.component';

export const schemeYearOrganisationRoutes: Route[] = [
  {
    path: ':organisationId',
    children: [
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
        component: OrganisationSummaryComponent,
      },
      {
        path: 'transfer-history',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
            'Manufacturer',
          ]),
        ],
        component: TransferHistoryComponent
      },
      {
        path: 'transfer-contacts',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
            'Manufacturer',
          ]),
        ],
        component: TransferContactsComponent,
      },
      {
        path: 'boiler-sales',
        loadChildren: () =>
          import('./boiler-sales.routes').then((mod) => mod.boilerSalesRoutes),
      },
      {
        path: 'heat-pumps',
        loadChildren: () =>
          import('./heat-pumps.routes').then((mod) => mod.heatPumpsRoutes),
      },
      {
        path: 'transfer-credits',
        canActivateChild: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
            'Manufacturer',
          ]),
        ],
        loadChildren: () =>
          import('./transfer-credits.routes').then(
            (mod) => mod.trasnferCreditsRoutes
          ),
      },
      {
        path: 'amend-credits',
        canActivateChild: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        loadChildren: () =>
          import('./amend-credit.routes').then((mod) => mod.amendCreditRoutes),
      },
      {
        path: 'amend-obligation',
        canActivateChild: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        loadChildren: () =>
          import('./amend-obligation.routes').then(
            (mod) => mod.amendObligationRoutes
          ),
      },
      {
        path: 'notes',
        canActivateChild: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        loadChildren: () =>
          import('./notes.routes').then((mod) => mod.notesRoutes),
      },
    ],
  },
];
