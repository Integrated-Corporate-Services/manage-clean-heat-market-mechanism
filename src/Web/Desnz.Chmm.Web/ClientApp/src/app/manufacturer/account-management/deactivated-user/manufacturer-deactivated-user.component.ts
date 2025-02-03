import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { selectManufacturerUser } from 'src/app/stores/account-management/selectors';
import { Observable } from 'rxjs';
import { RouterLink } from '@angular/router';
import {
  clearManufacturerUserBeingEdited,
  getManufacturerUser,
} from '../../../stores/account-management/actions';
import { ChmmUser } from '../../../stores/account-management/dtos/chmm-user';
import { HttpState } from '../../../stores/http-state';

export interface ILink {
  route: string;
  name: string;
}

@Component({
  selector: 'manufacturer-deactivated-user',
  templateUrl: './manufacturer-deactivated-user.component.html',
  standalone: true,
  imports: [NgFor, NgIf, AsyncPipe, RouterLink],
})
export class ManufacturerDeactivatedUserComponent implements OnInit, OnDestroy {
  @Input({ required: true }) organisationId?: string;
  @Input({ required: true }) userId?: string;

  user$: Observable<HttpState<ChmmUser | null>>;

  links: Partial<ILink>[] = [];

  constructor(private store: Store) {
    this.user$ = this.store.select(selectManufacturerUser);
  }

  ngOnInit() {
    this.store.dispatch(
      getManufacturerUser({
        organisationId: this.organisationId!,
        userId: this.userId!,
      })
    );

    this.links = [
      {
        route: `/organisation/${this.organisationId}/users`,
        name: 'Back to users',
      },
    ];
  }

  ngOnDestroy() {
    this.store.dispatch(clearManufacturerUserBeingEdited());
  }
}
