import { createAction, props } from '@ngrx/store';
import { LicenceHolderDto } from './dto/licence-holder';
import { LinkLicenceHolderFormValue } from 'src/app/admin/manufacturers/licence-holders/link-licence-holder/link-licence-holder.component';
import { SchemeYearDto } from '../organisation-summary/dtos/scheme-year.dto';
import { LicenceHolderLinkDto } from './dto/licence-holder-link.dto';
import { EditLinkedLicenceHolderFormValue } from 'src/app/admin/manufacturers/licence-holders/edit-linked-licence-holder/edit-linked-licence-holder.component';

export const getAllLicenceHolders = createAction(
  '[Licence Holders] Get all licence holders'
);
export const getUnlinkedLicenceHolders = createAction(
  '[Licence Holders] Get unlinked licence holders'
);
export const getLinkedLicenceHolders = createAction(
  '[Licence Holders] Get linked licence holders',
  props<{ organisationId: string }>()
);
export const linkLicenceHolder = createAction(
  '[Licence Holders] Link licence holder',
  props<{ licenceHolderId: string; organisationId: string }>()
);
export const onGetAllLicenceHolders = createAction(
  '[Licence Holders] On: Get all licence holders',
  props<{ licenceHolders: LicenceHolderDto[] }>()
);
export const onGetUnlinkedLicenceHolders = createAction(
  '[Licence Holders] On: Get unlinked licence holders',
  props<{ licenceHolders: LicenceHolderDto[] }>()
);
export const onGetLinkedLicenceHolders = createAction(
  '[Licence Holders] On: Get linked licence holders',
  props<{
    licenceHolderLinks: LicenceHolderLinkDto[];
    schemeYearStartDate: string;
  }>()
);
export const onLinkLicenceHolder = createAction(
  '[Licence Holders] On: Link licence holder',
  props<{ organisationId: string }>()
);

export const onErrorGetAllLicenceHolders = createAction(
  '[Licence Holders] On Error: Get all licence holders',
  props<{ message: string }>()
);
export const onErrorGetUnlinkedLicenceHolders = createAction(
  '[Licence Holders] On Error: Get unlinked licence holders',
  props<{ message: string }>()
);
export const onErrorGetLinkedLicenceHolders = createAction(
  '[Licence Holders] On Error: Get linked licence holders',
  props<{ message: string }>()
);
export const onErrorLinkLicenceHolder = createAction(
  '[Licence Holders] On Error: Link licence holder',
  props<{ message: string }>()
);

export const storeLinkLicenceHolderFormValue = createAction(
  '[Licence Holders] Store link licence holder form value',
  props<{
    organisationId: string;
    linkLicenceHolderFormValue: LinkLicenceHolderFormValue;
  }>()
);

export const storeEditLinkedLicenceHolderFormValue = createAction(
  '[Licence Holders] Store edit linked licence holder form value',
  props<{
    organisationId: string;
    licenceHolderId: string;
    editLinkedLicenceHolderFormValue: EditLinkedLicenceHolderFormValue;
  }>()
);

export const clearLinkLicenceHolderFormValue = createAction(
  '[Licence Holders] Clear link licence holder form value',
  props<{
    organisationId: string;
  }>()
);

export const getFirstSchemeYear = createAction(
  '[Licence Holders] Get first scheme year'
);

export const onGetFirstSchemeYear = createAction(
  '[Licence Holders] On: Get first scheme year',
  props<{
    schemeYear: SchemeYearDto;
  }>()
);

export const onGetFirstSchemeYearError = createAction(
  '[Licence Holders] On Error: Get first scheme year',
  props<{
    message: string;
  }>()
);

export const getCurrentSchemeYear = createAction(
  '[Licence Holders] Get current scheme year',
  props<{
    organisationId: string;
  }>()
);

export const onGetCurrentSchemeYear = createAction(
  '[Licence Holders] On: Get current scheme year',
  props<{
    schemeYear: SchemeYearDto;
    organisationId: string;
  }>()
);

export const onGetCurrentSchemeYearError = createAction(
  '[Licence Holders] On Error: Get current scheme year',
  props<{
    message: string;
  }>()
);

export const selectLinkedLincenceHolder = createAction(
  '[Licence Holders] Select linked licence holder',
  props<{
    organisationId: string;
    licenceHolderLink: LicenceHolderLinkDto;
  }>()
);

export const editLinkedLicenceHolder = createAction(
  '[Licence Holders] Edit linked licence holder',
  props<{
    organisationId: string;
    licenceHolderId: string;
  }>()
);

export const onEditLinkedLicenceHolder = createAction(
  '[Licence Holders] On: Edit linked licence holder',
  props<{
    licenceHolderId: string;
  }>()
);

export const onEditLinkedLicenceHolderError = createAction(
  '[Licence Holders] On Error: Edit linked licence holder',
  props<{
    message: string;
  }>()
);
