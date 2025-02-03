import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgIf } from '@angular/common';
import { Observable } from 'rxjs';
import {
  selectChangeIsNonSchemeParticipantLoading,
  selectIsSchemePartipantFormValue,
} from 'src/app/stores/onboarding/selectors';
import { changeIsNonSchemeParticipant } from 'src/app/stores/onboarding/actions';
import { isSchemeParticipant } from '../change-is-non-scheme-participant-form/change-is-non-scheme-participant-form.component';
import { HttpState } from 'src/app/stores/http-state';

@Component({
  selector: 'change-is-non-scheme-participant-check-answers',
  templateUrl:
    './change-is-non-scheme-participant-check-answers.component.html',
  styleUrls: ['./change-is-non-scheme-participant-check-answers.component.css'],
  standalone: true,
  imports: [RouterLink, NgIf, AsyncPipe],
})
export class ChangeIsNonSchemeParticipantCheckAnswersComponent
  implements OnInit
{
  @Input({ required: true }) organisationId!: string;

  isSchemePartipant$: Observable<isSchemeParticipant | null>;
  loading$: Observable<boolean | null>;

  constructor(private store: Store) {
    this.isSchemePartipant$ = this.store.select(
      selectIsSchemePartipantFormValue
    );
    this.loading$ = this.store.select(
      selectChangeIsNonSchemeParticipantLoading
    );
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }
  }

  onChangeType() {
    this.store.dispatch(
      changeIsNonSchemeParticipant({ organisationId: this.organisationId })
    );
  }
}
