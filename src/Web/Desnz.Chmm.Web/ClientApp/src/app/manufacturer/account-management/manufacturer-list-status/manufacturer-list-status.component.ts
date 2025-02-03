import { Component, Input, OnInit } from '@angular/core';
import { manufacturerUserStatusStyle } from '../styles';

@Component({
  selector: 'manufacturer-list-status',
  template: `<span class="govuk-tag govuk-tag--{{ colour }} status-column">{{
    status
  }}</span>`,
  styles: [manufacturerUserStatusStyle],
  standalone: true,
})
export class ManufacturerListStatusComponent implements OnInit {
  @Input() get status() {
    return this._status;
  }
  set status(value: string) {
    this._status = value;
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
      default:
        break;
    }
  }
}
