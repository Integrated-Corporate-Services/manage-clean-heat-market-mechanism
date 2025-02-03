import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgIf } from '@angular/common';
import { Observable } from 'rxjs';
import { selectIsSchemePartipantFormValue } from 'src/app/stores/onboarding/selectors';
import { isSchemeParticipant } from '../change-is-non-scheme-participant-form/change-is-non-scheme-participant-form.component';
import { Organisation } from 'src/app/navigation/models/organisation';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';
import { navigationOrganisationSessionKey } from 'src/app/shared/constants';

@Component({
  selector: 'change-is-non-scheme-participant-confirmation',
  templateUrl: './change-is-non-scheme-participant-confirmation.component.html',
  styleUrls: ['./change-is-non-scheme-participant-confirmation.component.css'],
  standalone: true,
  imports: [RouterLink, NgIf, AsyncPipe],
})
export class ChangeIsNonSchemeParticipantConfirmationComponent
  implements OnInit
{
  @Input({ required: true }) organisationId!: string;

  organisation: Organisation | null;
  isSchemePartipant$: Observable<isSchemeParticipant | null>;

  constructor(
    private store: Store,
    private sessionStorageService: SessionStorageService
  ) {
    this.isSchemePartipant$ = this.store.select(
      selectIsSchemePartipantFormValue
    );
    this.organisation = this.sessionStorageService.getObject<Organisation>(
      navigationOrganisationSessionKey
    );
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }
  }
}
