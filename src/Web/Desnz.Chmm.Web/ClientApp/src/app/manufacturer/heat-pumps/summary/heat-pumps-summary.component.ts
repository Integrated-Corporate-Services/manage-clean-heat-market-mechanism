import { AsyncPipe, DatePipe, DecimalPipe, JsonPipe, NgFor, NgIf } from "@angular/common";
import { Component, Input, OnInit } from "@angular/core";
import { Store } from "@ngrx/store";
import { BehaviorSubject, Observable } from "rxjs";
import { getHeatPumpInstallations } from "src/app/stores/heat-pumps/actions";
import { PeriodCreditTotalsDto } from "src/app/stores/heat-pumps/dtos/period-credit-totals.dto";
import { selectHeatPumpInstallations } from "src/app/stores/heat-pumps/selectors";
import { HttpState } from "src/app/stores/http-state";
import { SchemeYearDto } from "src/app/stores/organisation-summary/dtos/scheme-year.dto";
import { getSchemeYear } from "src/app/stores/scheme-year-configuration/actions";
import { SchemeYearConfigurationDto } from "src/app/stores/scheme-year-configuration/dtos/scheme-year-configuration.dto";
import { selectSchemeYearConfiguration } from "src/app/stores/scheme-year-configuration/selectors";
import { selectSelectedSchemeYear } from "src/app/stores/scheme-years/selectors";
import { SumHeatPumpsPipe } from "./sum-heat-pumps.pipe";
import { GovukPaginationComponent } from "src/app/shared/components/govuk/govuk-pagination/govuk-pagination.component";

@Component({
    selector: 'heat-pumps-summary',
    templateUrl: './heat-pumps-summary.component.html',
    styleUrls: ['heat-pumps-summary.component.css'],
    standalone: true,
    imports: [NgIf, NgFor, DatePipe, AsyncPipe, DecimalPipe, JsonPipe, SumHeatPumpsPipe, GovukPaginationComponent],
})
export class HeatPumpsSummaryComponent implements OnInit {

  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;

  paginatedDataSubject: BehaviorSubject<PeriodCreditTotalsDto[]>;
  paginatedData$: Observable<PeriodCreditTotalsDto[]>;
  
  heatPumpInstallations$: Observable<HttpState<PeriodCreditTotalsDto[]>>;
  selectedSchemeYear$: Observable<SchemeYearDto | null>;
  selectedSchemeYearConfiguration$: Observable<HttpState<SchemeYearConfigurationDto>>;

  constructor(private store: Store) {
    this.heatPumpInstallations$ = this.store.select(selectHeatPumpInstallations);
    this.selectedSchemeYear$ = this.store.select(selectSelectedSchemeYear);
    this.selectedSchemeYearConfiguration$ = this.store.select(selectSchemeYearConfiguration);
    this.paginatedDataSubject = new BehaviorSubject<PeriodCreditTotalsDto[]>([]);    
    this.paginatedData$ = this.paginatedDataSubject.asObservable();
  }
  
  ngOnInit() {
    this.store.dispatch(getHeatPumpInstallations({
      organisationId: this.organisationId,
      schemeYearId: this.schemeYearId
    }));
    this.store.dispatch(getSchemeYear({
      schemeYearId: this.schemeYearId
    }));
  }

  onPageChange(event: any[]) {
    this.paginatedDataSubject.next(event);
  }
}