import * as AuthActions from './auth/actions';
import * as UserActions from './account-management/actions';
import * as BoilerSalesActions from './boiler-sales/actions';
import * as NavigationActions from './navigation/actions';
import * as CreditActions from './credit/actions';
import * as ObligationActions from './amend-obligation/actions';
import * as McsActions from './mcs/actions';
import * as LicenceHolderActions from './licence-holders/actions';
import * as OrganisationsActions from './organisations/actions';
import * as ContactsActions from './contacts/actions';
import * as HistoryActions from './history/actions';
import * as OrganisationSummaryActions from './organisation-summary/actions';
import * as SchemeYearActions from './scheme-years/actions';
import * as SchemeYearConfigurationActions from './scheme-year-configuration/actions';
import * as HeatPumpInstallationsActions from './heat-pumps/actions';

import { ActionReducerMap } from '@ngrx/store';
import { AuthState, authReducer } from './auth/reducer';
import { userReducer } from './account-management/reducer';
import { AccountManagementState } from './account-management/state';
import { OrganisationState } from './onboarding/state';
import { onboardingReducer } from './onboarding/reducer';
import { BoilerSalesState } from './boiler-sales/state';
import { boilerSalesReducer } from './boiler-sales/reducer';
import { NavigationState, navigationReducer } from './navigation/reducer';
import { CreditState } from './credit/state';
import { creditReducer } from './credit/reducer';
import { AmendObligationState } from './amend-obligation/state';
import { amendObligationReducer } from './amend-obligation/reducer';
import { McsState } from './mcs/state';
import { mcsReducer } from './mcs/reducer';
import { LicenceHolderState } from './licence-holders/state';
import { licenceHolderReducer } from './licence-holders/reducer';
import { HttpState } from './http-state';
import { Organisation } from '../navigation/models/organisation';
import { availableForTransferReducer } from './organisations/reducer';
import { contactsReducer } from './contacts/reducer';
import { AuditItemDto } from './history/dtos/AuditItemDto';
import { historyReducer } from './history/reducer';
import { OrganisationApprovalCommentsDto } from './administrator-approval/dtos/organisation-approval-comments.dto';
import { administratorApprovalReducer } from './administrator-approval/reducer';
import { OrganisationSummaryState } from './organisation-summary/state';
import { organisationSummaryReducer } from './organisation-summary/reducer';
import { schemeYearsReducer } from './scheme-years/reducer';
import { SchemeYearsState } from './scheme-years/state';
import { ContactOrganisationDto } from './onboarding/dtos/contact-organisation-dto';
import { schemeYearConfigurationReducer } from './scheme-year-configuration/reducer';
import { SchemeYearConfigurationState } from './scheme-year-configuration/state';
import { OrganisationRejectionCommentsDto } from './administrator-rejection/dtos/organisation-rejection-comments.dto';
import { administratorRejectionReducer } from './administrator-rejection/reducer';
import { heatPumpInstallationsReducer } from './heat-pumps/reducer';
import { PeriodCreditTotalsDto } from './heat-pumps/dtos/period-credit-totals.dto';

export interface AppState {
  auth: AuthState;
  accountManagement: AccountManagementState;
  onboarding: OrganisationState;
  boilerSales: BoilerSalesState;
  navigationState: NavigationState;
  creditState: CreditState;
  obligationState: AmendObligationState;
  mcsState: McsState;
  licenceHolderState: LicenceHolderState;
  availableForTransfer: HttpState<Organisation[]>;
  contactsState: HttpState<ContactOrganisationDto[]>;
  historyState: HttpState<AuditItemDto[]>;
  administratorApprovalState: HttpState<OrganisationApprovalCommentsDto>;
  administratorRejectionState: HttpState<OrganisationRejectionCommentsDto>;
  organisationSummaryState: OrganisationSummaryState;
  schemeYearsState: SchemeYearsState;
  schemeYearConfigurationState: SchemeYearConfigurationState;
  heatPumpInstallations: HttpState<PeriodCreditTotalsDto[]>;
}

export const reducers: ActionReducerMap<AppState> = {
  auth: authReducer,
  accountManagement: userReducer,
  onboarding: onboardingReducer,
  boilerSales: boilerSalesReducer,
  navigationState: navigationReducer,
  creditState: creditReducer,
  obligationState: amendObligationReducer,
  mcsState: mcsReducer,
  licenceHolderState: licenceHolderReducer,
  availableForTransfer: availableForTransferReducer,
  contactsState: contactsReducer,
  historyState: historyReducer,
  administratorApprovalState: administratorApprovalReducer,
  administratorRejectionState: administratorRejectionReducer,
  organisationSummaryState: organisationSummaryReducer,
  schemeYearsState: schemeYearsReducer,
  schemeYearConfigurationState: schemeYearConfigurationReducer,
  heatPumpInstallations: heatPumpInstallationsReducer
};

export {
  AuthActions,
  UserActions,
  BoilerSalesActions,
  NavigationActions,
  CreditActions,
  ObligationActions,
  McsActions,
  LicenceHolderActions,
  OrganisationsActions,
  HistoryActions,
  OrganisationSummaryActions,
  SchemeYearActions,
  ContactsActions,
  SchemeYearConfigurationActions,
  HeatPumpInstallationsActions,
};
