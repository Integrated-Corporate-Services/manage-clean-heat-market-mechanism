import { JsonPipe, NgFor, NgIf, NgStyle } from '@angular/common';
import { Component, Input } from '@angular/core';
import { AuditItemRow } from 'src/app/stores/history/dtos/AuditItemDto';

@Component({
  selector: 'audit-item-value',
  templateUrl: './audit-item-value.component.html',
  standalone: true,
  imports: [NgIf, NgFor, JsonPipe, NgStyle],
  styleUrls: ['./audit-item-value.component.css'],
})
export class AuditItemValueComponent {
  @Input({ required: true }) auditItemRows!: AuditItemRow[];
}
