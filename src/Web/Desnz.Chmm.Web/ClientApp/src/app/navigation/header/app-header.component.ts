import { Component } from '@angular/core';
import { OneloginNavbarComponent } from '../onelogin-navbar/onelogin-navbar.component';
import { ServiceNavbarComponent } from '../service-navbar/service-navbar.component';
import { AsyncPipe, JsonPipe, NgIf } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable, combineLatest, map } from 'rxjs';
import { selectWhoAmI } from '../../stores/auth/selectors';
import { AdminSecondaryNavbarComponent } from '../admin-secondary-navbar/admin-secondary-navbar.component';
import { selectOrganisationDetailsData } from 'src/app/stores/onboarding/selectors';
import { AggregatedHeaderState } from '../models/aggregated-header-state';
import { Organisation } from '../models/organisation';
import { WhoAmI } from 'src/app/stores/auth/services/authentication.service';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';
import { selectNavigationState } from 'src/app/stores/navigation/selectors';
import {
  navigationOrganisationSessionKey,
  schemeYearSessionKey,
} from 'src/app/shared/constants';
import { SchemeYearDto } from 'src/app/stores/organisation-summary/dtos/scheme-year.dto';
import { selectSelectedSchemeYear } from 'src/app/stores/scheme-years/selectors';
import { SchemeYearNavbarComponent } from '../scheme-year-navbar/scheme-year-navbar.component';
import { setSelectedSchemeYear } from 'src/app/stores/scheme-years/actions';
import { isAdmin } from 'src/app/shared/auth-utils';

@Component({
  selector: 'app-header',
  templateUrl: './app-header.component.html',
  styleUrls: ['./app-header.component.css'],
  standalone: true,
  imports: [
    OneloginNavbarComponent,
    ServiceNavbarComponent,
    AsyncPipe,
    NgIf,
    AdminSecondaryNavbarComponent,
    SchemeYearNavbarComponent,
    JsonPipe,
  ],
})
export class AppHeaderComponent {
  headerState$: Observable<AggregatedHeaderState>;

  serviceName = 'Clean Heat Market Mechanism';
  isAdmin: boolean | null = null;

  constructor(
    private store: Store,
    private sessionStorageService: SessionStorageService
  ) {
    this.headerState$ = combineLatest([
      this.store.select(selectWhoAmI),
      this.store.select(selectOrganisationDetailsData),
      this.store.select(selectSelectedSchemeYear),
      this.store.select(selectNavigationState),
    ]).pipe(
      map(([whoAmI, organisationDetails, selectedSchemeYear, navigation]) => {
        let schemeYear =
          this.sessionStorageService.getObject<SchemeYearDto>(
            schemeYearSessionKey
          );
        if (schemeYear !== null && !selectedSchemeYear) {
          this.store.dispatch(setSelectedSchemeYear({ schemeYear }));
        }

        let organisation = this.sessionStorageService.getObject<Organisation>(
          navigationOrganisationSessionKey
        ) ?? {
          id: null,
          name: null,
          status: null,
        };

        let showAdminSecondaryNav = organisation?.status === 'Active';

        if (organisationDetails?.id) {
          organisation.id = organisationDetails.id;
        }
        if (organisationDetails?.responsibleUndertaking?.name) {
          organisation.name = organisationDetails.responsibleUndertaking?.name;
        }
        if (organisationDetails?.status) {
          showAdminSecondaryNav = organisationDetails.status === 'Active';
        }

        this.isAdmin = isAdmin(whoAmI);

        return {
          isAuthenticated: whoAmI !== null,
          currentUserOrgId: whoAmI?.organisationId,
          links: {
            showAdmin: this.showAdminServiceNavbarLinks(whoAmI),
            showAdminSecondaryNav:
              this.showAdminServiceNavbarLinks(whoAmI) &&
              showAdminSecondaryNav &&
              !navigation.hideSecondaryNav,
            showManufacturer: this.showManufacturerServiceNavbarLinks(whoAmI),
          },
          organisation: organisation,
          schemeYear: selectedSchemeYear ?? schemeYear,
          hideSchemeYearSelector: navigation.hideSchemeYearSelector,
          hideServiceNav: navigation.hideServiceNav && !isAdmin(whoAmI),
        };
      })
    );
  }

  private showAdminServiceNavbarLinks(whoAmI: WhoAmI | null): boolean {
    return (
      whoAmI?.roles.some((role) =>
        [
          'Regulatory Officer',
          'Senior Technical Officer',
          'Principal Technical Officer',
        ].includes(role)
      ) ?? false
    );
  }

  private showManufacturerServiceNavbarLinks(whoAmI: WhoAmI | null): boolean {
    return whoAmI != null
      ? whoAmI?.roles.includes('Manufacturer') &&
          whoAmI?.status === 'Active' &&
          whoAmI?.organisationId !== null
      : false;
  }
}
