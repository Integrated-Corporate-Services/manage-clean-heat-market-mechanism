import { importProvidersFrom, isDevMode } from '@angular/core';
import { AppComponent } from './app/app.component';
import {
  provideRouter,
  withComponentInputBinding,
  withInMemoryScrolling,
} from '@angular/router';
import { bootstrapApplication } from '@angular/platform-browser';
import {
  provideHttpClient,
  HttpClientXsrfModule,
  withInterceptors,
} from '@angular/common/http';
import { provideStore } from '@ngrx/store';
import { reducers } from './app/stores';
import { provideEffects } from '@ngrx/effects';
import { UserEffects } from './app/stores/account-management/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { UserService } from './app/stores/account-management/services/user-service';
import { RoleService } from './app/stores/account-management/services/role.service';
import { NotesService } from './app/stores/account-management/services/notes.service';
import { applicationRoutes } from './routes/routes';
import { OnboardingEffects } from './app/stores/onboarding/effects';
import { BackLinkProvider } from './app/navigation/back-link/back-link.provider';
import { OrganisationService } from './app/stores/onboarding/services.ts/organisation-service';
import { BoilerSalesEffects } from './app/stores/boiler-sales/effects';
import { BoilerSalesService } from './app/stores/boiler-sales/services/boiler-sales-service';
import { SessionStorageService } from './app/shared/services/session-storage.service';
import { NavigationEffects } from './app/stores/navigation/effects';
import { CreditEffects } from './app/stores/credit/effects';
import { ObligationEffects } from './app/stores/amend-obligation/effects';
import { CreditService } from './app/stores/credit/services/credit.service';
import { ObligationService } from './app/stores/amend-obligation/services/obligation.service';
import { McsService } from './app/stores/mcs/services/mcs.service';
import { McsEffects } from './app/stores/mcs/effects';
import { LicenceHolderEffects } from './app/stores/licence-holders/effects';
import { LicenceHolderService } from './app/stores/licence-holders/services/licence-holder.service';
import { AvailableForTransferEffects } from './app/stores/organisations/effects';
import { ContactsEffects } from './app/stores/contacts/effects';
import { dateTimeInterceptor } from './app/http-interceptors/datetime-interceptor';
import { HistoryEffects } from './app/stores/history/effects';
import { SystemAuditService } from './app/stores/history/services/SystemAuditService';
import { authInterceptor } from './app/http-interceptors/auth-interceptor';
import { AdministratorApprovalEffects } from './app/stores/administrator-approval/effects';
import { OrganisationSummaryEffects } from './app/stores/organisation-summary/effects';
import { SchemeYearService } from './app/stores/organisation-summary/services/scheme-year.service';
import { SchemeYearsEffects } from './app/stores/scheme-years/effects';
import { SchemeYearConfigurationEffects } from './app/stores/scheme-year-configuration/effects';
import { SchemeYearConfigurationService } from './app/stores/scheme-year-configuration/services/scheme-year-configuration.service';
import { AdministratorRejectionEffects } from './app/stores/administrator-rejection/effects';
import { HeatPumpInstallationsEffects } from './app/stores/heat-pumps/effects';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

bootstrapApplication(AppComponent, {
  providers: [
    UserService,
    RoleService,
    OrganisationService,
    BoilerSalesService,
    NotesService,
    BackLinkProvider,
    SessionStorageService,
    CreditService,
    ObligationService,
    McsService,
    LicenceHolderService,
    SystemAuditService,
    SchemeYearService,
    SchemeYearConfigurationService,
    importProvidersFrom(
      HttpClientXsrfModule.withOptions({
        cookieName: 'XSRF-TOKEN',
        headerName: 'X-XSRF-TOKEN',
      })
    ),
    provideHttpClient(withInterceptors([authInterceptor, dateTimeInterceptor])),
    provideRouter(
      applicationRoutes,
      withComponentInputBinding(),
      withInMemoryScrolling({ anchorScrolling: 'enabled' })
    ),
    provideStore(reducers),
    provideEffects([
      UserEffects,
      OnboardingEffects,
      BoilerSalesEffects,
      NavigationEffects,
      CreditEffects,
      ObligationEffects,
      McsEffects,
      LicenceHolderEffects,
      AvailableForTransferEffects,
      ContactsEffects,
      HistoryEffects,
      AdministratorApprovalEffects,
      AdministratorRejectionEffects,
      OrganisationSummaryEffects,
      SchemeYearsEffects,
      SchemeYearConfigurationEffects,
      HeatPumpInstallationsEffects,
    ]),
    provideStoreDevtools({ maxAge: 25, logOnly: !isDevMode() }),
  ],
}).catch((err) => console.log(err));
