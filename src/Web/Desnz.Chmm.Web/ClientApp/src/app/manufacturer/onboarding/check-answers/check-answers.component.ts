import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import {
  NgIf,
  NgFor,
  AsyncPipe,
  NgSwitch,
  NgSwitchCase,
  NgStyle,
} from '@angular/common';
import { Store } from '@ngrx/store';
import { FormControl } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { Router, RouterLink } from '@angular/router';
import { selectOrganisationDetails } from 'src/app/stores/onboarding/selectors';
import {
  clearIsSchemePartipantFormValue,
  editAccount,
  getOrganisationDetails,
  submitOnboardingDetails,
} from 'src/app/stores/onboarding/actions';
import { HttpState } from 'src/app/stores/http-state';
import { OrganisationDetails } from '../models/organisation-details';
import { selectWhoAmI } from '../../../stores/auth/selectors';
import { ViewAdministratorApprovalComponent } from 'src/app/admin/view-administrator-approval/view-administrator-approval.component';
import { ViewAdministratorRejectionComponent } from 'src/app/admin/view-administrator-rejection/view-administrator-rejection.component';
import { isAdmin } from 'src/app/shared/auth-utils';

export interface UserForm {
  name: FormControl<string>;
  email: FormControl<string>;
  roleId: FormControl<string>;
  status: FormControl<string>;
}

@Component({
  selector: 'check-answers',
  templateUrl: './check-answers.component.html',
  styleUrls: ['./check-answers.component.css'],
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    AsyncPipe,
    RouterLink,
    NgSwitch,
    NgSwitchCase,
    ViewAdministratorApprovalComponent,
    ViewAdministratorRejectionComponent,
    NgStyle,
  ],
})
export class CheckAnswersComponent implements OnInit, OnDestroy {
  @Input() organisationId!: string;
  @Input() mode: 'submit' | 'approve' | 'edit' | 'view' = 'submit';
  _fetchData = false;
  @Input() set fetchData(value: string) {
    this._fetchData = value === 'true' ? true : false;
  }
  get fetchData(): boolean {
    return this._fetchData;
  }
  @Input() redirectToSummary: boolean = false;
  onboarding$: Observable<HttpState<OrganisationDetails>>;

  allowChange: boolean = true;
  isAdmin: boolean | null = null;
  subscription: Subscription | null = null;

  constructor(private store: Store, private router: Router) {
    this.onboarding$ = this.store.select(selectOrganisationDetails);
    this.subscription = this.store.select(selectWhoAmI).subscribe((whoAmI) => {
      if (!whoAmI) return;
      this.allowChange = !whoAmI.roles.includes('Manufacturer');
      this.isAdmin = isAdmin(whoAmI);
    });
  }

  ngOnInit() {
    if (this.organisationId && this.fetchData) {
      this.store.dispatch(
        getOrganisationDetails({
          orgId: this.organisationId,
          redirectToSummary: this.redirectToSummary,
        })
      );
    }

    this.mode = this.mode ?? 'submit';
  }

  ngOnDestroy() {
    if (!this.subscription) return;
    this.subscription.unsubscribe();
  }

  onChangeIsSchemeParticipant() {
    this.store.dispatch(
      clearIsSchemePartipantFormValue({ organisationId: this.organisationId })
    );
  }

  onSave() {
    if (this.mode == 'submit') {
      this.store.dispatch(submitOnboardingDetails());
    } else {
      this.store.dispatch(editAccount({ area: '*' }));
    }
  }

  onApprove() {
    this.router.navigate([`/organisation/${this.organisationId}/approve`], {
      queryParams: { edit: true },
    });
  }

  onReject() {
    this.router.navigate([`/organisation/${this.organisationId}/reject`], {
      queryParams: { edit: true },
    });
  }
}
