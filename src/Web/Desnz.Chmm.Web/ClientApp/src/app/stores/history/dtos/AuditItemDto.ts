export interface AuditItemDto {
  eventName: string;
  createdBy: string;
  creationDate: string;
  auditItemRows: AuditItemRow[];
}

export interface AuditItemRow {
  label: string;
  simpleValue: string | null;
  objectValue: any;
}
