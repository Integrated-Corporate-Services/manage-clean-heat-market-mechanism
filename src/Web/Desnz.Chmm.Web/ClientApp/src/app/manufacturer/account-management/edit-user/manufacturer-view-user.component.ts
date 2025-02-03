import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs';
import { selectManufacturerUser } from '../../../stores/account-management/selectors';
import { clearManufacturerUserBeingEdited, editManufacturerUser, getManufacturerUser } from '../../../stores/account-management/actions';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { FormsModule } from '@angular/forms';
import { EditManufacturerUserCommand } from '../../../stores/account-management/commands/edit-manufacturer-user-command';
import { ChmmUser } from '../../../stores/account-management/dtos/chmm-user';

@Component({
  selector: 'manufacturer-view-user',
  templateUrl: './manufacturer-view-user.component.html',
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
export class ManufacturerViewUserComponent implements OnInit, OnDestroy {

  @Input({ required: true }) organisationId?: string;
  @Input({ required: true }) userId?: string;

  errors: any = {};
  user: ChmmUser | undefined | null = null;
  command: EditManufacturerUserCommand = {
    email: '',
    jobTitle: '',
    name: '',
    organisationId: '',
    telephoneNumber: '',
    id: ''
  };

  usersSubscription: Subscription | null = null;
  commandSubscription: Subscription | null = null;

  constructor(
    private store: Store,
    private router: Router,
    private backLinkProvider: BackLinkProvider) {
  }

  ngOnInit() {
    this.command.organisationId = this.organisationId!;
    this.command.id = this.userId!;
    this.backLinkProvider.link = `/organisation/${this.organisationId}/users`;

    this.usersSubscription = this.store.select(selectManufacturerUser).subscribe(user => {
      if (!user.data) return;

      this.user = user.data;

      this.command = {
        ...this.user!,
        jobTitle: this.user!.jobTitle!,
        organisationId: this.organisationId!,
        id: this.userId!
      };
    });

    this.store.dispatch(getManufacturerUser({
      organisationId: this.organisationId!,
      userId: this.userId!
    }));
  }

  onSubmit(){
    this.store.dispatch(editManufacturerUser({
      command: this.command
    }));
  }

  ngOnDestroy() {
    if (this.commandSubscription) this.commandSubscription.unsubscribe();
    if (this.usersSubscription) this.usersSubscription.unsubscribe();
  }

  deactivate() {
    this.router.navigateByUrl(`/organisation/${this.organisationId}/users/${this.userId}/deactivate`);
  }

  cancel() {
    this.store.dispatch(clearManufacturerUserBeingEdited());
    this.router.navigateByUrl(`/organisation/${this.organisationId}/users`);
  }
}
