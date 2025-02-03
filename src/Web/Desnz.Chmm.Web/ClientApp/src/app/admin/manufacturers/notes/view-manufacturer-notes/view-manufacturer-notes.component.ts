import { Component, Input, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe, DatePipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BackLinkProvider } from '../../../../navigation/back-link/back-link.provider';
import {
  getAdminUsers,
  getManufacturerNotes,
} from '../../../../stores/account-management/actions';
import { Observable } from 'rxjs';
import { ManufacturerNote } from '../../../../stores/account-management/dtos/manufacturer-note';
import { HttpState } from '../../../../stores/http-state';
import {
  selectAdminUsers,
  selectManufacturerExistingNoteFiles,
  selectManufacturerNotes,
} from '../../../../stores/account-management/selectors';
import { ChmmUser } from '../../../../stores/account-management/dtos/chmm-user';

@Component({
  selector: 'view-manufacturer-notes',
  templateUrl: './view-manufacturer-notes.component.html',
  styleUrls: ['./view-manufacturer-notes.component.css'],
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe,
    DatePipe,
  ],
})
export class ViewManufacturerNotesComponent implements OnInit {
  @Input() organisationId?: string;
  @Input({ required: true }) schemeYearId!: string;

  notes$: Observable<HttpState<ManufacturerNote[]>>;
  noteFiles$: Observable<Record<string, string[]>>;
  admins$: Observable<HttpState<ChmmUser[]>>;

  constructor(private store: Store, backLinkProvider: BackLinkProvider) {
    backLinkProvider.clear();
    this.notes$ = this.store.select(selectManufacturerNotes);
    this.noteFiles$ = this.store.select(selectManufacturerExistingNoteFiles);
    this.admins$ = this.store.select(selectAdminUsers);
  }

  ngOnInit() {
    this.store.dispatch(
      getManufacturerNotes({
        organisationId: this.organisationId!,
        schemeYearId: this.schemeYearId,
      })
    );
    this.store.dispatch(getAdminUsers());
  }
}
