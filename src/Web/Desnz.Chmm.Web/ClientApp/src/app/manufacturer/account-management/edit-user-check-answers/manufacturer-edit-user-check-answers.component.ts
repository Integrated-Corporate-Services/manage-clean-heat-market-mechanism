import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Subscription, Observable } from 'rxjs';
import { selectManufacturerUser, selectManufacturerUserBeingEdited } from '../../../stores/account-management/selectors';
import { clearManufacturerUserBeingEdited, getManufacturerUser, updateManufacturerUser } from '../../../stores/account-management/actions';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { FormsModule } from '@angular/forms';
import { EditManufacturerUserCommand } from '../../../stores/account-management/commands/edit-manufacturer-user-command';


@Component({
  selector: 'manufacturer-edit-user-check-answers',
  templateUrl: './manufacturer-edit-user-check-answers.component.html',
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
export class ManufacturerEditUserCheckAnswersComponent {

  @Input({ required: true }) organisationId?: string;
  @Input({ required: true }) userId?: string;

  errors: any = {};
  editManufacturerUser$: Observable<EditManufacturerUserCommand | null>;

  constructor(
    private store: Store,
    private router: Router,) {
        this.editManufacturerUser$ = this.store.select(selectManufacturerUserBeingEdited);
  }

  onSubmit(editUser: EditManufacturerUserCommand) {
    this.errors = {};
    this.store.dispatch(updateManufacturerUser({ command: editUser }));
  }

  cancel() {
    this.store.dispatch(clearManufacturerUserBeingEdited());
    this.router.navigateByUrl(`/organisation/${this.organisationId}/users`);
  }
}
