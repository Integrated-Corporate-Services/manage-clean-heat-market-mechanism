import { createReducer, on } from '@ngrx/store';
import { NavigationActions } from '..';

export type SelectedLink =
  | 'summary'
  | 'boilerSales'
  | 'account'
  | 'notes'
  | 'licenceHolders'
  | null;

export interface NavigationState {
  hideSecondaryNav: boolean;
  selectedLink: SelectedLink;
  hideSchemeYearSelector: boolean;
  hideServiceNav: boolean;
}

const defaultState: NavigationState = {
  hideSecondaryNav: false,
  selectedLink: null,
  hideSchemeYearSelector: false,
  hideServiceNav: false,
};

export const navigationReducer = createReducer(
  defaultState,
  on(NavigationActions.toggleSecondaryNav, (state, action): NavigationState => {
    return {
      ...state,
      hideSecondaryNav: !action.show,
    };
  }),
  on(NavigationActions.toggleServiceNav, (state, action): NavigationState => {
    return {
      ...state,
      hideServiceNav: !action.show,
    };
  }),
  on(NavigationActions.selectLink, (state, action): NavigationState => {
    return {
      ...state,
      selectedLink: action.selectedLink,
    };
  }),
  on(
    NavigationActions.toggleSchemeYearSelector,
    (state, action): NavigationState => {
      return {
        ...state,
        hideSchemeYearSelector: !action.show,
      };
    }
  )
);
