import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'manufacturers-list-status',
  template: `<span class="govuk-tag govuk-tag--{{ colour }}">{{
    status
  }}</span>`,
  styles: ['span { width: 8ch; text-align: center; }'],
  standalone: true,
})
export class ManufacturersListStatusComponent implements OnInit {
  @Input() get status() {
    return this._status;
  }
  set status(value: string | null | undefined) {
    this._status = value || 'Unknown';
    this.setColour();
  }
  _status: string = 'Unknown';
  colour: string = 'grey';

  ngOnInit() { }

  setColour() {
    switch (this.status) {
      case 'Pending':
        this.colour = 'purple';
        break;
      case 'Active':
        this.colour = 'green';
        break;
      case 'Inactive':
        this.colour = 'grey';
        break;
      case 'Retired':
        this.colour = 'red';
        break;
      case 'Archived':
        this.colour = 'default';
        break;
      default:
        break;
    }
  }
}
