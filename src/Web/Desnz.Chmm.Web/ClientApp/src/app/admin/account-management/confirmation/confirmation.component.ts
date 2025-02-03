import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { selectUserBeingEdited } from 'src/app/stores/account-management/selectors';
import { Subscription } from 'rxjs';
import { ChmmUser } from 'src/app/stores/account-management/dtos/chmm-user';
import { RouterLink } from '@angular/router';

export interface ILink {
  route: string;
  name: string;
  onClick: (() => any) | null;
}

@Component({
  selector: 'confirmation',
  templateUrl: './confirmation.component.html',
  standalone: true,
  imports: [NgFor, NgIf, AsyncPipe, RouterLink],
})
export class ConfirmationComponent implements OnInit, OnDestroy {
  @Input() mode: string = 'edit';

  title: string = '';
  subtitle: string = '';
  whatHappensNext: string = '';
  links: Partial<ILink>[] = [
    {
      route: '/admin/users',
      name: 'Back to Administrator users',
    }
  ];

  subscription: Subscription | null = null;

  constructor(private store: Store) { }

  private setAdd(user: ChmmUser) {
    this.title = 'New user added';
    this.subtitle = 'Added ' + user.name;
    this.whatHappensNext = 'The user account has now been created. They will need to go to the M-CHMM IT system start page to log in. They will need to create a GOV.UK One Login to do so.';
  }

  private setEdit(user: ChmmUser) {
    this.title = 'User edited';
    this.subtitle = user.name + '\'s details have been updated';
    this.whatHappensNext = 'The user details have been updated and you can now see these on their account.';
  }

  private setActivate(user: ChmmUser) {
    this.title = 'User activated';
    this.subtitle = user.name + '\'s account has been activated';
    this.whatHappensNext = 'This user has now been activated. They will now have access to the M-CHMM system.';
  }

  private setDeactivate(user: ChmmUser) {
    this.title = 'User deactivated';
    this.subtitle = user.name + '\'s account has been deactivated';
    this.whatHappensNext = 'This user has now been deactivated. They will no longer have access to the M-CHMM system.';
  }

  ngOnInit() {
    this.subscription = this.store.select(selectUserBeingEdited)
      .subscribe((user) => {
        if (!user.data) return;
        if (this.mode == 'add') this.setAdd(user.data);
        else if (this.mode == 'edit') this.setEdit(user.data);
        else if (this.mode == 'activated') this.setActivate(user.data);
        else if (this.mode == 'deactivated') this.setDeactivate(user.data);
      });
  }

  ngOnDestroy() {
    if (!!this.subscription)
      this.subscription.unsubscribe();
  }
}
