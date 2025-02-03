import { AsyncPipe, JsonPipe, NgFor, NgIf, DatePipe } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { getAuditItems } from 'src/app/stores/history/actions';
import { AuditItemDto } from 'src/app/stores/history/dtos/AuditItemDto';
import { selectHistoryState } from 'src/app/stores/history/selectors';
import { HttpState } from 'src/app/stores/http-state';
import { AuditItemValueComponent } from '../audit-item-value/audit-item-value.component';
import { OnboardingAuditItemValueComponent } from '../onboarding-audit-item-value/onboarding-audit-item-value.component';

@Component({
  selector: 'organisation-history',
  templateUrl: './organisation-history.component.html',
  standalone: true,
  imports: [
    NgIf,
    AsyncPipe,
    NgFor,
    JsonPipe,
    AuditItemValueComponent,
    OnboardingAuditItemValueComponent,
    DatePipe
  ],
})
export class OrganisationHistoryComponent implements OnInit {
  @Input({ required: true }) organisationId!: string;

  historyState$: Observable<HttpState<AuditItemDto[]>>;

  constructor(private store: Store) {
    this.historyState$ = this.store.select(selectHistoryState);
  }

  ngOnInit() {
    this.validateInputs();
    this.store.dispatch(getAuditItems({ organisationId: this.organisationId }));
  }

  translateEventName(name: string): string {
    if (name === 'Adjust Credits') return 'Amend Credits';
    return name;
  }

  validateInputs() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }
  }
}
