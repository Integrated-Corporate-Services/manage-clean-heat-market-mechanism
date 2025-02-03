import { Component, Input } from '@angular/core';
import { Store } from '@ngrx/store';
import { approveAccount, cancelAccountApproval } from 'src/app/stores/onboarding/actions';
import { RouterLink } from '@angular/router';
import { selectOrganisationDetailsLoading } from 'src/app/stores/onboarding/selectors';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'confirm-account-approval',
  templateUrl: './confirm-account-approval.component.html',
  standalone: true,
  imports: [RouterLink, AsyncPipe],
})
export class ConfirmAccountApprovalComponent {
  @Input() organisationId?: string;
  _edit = false;
  @Input() set edit(value: string) {
    this._edit = value === 'true' ? true : false;
  }
  get edit(): boolean {
    return this._edit;
  }

  loading$: Observable<boolean>;

  constructor(private store: Store) {
    this.loading$ = this.store.select(selectOrganisationDetailsLoading);
  }

  onApprove() {
    this.store.dispatch(approveAccount());
  }

  onCancel() {
    this.store.dispatch(cancelAccountApproval({ edit: this.edit }));
  }
}
