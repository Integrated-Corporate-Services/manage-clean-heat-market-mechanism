import { Component, Input } from '@angular/core';
import { Store } from '@ngrx/store';
import { RouterLink } from '@angular/router';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { LicenceHolderDto } from 'src/app/stores/licence-holders/dto/licence-holder';
import { selectSelectedLicenceHolder } from 'src/app/stores/licence-holders/selectors';

@Component({
  selector: 'linked-licence-holder',
  templateUrl: './linked-licence-holder.component.html',
  standalone: true,
  imports: [FormsModule, RouterLink, NgFor, NgIf, AsyncPipe],
})
export class LinkedLicenceHolderComponent {
  @Input() organisationId?: string;
  @Input() licenceHolderId?: string;

  licenceHolder$: Observable<LicenceHolderDto | null>;

  constructor(private store: Store) {
    this.licenceHolder$ = this.store.select(selectSelectedLicenceHolder);
  }
}
