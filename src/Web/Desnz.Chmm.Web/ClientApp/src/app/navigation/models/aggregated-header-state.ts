import { SchemeYearDto } from 'src/app/stores/organisation-summary/dtos/scheme-year.dto';
import { Organisation } from './organisation';

export interface AggregatedHeaderState {
  isAuthenticated: boolean;
  currentUserOrgId?: string | null;
  links: {
    showAdmin: boolean;
    showManufacturer: boolean;
    showAdminSecondaryNav: boolean;
  };
  organisation: Organisation | null;
  schemeYear: SchemeYearDto | null;
  hideSchemeYearSelector: boolean;
  hideServiceNav: boolean;
}
