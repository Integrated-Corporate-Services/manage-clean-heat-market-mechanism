import { Routes } from '@angular/router';
import { authGuard } from '../app/authentication/auth.guard';
import { ViewManageChmmComponent } from '../app/view-manage-chmm/view-manage-chmm.component';
import { RegisterOrganisationComponent } from 'src/app/register-organisation/register-organisation.component';
import { ViewMcsDataComponent } from 'src/app/admin/mcs/view-mcs-data/view-mcs-data.component';
import { ManufacturersListComponent } from 'src/app/admin/manufacturers/manufacturers-list/manufacturers-list.component';
import { FeedbackComponent } from '../app/feedback/feedback.component';
import { AccountInactiveComponent } from 'src/app/account-inactive/account-inactive.component';
import { SelectSchemeYearComponent } from 'src/app/select-scheme-year/select-scheme-year.component';
import { organisationRoutes } from './organisation/organisation.routes';
import { schemeYearConfigurationRoutes } from './admin/scheme-year-configuration.routes';
import { SchemeConfigurationYearSelectorComponent } from '../app/admin/scheme-configuration/year-selector/scheme-configuration-year-selector.component';

export const applicationRoutes: Routes = [
  {
    path: '',
    component: ViewManageChmmComponent,
    pathMatch: 'full',
  },
  {
    path: 'feedback',
    component: FeedbackComponent,
  },
  {
    path: 'register-organisation',
    canActivate: [authGuard()],
    component: RegisterOrganisationComponent,
  },
  {
    path: 'account-inactive',
    canActivate: [authGuard()],
    component: AccountInactiveComponent,
  },
  {
    path: 'admin',
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'users',
      },
      {
        path: 'users',
        loadChildren: () =>
          import('./admin/admin-users.routes').then(
            (mod) => mod.adminUsersRoutes
          ),
      },
      {
        path: 'mcs-data',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        component: ViewMcsDataComponent,
      },
      {
        path: 'organisations',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        component: ManufacturersListComponent,
      },
      {
        path: 'scheme-year-configuration',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        component: SchemeConfigurationYearSelectorComponent,
      },
    ],
  },
  {
    path: 'scheme-year',
    canActivate: [
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
        component: SelectSchemeYearComponent,
      },
      {
        path: ':schemeYearId/organisation',
        loadChildren: () =>
          import('./organisation/scheme-year-organisation.routes').then(
            (mod) => mod.schemeYearOrganisationRoutes
          ),
      },
      {
        path: ':schemeYearId/configuration',
        canActivate: [
          authGuard([
            'Regulatory Officer',
            'Senior Technical Officer',
            'Principal Technical Officer',
          ]),
        ],
        loadChildren: () =>
          import('./admin/scheme-year-configuration.routes').then(
            (mod) => mod.schemeYearConfigurationRoutes
          ),
      },
    ],
  },
  {
    path: 'organisation',
    canActivate: [authGuard()],
    children: organisationRoutes,
  },
];
