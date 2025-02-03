import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AsyncPipe, NgIf } from '@angular/common';
import { CreditTransfer } from 'src/app/manufacturer/credits/models/credit-transfer';
import { selectCreditTransfer } from 'src/app/stores/credit/selectors';

@Component({
  selector: 'transfer-credits-confirmation',
  templateUrl: './transfer-credits-confirmation.component.html',
  standalone: true,
  imports: [NgIf, AsyncPipe],
})
export class TransferCreditsConfirmationComponent implements OnInit {
  @Input() organisationId?: string;
  @Input({ required: true }) schemeYearId!: string;

  creditTransfer$: Observable<CreditTransfer | null>;

  constructor(private store: Store) {
    this.creditTransfer$ = this.store.select(selectCreditTransfer);
  }

  ngOnInit() {}
}
