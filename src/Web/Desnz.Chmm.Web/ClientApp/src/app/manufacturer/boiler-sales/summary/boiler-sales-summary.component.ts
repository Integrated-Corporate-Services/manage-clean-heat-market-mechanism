import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe, DecimalPipe, NgStyle } from '@angular/common';
import { Store, select } from '@ngrx/store';
import { Observable, Subscription } from 'rxjs';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { HttpState } from 'src/app/stores/http-state';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { selectBoilerSalesSummary } from '../../../stores/boiler-sales/selectors';
import { BoilerSalesSummary } from '../../../stores/boiler-sales/dtos/boiler-sales-summary';
import {
  clearBoilerSalesSummary,
  getAnnualBoilerSales,
  getQuarterlyBoilerSales,
} from '../../../stores/boiler-sales/actions';
import { BoilerSalesQuarterStatusComponent } from './boiler-sales-quarter-status.component';
import { selectIsAdmin } from 'src/app/stores/auth/selectors';
import { SessionStorageService } from '../../../shared/services/session-storage.service';
import { selectSelectedSchemeYear } from '../../../stores/scheme-years/selectors';

@Component({
  selector: 'boiler-sales-summary',
  templateUrl: './boiler-sales-summary.component.html',
  styleUrls: ['boiler-sales-summary.component.css'],
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe,
    DecimalPipe,
    BoilerSalesQuarterStatusComponent,
    NgStyle,
  ],
})
export class BoilerSalesSummaryComponent implements OnInit, OnDestroy {
  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;

  boilerSales$: Observable<HttpState<BoilerSalesSummary>>;
  isAdmin$: Observable<boolean>;

  allowEdit: boolean = false;
  allowEditQuarterly: boolean = false;

  subscription: Subscription = Subscription.EMPTY;

  constructor(private store: Store, backLinkProvider: BackLinkProvider, private sessionStorageService: SessionStorageService) {
    backLinkProvider.clear();
    this.boilerSales$ = this.store.select(selectBoilerSalesSummary);
    this.isAdmin$ = this.store.select(selectIsAdmin);
  }

  ngOnInit() {
    this.validateInputs();

    this.store.dispatch(clearBoilerSalesSummary());

    this.store.dispatch(
      getAnnualBoilerSales({
        organisationId: this.organisationId,
        schemeYearId: this.schemeYearId,
      })
    );

    this.store.dispatch(
      getQuarterlyBoilerSales({
        organisationId: this.organisationId,
        schemeYearId: this.schemeYearId,
      })
    );

    const dateTimeOverride = this.sessionStorageService.getObject<string>('dateTimeOverride');
    this.subscription = this.store.select(selectSelectedSchemeYear).subscribe(selectedSchemeYear => {
      if (!selectedSchemeYear) return;

      let currentDate = dateTimeOverride ? Date.parse(dateTimeOverride) : Date.now();
      let endDate = Date.parse(selectedSchemeYear.endDate);
      let surrenderDayDate = Date.parse(selectedSchemeYear.surrenderDayDate);

      this.allowEdit = !(currentDate < endDate || currentDate >= surrenderDayDate);
      this.allowEditQuarterly = !(currentDate >= surrenderDayDate);
    });
  }

  ngOnDestroy() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  validateInputs() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    if (this.schemeYearId === null || this.schemeYearId === undefined) {
      throw TypeError('schemeYearId cannot be null or undefined');
    }
  }
}
