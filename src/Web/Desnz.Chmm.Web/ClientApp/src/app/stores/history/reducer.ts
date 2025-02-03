import { createReducer, on } from '@ngrx/store';
import { HttpState } from '../http-state';
import { AuditItemDto } from './dtos/AuditItemDto';
import { HistoryActions } from '..';

const defaultState: HttpState<AuditItemDto[]> = {
  loading: false,
  errorMessage: null,
  data: null,
};

export const historyReducer = createReducer(
  defaultState,
  on(HistoryActions.getAuditItems, (state, _) => {
    return {
      ...state,
      loading: true,
      errorMessage: null,
    };
  }),
  on(HistoryActions.onGetAuditItems, (state, action) => {
    return {
      ...state,
      loading: false,
      data: [...action.auditItems],
    };
  }),
  on(HistoryActions.onGetAuditItemsError, (state, action) => {
    return {
      ...state,
      loading: false,
      errorMessage: action.message,
    };
  })
);
