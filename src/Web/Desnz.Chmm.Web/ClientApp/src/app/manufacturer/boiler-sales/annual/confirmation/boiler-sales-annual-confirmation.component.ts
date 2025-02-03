import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { SubmitAnnualState } from '../../../../stores/boiler-sales/state';
import { Store } from '@ngrx/store';
import { selectSubmitAnnual } from '../../../../stores/boiler-sales/selectors';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { navigationOrganisationSessionKey } from '../../../../shared/constants';
import { Organisation } from '../../../../navigation/models/organisation';
import { SessionStorageService } from '../../../../shared/services/session-storage.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'boiler-sales-annual-confirmation',
  templateUrl: './boiler-sales-annual-confirmation.component.html',
  standalone: true,
  imports: [NgIf, NgFor, AsyncPipe, RouterLink],
})
export class BoilerSalesAnnualConfirmationComponent implements OnInit {
  @Input() organisationId?: string;
  @Input() schemeYearId?: string;

  year: number = 2024;

  boilerSales$: Observable<SubmitAnnualState>;
  organisation: Organisation | null;

  constructor(
    private store: Store,
    sessionStorageService: SessionStorageService
  ) {
    this.boilerSales$ = this.store.select(selectSubmitAnnual);
    this.organisation = sessionStorageService.getObject<Organisation>(
      navigationOrganisationSessionKey
    );
  }

  ngOnInit() {}
}
