import { Route } from '@angular/router';
import { AddManufacturerNoteComponent } from 'src/app/admin/manufacturers/notes/add-manufacturer-note/add-manufacturer-note.component';
import { ViewManufacturerNotesComponent } from 'src/app/admin/manufacturers/notes/view-manufacturer-notes/view-manufacturer-notes.component';

export const notesRoutes: Route[] = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'view',
  },
  {
    path: 'view',
    component: ViewManufacturerNotesComponent,
  },
  {
    path: 'add',
    component: AddManufacturerNoteComponent,
  },
];
