import { Component, Input } from '@angular/core';
import { NgIf, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { RouterLink } from '@angular/router';
import { EditObligationState } from 'src/app/stores/amend-obligation/state';
import { selectEditObligationState } from 'src/app/stores/amend-obligation/selectors';
import { submitObligationAmendment } from 'src/app/stores/amend-obligation/actions';

@Component({
  selector: 'amend-obligation-check-answers',
  templateUrl: './amend-obligation-check-answers.component.html',
  standalone: true,
  imports: [NgIf, AsyncPipe, RouterLink],
})
export class AmendObligationCheckAnswersComponent {
  @Input() organisationId?: string;
  @Input({ required: true }) schemeYearId!: string;

  editObligation$: Observable<EditObligationState>;

  constructor(private store: Store) {
    this.editObligation$ = this.store.select(selectEditObligationState);
  }

  onSubmit() {
    this.store.dispatch(
      submitObligationAmendment({ schemeYearId: this.schemeYearId })
    );
  }
}
