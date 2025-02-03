import { Component, DestroyRef, Input, OnInit, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { storeIsSchemePartipantFormValue } from 'src/app/stores/onboarding/actions';
import { selectIsSchemePartipantFormValue } from 'src/app/stores/onboarding/selectors';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

export type isSchemeParticipant = 'Yes' | 'No';

@Component({
  selector: 'change-is-non-scheme-participant-form',
  templateUrl: './change-is-non-scheme-participant-form.component.html',
  styleUrls: ['./change-is-non-scheme-participant-form.component.css'],
  standalone: true,
  imports: [RouterLink, FormsModule, NgIf],
})
export class ChangeIsNonSchemeParticipantFormComponent implements OnInit {
  private destroyRef = inject(DestroyRef);

  @Input({ required: true }) organisationId!: string;

  isSchemeParticipant: isSchemeParticipant | null = null;
  error: string | null = null;

  constructor(private store: Store) {
    this.store
      .select(selectIsSchemePartipantFormValue)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((isNonSchemePartipant) => {
        if (isNonSchemePartipant) {
          this.isSchemeParticipant = isNonSchemePartipant;
        }
      });
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }
  }

  onContinue() {
    this.error = null;
    if (!this.isSchemeParticipant) {
      this.error = 'Select if the manufacturer is a scheme participant';
    } else {
      this.store.dispatch(
        storeIsSchemePartipantFormValue({
          organisationId: this.organisationId,
          isSchemeParticipant: this.isSchemeParticipant,
        })
      );
    }
  }
}
