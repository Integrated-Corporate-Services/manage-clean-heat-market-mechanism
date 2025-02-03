import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { selectEditCreditAmendment } from 'src/app/stores/credit/selectors';
import { CreditAmendment } from '../models/amend-credit.model';
import { AsyncPipe, NgIf } from '@angular/common';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';
import { Organisation } from 'src/app/navigation/models/organisation';
import { navigationOrganisationSessionKey } from 'src/app/shared/constants';

@Component({
  selector: 'amend-credit-confirmation',
  templateUrl: './amend-credit-confirmation.component.html',
  standalone: true,
  imports: [NgIf, AsyncPipe],
})
export class AmendCreditConfirmationComponent {
  @Input() organisationId?: string;
  @Input({ required: true }) schemeYearId!: string;

  creditAmendment$: Observable<CreditAmendment | null>;
  organisationName: string | null;

  constructor(
    private store: Store,
    private sessionStorageService: SessionStorageService
  ) {
    this.creditAmendment$ = this.store.select(selectEditCreditAmendment);
    this.organisationName =
      this.sessionStorageService.getObject<Organisation>(
        navigationOrganisationSessionKey
      )?.name ?? null;
  }
}
