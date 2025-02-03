import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'register-organisation',
  templateUrl: './register-organisation.component.html',
  standalone: true,
  imports: [RouterLink],
})
export class RegisterOrganisationComponent {
  constructor(private store: Store) {}
}
