import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Observable, Subscription } from 'rxjs';
import { selectManufacturerUserBeingInvited, selectManufacturerUserBeingInvitedError } from '../../../stores/account-management/selectors';
import {
  clearManufacturerUserBeingInvited,
  storeManufacturerUserBeingInvited,
} from '../../../stores/account-management/actions';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { InviteManufacturerUserCommand } from '../../../stores/account-management/commands/invite-manufacturer-user-command';
import { FormsModule } from '@angular/forms';
import { ManufacturerInviteUserValidator } from './manufacturer-invite-user.validator';

@Component({
  selector: 'manufacturer-invite-user',
  templateUrl: './manufacturer-invite-user.component.html',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe
  ],
  providers: [ManufacturerInviteUserValidator]
})
export class ManufacturerInviteUserComponent implements OnInit, OnDestroy {

  @Input({ required: true }) organisationId?: string;

  serverError$: Observable<string | null>;

  errors: any = {};
  command: InviteManufacturerUserCommand = {
    email: '',
    jobTitle: '',
    name: '',
    organisationId: '',
    telephoneNumber: ''
  };

  commandSubscription: Subscription | null = null;
  errorSubscription: Subscription | null = null;

  constructor(
    private store: Store,
    private router: Router,
    private validator: ManufacturerInviteUserValidator,
    backLinkProvider: BackLinkProvider) {
    backLinkProvider.clear();
    this.serverError$ = this.store.select(selectManufacturerUserBeingInvitedError);
  }

  ngOnInit() {
    this.command.organisationId = this.organisationId!;
    this.commandSubscription = this.store.select(selectManufacturerUserBeingInvited).subscribe(existingCommand => {
      if (!existingCommand) return;
      this.command = {
        ...existingCommand,
        organisationId: this.organisationId!
      };
    });
  }

  ngOnDestroy() {
    if (this.commandSubscription) this.commandSubscription.unsubscribe();
    if (this.errorSubscription) this.errorSubscription.unsubscribe();
  }

  continue() {
    let validationResponse = this.validator.validate(this.command);
    if (!validationResponse.valid) {
      this.errors = validationResponse.errors;
      return;
    }

    this.store.dispatch(storeManufacturerUserBeingInvited({
      command: this.command
    }));
    this.router.navigateByUrl(`/organisation/${this.organisationId}/users/confirm-invite`);
  }

  cancel() {
    this.store.dispatch(clearManufacturerUserBeingInvited());
    this.router.navigateByUrl(`/organisation/${this.organisationId}/users`);
  }
}
