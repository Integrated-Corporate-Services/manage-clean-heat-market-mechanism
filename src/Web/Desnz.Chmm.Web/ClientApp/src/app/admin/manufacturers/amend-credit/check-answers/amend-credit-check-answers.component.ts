import { Component, Input } from '@angular/core';
import { NgIf, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { RouterLink } from '@angular/router';
import { EditCreditState } from 'src/app/stores/credit/state';
import { selectEditCreditState } from 'src/app/stores/credit/selectors';
import { submitCreditAmendment } from 'src/app/stores/credit/actions';

@Component({
  selector: 'amend-credit-check-answers',
  templateUrl: './amend-credit-check-answers.component.html',
  standalone: true,
  imports: [NgIf, AsyncPipe, RouterLink],
})
export class AmendCreditCheckAnswersComponent {
  @Input() organisationId?: string;
  @Input({ required: true }) schemeYearId!: string;

  editCredit$: Observable<EditCreditState>;

  constructor(private store: Store) {
    this.editCredit$ = this.store.select(selectEditCreditState);
  }

  onSubmit() {
    this.store.dispatch(
      submitCreditAmendment({ schemeYearId: this.schemeYearId })
    );
  }
}
