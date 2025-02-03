import { Component, Input, OnInit } from '@angular/core';
import { NgIf, AsyncPipe, DecimalPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable, Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { CreditTransfer } from 'src/app/manufacturer/credits/models/credit-transfer';
import {
  selectCreditTransfer,
  selectIsLoading,
} from 'src/app/stores/credit/selectors';
import { selectIsAdmin } from 'src/app/stores/auth/selectors';
import { submitCreditTransfer } from 'src/app/stores/credit/actions';

@Component({
  selector: 'transfer-credits-check-answers',
  templateUrl: './transfer-credits-check-answers.component.html',
  standalone: true,
  imports: [NgIf, AsyncPipe, DecimalPipe],
})
export class TransferCreditsCheckAnswersComponent implements OnInit {
  @Input() organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;

  isAdmin!: boolean;

  creditTransfer$: Observable<CreditTransfer | null>;
  loading$: Observable<boolean>;
  isAdminSub: Subscription = Subscription.EMPTY;

  constructor(private store: Store, private router: Router) {
    this.creditTransfer$ = this.store.select(selectCreditTransfer);
    this.loading$ = this.store.select(selectIsLoading);
    this.isAdminSub = this.store.select(selectIsAdmin).subscribe((isAdmin) => {
      this.isAdmin = isAdmin;
    });
  }

  ngOnInit() {
    this.validateInputs();
  }

  onSubmit() {
    this.store.dispatch(
      submitCreditTransfer({
        organisationId: this.organisationId,
        schemeYearId: this.schemeYearId,
      })
    );
  }

  onChange() {
    if (this.isAdmin === null || this.isAdmin === undefined) {
      throw TypeError('isAdmin cannot be null or undefined');
    }

    this.router.navigate([
      `/scheme-year/${this.schemeYearId}/organisation/${this.organisationId}/transfer-credits`,
    ]);
  }

  validateInputs() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }
  }
}
