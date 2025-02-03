import { AsyncPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { getRejectionComments } from 'src/app/stores/administrator-rejection/actions';
import { OrganisationRejectionCommentsDto } from 'src/app/stores/administrator-rejection/dtos/organisation-rejection-comments.dto';
import { selectAdministratorRejectionState } from 'src/app/stores/administrator-rejection/selectors';
import { HttpState } from 'src/app/stores/http-state';

@Component({
  selector: 'view-administrator-rejection',
  templateUrl: './view-administrator-rejection.component.html',
  styleUrls: ['./view-administrator-rejection.component.css'],
  standalone: true,
  imports: [NgFor, NgIf, AsyncPipe, DatePipe],
})
export class ViewAdministratorRejectionComponent {
  @Input({ required: true }) organisationId?: string;

  administratorRejection$: Observable<
    HttpState<OrganisationRejectionCommentsDto>
  >;

  constructor(private store: Store) {
    this.administratorRejection$ = this.store.select(
      selectAdministratorRejectionState
    );
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    this.store.dispatch(
      getRejectionComments({ organisationId: this.organisationId })
    );
  }
}
