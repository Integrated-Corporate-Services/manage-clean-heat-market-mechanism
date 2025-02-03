import { Route } from '@angular/router';
import { OrganisationHistoryComponent } from 'src/app/admin/history/organisation-history/organisation-history.component';
import { authGuard } from 'src/app/authentication/auth.guard';
import { changeManufacturerTypeRoutes } from './change-manufacturer-type.routes';
import { CheckAnswersComponent } from 'src/app/manufacturer/onboarding/check-answers/check-answers.component';

export const organisationRoutes: Route[] = [
  {
    path: 'register',
    loadChildren: () =>
      import('./registration.routes').then((mod) => mod.registrationRoutes),
  },
  {
    path: ':organisationId',
    children: [
      {
        path: 'licence-holders',
        canActivateChild: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        loadChildren: () =>
          import('./licence-holders.routes').then(
            (mod) => mod.licenceHoldersRoutes
          ),
      },
      {
        path: 'edit',
        canActivateChild: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        loadChildren: () =>
          import('./registration.routes').then((mod) => mod.registrationRoutes),
      },
      {
        path: 'account',
        canActivate: [
          authGuard([
            'Manufacturer',
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        component: CheckAnswersComponent,
      },
      {
        path: 'approve',
        canActivateChild: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        loadChildren: () =>
          import('./approval.routes').then((mod) => mod.approvalRoutes),
      },
      {
        path: 'reject',
        canActivateChild: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        loadChildren: () =>
          import('./rejection.routes').then((mod) => mod.rejectionRoutes),
      },
      {
        path: 'history',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        component: OrganisationHistoryComponent,
      },
      {
        path: 'users',
        canActivateChild: [
          authGuard([
            'Manufacturer',
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        loadChildren: () =>
          import('./account-management.routes').then(
            (mod) => mod.accountManagementRoutes
          ),
      },
      {
        path: 'change-manufacturer-type',
        canActivateChild: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        children: changeManufacturerTypeRoutes,
      },
    ],
  },
];
