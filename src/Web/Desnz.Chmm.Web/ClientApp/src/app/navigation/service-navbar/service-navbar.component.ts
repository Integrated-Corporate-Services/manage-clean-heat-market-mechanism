import { NgIf } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Store } from '@ngrx/store';
import {
  toggleSchemeYearSelector,
  toggleSecondaryNav,
} from 'src/app/stores/navigation/actions';

@Component({
  selector: 'service-navbar',
  templateUrl: './service-navbar.component.html',
  styleUrls: ['./service-navbar.component.css'],
  standalone: true,
  imports: [RouterLink, RouterLinkActive, NgIf],
})
export class ServiceNavbarComponent {
  @Input() organisationId?: string | null;
  @Input({ required: true }) schemeYearId?: string;
  @Input({ required: true }) schemeYearName?: string;
  @Input({ required: true }) showAdminLinks: boolean | null = false;
  @Input({ required: true }) showManufacturerLinks: boolean | null = false;

  constructor(private store: Store) {}

  onSelectLink() {
    this.store.dispatch(toggleSecondaryNav({ show: false }));
    this.store.dispatch(toggleSchemeYearSelector({ show: false }));
  }
}
