import { Route } from "@angular/router";
import { authGuard } from "src/app/authentication/auth.guard";
import { HeatPumpsSummaryComponent } from "src/app/manufacturer/heat-pumps/summary/heat-pumps-summary.component";

export const heatPumpsRoutes: Route[] = [
  {
    path: '',
    canActivate: [
      authGuard([
        'Regulatory Officer',
        'Senior Technical Officer',
        'Principal Technical Officer',
        'Manufacturer',
      ]),
    ],
    component: HeatPumpsSummaryComponent
  }
];