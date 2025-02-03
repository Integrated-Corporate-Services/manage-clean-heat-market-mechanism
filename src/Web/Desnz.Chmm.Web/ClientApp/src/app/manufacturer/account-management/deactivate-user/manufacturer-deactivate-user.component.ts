import { Component, Input } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { clearManufacturerUserBeingEdited, deactivateManufacturerUser } from '../../../stores/account-management/actions';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';

@Component({
  selector: 'manufacturer-deactivate-user',
  templateUrl: './manufacturer-deactivate-user.component.html',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe
  ],
})
export class ManufacturerDeactivateUserComponent {

  @Input({ required: true }) organisationId?: string;
  @Input({ required: true }) userId?: string;

  constructor(private store: Store, private router: Router, backLinkProvider: BackLinkProvider) {
    backLinkProvider.clear();
  }

  deactivate() {
    this.store.dispatch(deactivateManufacturerUser({
      organisationId: this.organisationId!,
      userId: this.userId!
    }));
  }

  back() {
    this.store.dispatch(clearManufacturerUserBeingEdited());
    this.router.navigateByUrl(`/organisation/${this.organisationId}/users`);
  }
}
