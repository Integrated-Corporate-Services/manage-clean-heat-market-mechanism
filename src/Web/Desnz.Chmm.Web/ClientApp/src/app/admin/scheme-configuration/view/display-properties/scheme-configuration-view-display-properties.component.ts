import { Component, Input } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { SchemeYearConfigurationDto } from '../../../../stores/scheme-year-configuration/dtos/scheme-year-configuration.dto';
import { UpdateSchemeYearConfigurationCommand } from '../../../../stores/scheme-year-configuration/commands/update-scheme-year-configuration.command';

@Component({
  selector: 'scheme-configuration-view-display-properties',
  templateUrl: './scheme-configuration-view-display-properties.component.html',
  styleUrls: ['./scheme-configuration-view-display-properties.component.css'],
  standalone: true,
  imports: [DecimalPipe],
})
export class SchemeConfigurationViewDisplayPropertiesComponent {

  @Input({ required: true }) values!: UpdateSchemeYearConfigurationCommand | SchemeYearConfigurationDto;

}
