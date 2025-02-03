import { createAction, props } from '@ngrx/store';
import { AuditItemDto } from './dtos/AuditItemDto';

export const getAuditItems = createAction(
  '[History] Get audit items',
  props<{ organisationId: string }>()
);

export const onGetAuditItems = createAction(
  '[History] On: Get audit items',
  props<{ auditItems: AuditItemDto[] }>()
);

export const onGetAuditItemsError = createAction(
  '[History] On Error: Get audit items',
  props<{ message: string }>()
);
