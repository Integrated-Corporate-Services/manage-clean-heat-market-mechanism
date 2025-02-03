import { Component, Input, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs';
import { selectManufacturerUserBeingInvited, } from '../../../stores/account-management/selectors';
import { inviteManufacturerUser } from '../../../stores/account-management/actions';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { InviteManufacturerUserCommand } from '../../../stores/account-management/commands/invite-manufacturer-user-command';

@Component({
  selector: 'manufacturer-confirm-invite-user',
  templateUrl: './manufacturer-confirm-invite-user.component.html',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe    
  ],
})
export class ManufacturerConfirmInviteUserComponent implements OnInit {

  @Input({ required: true }) organisationId?: string;

  command: InviteManufacturerUserCommand = {
    email: '',
    jobTitle: '',
    name: '',
    organisationId: '',
    telephoneNumber: ''
  };

  subscription: Subscription | null = null;

  constructor(private store: Store) {
  }

  ngOnInit() {
    this.command.organisationId = this.organisationId!;
    this.subscription = this.store.select(selectManufacturerUserBeingInvited).subscribe(existingCommand => {
      if (!existingCommand) return;
      this.command = { ...existingCommand };
    });
  }

  ngOnDestroy() {
    if (!this.subscription) return;
    this.subscription.unsubscribe();
  }

  continue() {
    this.store.dispatch(inviteManufacturerUser());
  }
}
