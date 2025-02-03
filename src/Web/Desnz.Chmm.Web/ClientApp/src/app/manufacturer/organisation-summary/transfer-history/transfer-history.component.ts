import { AsyncPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { Component, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { selectCreditTransferHistory } from '../../../stores/credit/selectors';
import { Observable } from 'rxjs';
import { TransferHistoryDto } from '../../../stores/credit/dtos/transfer-history.dto';
import { getCreditTransferHistory } from '../../../stores/credit/actions';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';

@Component({
  selector: 'transfer-history',
  templateUrl: './transfer-history.component.html',
  styleUrls: ['./transfer-history.component.scss'],
  standalone: true,
  imports: [NgFor, NgIf, RouterLink, AsyncPipe, DatePipe],
})
export class TransferHistoryComponent implements OnInit {

  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;

  transferHistory$: Observable<TransferHistoryDto | null>;

  constructor(private store: Store, private backLinkProvider: BackLinkProvider) {
    this.transferHistory$ = this.store.select(selectCreditTransferHistory);
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }
    if (this.schemeYearId === null || this.schemeYearId === undefined) {
      throw TypeError('schemeYearId cannot be null or undefined');
    }

    this.backLinkProvider.link = `scheme-year/${this.schemeYearId}/organisation/${this.organisationId}/summary`;

    this.store.dispatch(
      getCreditTransferHistory({
        organisationId: this.organisationId,
        schemeYearId: this.schemeYearId
      })
    );
  }
}
