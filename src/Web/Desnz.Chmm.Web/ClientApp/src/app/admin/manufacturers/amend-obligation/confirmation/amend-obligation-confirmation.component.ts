import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { selectEditObligationAmendment } from 'src/app/stores/amend-obligation/selectors';
import { ObligationAmendment } from '../models/amend-obligation.model';
import { AsyncPipe, NgIf } from '@angular/common';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';
import { Organisation } from 'src/app/navigation/models/organisation';
import { navigationOrganisationSessionKey } from 'src/app/shared/constants';

@Component({
  selector: 'amend-obligation-confirmation',
  templateUrl: './amend-obligation-confirmation.component.html',
  standalone: true,
  imports: [NgIf, AsyncPipe],
})
export class AmendObligationConfirmationComponent implements OnInit {
  @Input() organisationId?: string;
  @Input({ required: true }) schemeYearId!: string;

  obligationAmendment$: Observable<ObligationAmendment | null>;
  organisationName: string | null;

  constructor(
    private store: Store,
    private sessionStorageService: SessionStorageService
  ) {
    this.obligationAmendment$ = this.store.select(
      selectEditObligationAmendment
    );
    this.organisationName =
      this.sessionStorageService.getObject<Organisation>(
        navigationOrganisationSessionKey
      )?.name ?? null;
  }

  ngOnInit() {}
}
