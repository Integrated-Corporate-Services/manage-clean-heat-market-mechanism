import { Route } from '@angular/router';
import { AddressFormComponent } from 'src/app/manufacturer/onboarding/address-form/address-form.component';
import { CheckAnswersComponent } from 'src/app/manufacturer/onboarding/check-answers/check-answers.component';
import { ConfirmationComponent } from 'src/app/manufacturer/onboarding/confirmation/confirmation.component';
import { ContactDetailsForCtFormComponent } from 'src/app/manufacturer/onboarding/credit-transfers-opt-in-form/contact-details-for-ct-form.component';
import { HeatPumpBrandsFormComponent } from 'src/app/manufacturer/onboarding/heat-pump-brands-form/heat-pump-brands-form.component';
import { LegalCorrespondenceFormComponent } from 'src/app/manufacturer/onboarding/legal-correspondence-form/legal-correspondence-form.component';
import { OrganisationStructureFormComponent } from 'src/app/manufacturer/onboarding/organisation-structure-form/organisation-structure-form.component';
import { ResponsibleOfficerFormComponent } from 'src/app/manufacturer/onboarding/responsible-officer-form/responsible-officer-form.component';
import { ResponsibleUndertakingFormComponent } from 'src/app/manufacturer/onboarding/responsible-undertaking-form/responsible-undertaking-form.component';
import { SellFossilFuelBoilersFormComponent } from 'src/app/manufacturer/onboarding/sell-fossil-fuel-boilers-form/sell-fossil-fuel-boilers-form.component';
import { UserDetailsFormComponent } from 'src/app/manufacturer/onboarding/user-details-form/user-details-form.component';

export const registrationRoutes: Route[] = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'organisation-structure',
  },
  {
    path: 'organisation-structure',
    component: OrganisationStructureFormComponent,
  },
  {
    path: 'responsible-undertaking',
    component: ResponsibleUndertakingFormComponent,
  },
  {
    path: 'registered-office-address',
    component: AddressFormComponent,
  },
  {
    path: 'legal-correspondence-address',
    component: LegalCorrespondenceFormComponent,
  },
  {
    path: 'fossil-fuel-boilers',
    component: SellFossilFuelBoilersFormComponent,
  },
  {
    path: 'heat-pump-brands',
    component: HeatPumpBrandsFormComponent,
  },
  {
    path: 'user-details',
    component: UserDetailsFormComponent,
  },
  {
    path: 'responsible-officer-form',
    component: ResponsibleOfficerFormComponent,
  },
  {
    path: 'contact-details',
    component: ContactDetailsForCtFormComponent,
  },
  {
    path: 'check-answers',
    component: CheckAnswersComponent,
  },
  {
    path: 'confirmation',
    component: ConfirmationComponent,
  },
];
