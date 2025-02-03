import { Component, Input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'admin-secondary-navbar',
  templateUrl: './admin-secondary-navbar.component.html',
  styleUrls: ['./admin-secondary-navbar.component.css'],
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
})
export class AdminSecondaryNavbarComponent {
  @Input({ required: true }) organisationId?: string | null;
  @Input({ required: true }) schemeYearId!: string;
}
