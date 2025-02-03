import { createSelector } from '@ngrx/store';
import { AppState } from '..';
import { LicenceHolderState } from './state';
import { LinkLicenceHolderFormValue } from 'src/app/admin/manufacturers/licence-holders/link-licence-holder/link-licence-holder.component';
import * as moment from 'moment';
import { EditLinkedLicenceHolderFormValue } from 'src/app/admin/manufacturers/licence-holders/edit-linked-licence-holder/edit-linked-licence-holder.component';

export const selectLicenceHolderState = (state: AppState) =>
  state.licenceHolderState;

export const selectLinkedLicenceHolders = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.linked
);

export const selectUnlinkedLicenceHolders = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.unlinked
);

export const selectSelectedLicenceHolder = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.selectedLicenceHolder
);

export const selectSelectedLicenceHolderLink = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.selectedLicenceHolderLink
);

export const selectLinkLicenceHolderFormValue = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.linkLicenceHolderFormValue
);

export const selectEditLinkedLicenceHolderFormValue = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.editLinkedLicenceHolderFormValue
);

export const selectStarOfLink = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.startOfLink
);

export const selectEndOfLink = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.endOfLink
);

export const selectLinkLicenceHolderDto = createSelector(
  selectLinkLicenceHolderFormValue,
  (link: LinkLicenceHolderFormValue) => {
    return {
      startDate:
        link.day !== null && link.month !== null && link.year !== null
          ? moment([link.year!, Number(link.month) - 1, link.day!]).format(
              'YYYY-MM-DD'
            )
          : null,
    };
  }
);

export const selectCurrentSchemeYear = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.currentSchemeYear
);

export const selectEditLinkedLicenceHolderDto = createSelector(
  selectEditLinkedLicenceHolderFormValue,
  (link: EditLinkedLicenceHolderFormValue) => {
    return {
      organisationIdToTransfer: link.organisationId,
      endDate:
        link.day !== null && link.month !== null && link.year !== null
          ? moment([link.year!, Number(link.month) - 1, link.day!]).format(
              'YYYY-MM-DD'
            )
          : null,
    };
  }
);

export const selectEditLinkedLicenceHolderLoading = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.editLinkedLicenceHolder.loading
);

export const selectEditLinkedLicenceHolderError = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.editLinkedLicenceHolder.errorMessage
);

export const selectFirstSchemeYear = createSelector(
  selectLicenceHolderState,
  (state: LicenceHolderState) => state.firstSchemeYear
);
