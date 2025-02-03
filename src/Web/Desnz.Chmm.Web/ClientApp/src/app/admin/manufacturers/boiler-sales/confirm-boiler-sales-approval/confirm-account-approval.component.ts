import { Component, Input } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { AsyncPipe, NgIf } from '@angular/common';
import { approveAnnualBoilerSales } from 'src/app/stores/boiler-sales/actions';
import { selectApproveAnnualState } from 'src/app/stores/boiler-sales/selectors';
import { HttpState } from 'src/app/stores/http-state';

@Component({
  selector: 'confirm-boiler-sales-approval',
  templateUrl: './confirm-boiler-sales-approval.component.html',
  standalone: true,
  imports: [RouterLink, AsyncPipe, NgIf],
})
export class ConfirmBoilerSalesApprovalComponent {
  @Input({ required: true }) organisationId?: string;
  @Input({ required: true }) schemeYearId?: string;

  approveAnnualState$: Observable<HttpState>;

  constructor(private store: Store) {
    this.approveAnnualState$ = this.store.select(selectApproveAnnualState);
  }

  onApprove() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    if (this.schemeYearId === null || this.schemeYearId === undefined) {
      throw TypeError('schemeYearId cannot be null or undefined');
    }

    this.store.dispatch(
      approveAnnualBoilerSales({
        organisationId: this.organisationId,
        schemeYearId: this.schemeYearId,
      })
    );
  }
}
