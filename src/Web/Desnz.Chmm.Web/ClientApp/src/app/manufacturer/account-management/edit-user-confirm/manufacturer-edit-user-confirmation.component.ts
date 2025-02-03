import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Subscription, Observable } from 'rxjs';
import { selectManufacturerUser, selectManufacturerUserBeingEdited } from '../../../stores/account-management/selectors';
import { clearManufacturerUserBeingEdited, getManufacturerUser, updateManufacturerUser } from '../../../stores/account-management/actions';
import { AccountManagementState,  } from '../../../stores/account-management/state';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { FormsModule } from '@angular/forms';
import { EditManufacturerUserCommand } from '../../../stores/account-management/commands/edit-manufacturer-user-command';
import { ChmmUser } from '../../../stores/account-management/dtos/chmm-user';

@Component({
  selector: 'manufacturer-edit-user-confirmation',
  templateUrl: './manufacturer-edit-user-confirmation.component.html',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe
  ]
})
export class ManufacturerEditUserConfirmationComponent implements OnInit {

  @Input({ required: true }) organisationId?: string;
  @Input({ required: true }) userId?: string;

editManufacturerUser$: Observable<EditManufacturerUserCommand | null>;

  usersSubscription: Subscription | null = null;
  commandSubscription: Subscription | null = null;

  constructor(
    private store: Store,
    private router: Router,
    private backLinkProvider: BackLinkProvider) {
        this.editManufacturerUser$ = this.store.select(selectManufacturerUserBeingEdited);
  }

  ngOnInit() {
    this.backLinkProvider.clear();
  }

  cancel() {
    this.store.dispatch(clearManufacturerUserBeingEdited());
    this.router.navigateByUrl(`/organisation/${this.organisationId}/users`);
  }
}
