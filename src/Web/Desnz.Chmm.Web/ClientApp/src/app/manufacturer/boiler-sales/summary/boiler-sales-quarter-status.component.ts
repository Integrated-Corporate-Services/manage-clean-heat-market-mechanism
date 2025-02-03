import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'boiler-sales-quarter-status',
  template: `<span class="govuk-tag govuk-tag--{{ colour }}">{{
    status
  }}</span>`,
  standalone: true,
})
export class BoilerSalesQuarterStatusComponent implements OnInit {
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
      case 'Due':
        this.colour = 'red';
        break;
      case 'Awaiting Review':
        this.colour = 'purple';
        break;
      case 'Reviewed':
        this.colour = 'green';
        break;
      case 'N/A':
        this.colour = 'grey';
        break;
      default:
        break;
    }
  }
}
