import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { selectManufacturerUserBeingInvited } from 'src/app/stores/account-management/selectors';
import { Subscription } from 'rxjs';
import { RouterLink } from '@angular/router';
import { InviteManufacturerUserCommand } from '../../../stores/account-management/commands/invite-manufacturer-user-command';
import { clearManufacturerUserBeingInvited } from '../../../stores/account-management/actions';

export interface ILink {
  route: string;
  name: string;
}

@Component({
  selector: 'manufacturer-invited-user',
  templateUrl: './manufacturer-invited-user.component.html',
  standalone: true,
  imports: [NgFor, NgIf, AsyncPipe, RouterLink],
})
export class ManufacturerInvitedUserComponent implements OnInit, OnDestroy {
  @Input({ required: true }) organisationId?: string;

  command: InviteManufacturerUserCommand = {
    email: '',
    jobTitle: '',
    name: '',
    organisationId: '',
    telephoneNumber: '',
  };

  links: Partial<ILink>[] = [];

  subscription: Subscription | null = null;

  constructor(private store: Store) {}

  ngOnInit() {
    this.subscription = this.store
      .select(selectManufacturerUserBeingInvited)
      .subscribe((existingCommand) => {
        if (!existingCommand) return;
        this.command = { ...existingCommand };
      });
    this.links = [
      {
        route: `/organisation/${this.organisationId}/users`,
        name: 'Back to users',
      },
    ];
  }

  ngOnDestroy() {
    if (this.subscription) this.subscription.unsubscribe();
    this.store.dispatch(clearManufacturerUserBeingInvited());
  }
}
