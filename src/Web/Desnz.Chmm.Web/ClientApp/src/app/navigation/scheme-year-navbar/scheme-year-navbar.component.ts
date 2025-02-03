import { NgIf } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import {
  toggleSchemeYearSelector,
  toggleSecondaryNav,
  toggleServiceNav,
} from 'src/app/stores/navigation/actions';

@Component({
  selector: 'scheme-year-navbar',
  templateUrl: './scheme-year-navbar.component.html',
  styleUrls: ['./scheme-year-navbar.component.css'],
  standalone: true,
  imports: [NgIf, RouterLink],
})
export class SchemeYearNavbarComponent {
  @Input({ required: true }) organisationName?: string | null = null;
  @Input({ required: true }) organisationId?: string | null = null;
  @Input({ required: true }) isAdmin!: boolean;
  @Input({ required: true }) schemeYear!: string;

  constructor(private router: Router, private store: Store) {}

  onChangeSchemeYear() {
    this.store.dispatch(toggleSchemeYearSelector({ show: false }));
    this.router.navigate([`scheme-year`], {
      queryParams: {
        organisationId: this.organisationId,
      },
    });
    if (!this.isAdmin) {
      this.store.dispatch(toggleServiceNav({ show: false }));
    } else {
      this.store.dispatch(toggleSecondaryNav({ show: false }));
    }
  }
}
