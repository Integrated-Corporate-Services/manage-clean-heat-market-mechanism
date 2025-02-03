import { Route } from '@angular/router';
import { LinkLicenceHolderComponent } from 'src/app/admin/manufacturers/licence-holders/link-licence-holder/link-licence-holder.component';
import { ViewLicenceHoldersComponent } from 'src/app/admin/manufacturers/licence-holders/view-licence-holders/view-licence-holders.component';
import { ConfirmLinkLicenceHolderComponent } from '../../app/admin/manufacturers/licence-holders/confirm-link-licence-holder/confirm-link-licence-holder.component';
import { LinkedLicenceHolderComponent } from '../../app/admin/manufacturers/licence-holders/linked-licence-holder/linked-licence-holder.component';
import { EditLinkedLicenceHolderComponent } from 'src/app/admin/manufacturers/licence-holders/edit-linked-licence-holder/edit-linked-licence-holder.component';
import { ConfirmEditLinkedLicenceHolderComponent } from 'src/app/admin/manufacturers/licence-holders/confirm-edit-linked-licence-holder/confirm-edit-linked-licence-holder.component';
import { EditedLicenceHolderComponent } from 'src/app/admin/manufacturers/licence-holders/edited-licence-holder/edited-licence-holder.component';

export const licenceHoldersRoutes: Route[] = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'view',
  },
  {
    path: 'view',
    component: ViewLicenceHoldersComponent,
  },
  {
    path: 'link',
    component: LinkLicenceHolderComponent,
  },
  {
    path: ':licenceHolderId/confirm-link',
    component: ConfirmLinkLicenceHolderComponent,
  },
  {
    path: ':licenceHolderId/linked',
    component: LinkedLicenceHolderComponent,
  },
  {
    path: ':licenceHolderId/edit',
    component: EditLinkedLicenceHolderComponent,
  },
  {
    path: ':licenceHolderId/confirm-edit',
    component: ConfirmEditLinkedLicenceHolderComponent,
  },
  {
    path: ':licenceHolderId/edited',
    component: EditedLicenceHolderComponent,
  },
];
