import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukButton } from './govuk-button.options';

const defaultOptions: GovukButton = {
  text: '',
  colour: 'Green',
  disabled: false,
};

@Component({
  selector: 'govuk-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-button.component.html',
  styleUrls: ['./govuk-button.component.css'],
})
export class GovukButtonComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukButton>;

  state: GovukButton = defaultOptions;

  public classes: string = 'govuk-button ';

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };

    if (this.state.colour == 'Grey') this.classes += 'govuk-button--secondary';
    else if (this.state.colour == 'Red')
      this.classes += 'govuk-button--warning';
  }
}
