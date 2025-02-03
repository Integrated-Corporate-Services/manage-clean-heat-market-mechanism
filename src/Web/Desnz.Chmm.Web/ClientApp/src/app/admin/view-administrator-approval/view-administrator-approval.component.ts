import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { getApprovalComments } from 'src/app/stores/administrator-approval/actions';
import { OrganisationApprovalCommentsDto } from 'src/app/stores/administrator-approval/dtos/organisation-approval-comments.dto';
import { selectAdministratorApprovalState } from 'src/app/stores/administrator-approval/selectors';
import { HttpState } from 'src/app/stores/http-state';

@Component({
  selector: 'view-administrator-approval',
  templateUrl: './view-administrator-approval.component.html',
  styleUrls: ['./view-administrator-approval.component.css'],
  standalone: true,
  imports: [NgFor, NgIf, AsyncPipe],
})
export class ViewAdministratorApprovalComponent {
  @Input({ required: true }) organisationId?: string;

  administratorApproval$: Observable<
    HttpState<OrganisationApprovalCommentsDto>
  >;

  constructor(private store: Store) {
    this.administratorApproval$ = this.store.select(
      selectAdministratorApprovalState
    );
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    this.store.dispatch(
      getApprovalComments({ organisationId: this.organisationId })
    );
  }
}
