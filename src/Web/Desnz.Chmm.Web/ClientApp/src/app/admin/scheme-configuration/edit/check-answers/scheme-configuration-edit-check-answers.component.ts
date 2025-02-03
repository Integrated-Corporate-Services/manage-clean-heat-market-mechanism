import { Component, Input, OnInit } from '@angular/core';
import { NgIf, AsyncPipe, NgFor, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { Store } from '@ngrx/store';
import { selectUpdateSchemeYearConfigurationCommand } from '../../../../stores/scheme-year-configuration/selectors';
import { UpdateSchemeYearConfigurationCommand } from '../../../../stores/scheme-year-configuration/commands/update-scheme-year-configuration.command';
import { SchemeConfigurationViewDisplayPropertiesComponent } from '../../view/display-properties/scheme-configuration-view-display-properties.component';
import { saveSchemeYearConfiguration } from '../../../../stores/scheme-year-configuration/actions';

@Component({
  selector: 'scheme-configuration-edit-check-answers',
  templateUrl: './scheme-configuration-edit-check-answers.component.html',
  styleUrls: ['./scheme-configuration-edit-check-answers.component.css'],
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, AsyncPipe, SchemeConfigurationViewDisplayPropertiesComponent],
})
export class SchemeConfigurationEditCheckAnswersComponent {

  @Input({ required: true }) schemeYearId!: string;

  command$: Observable<UpdateSchemeYearConfigurationCommand | null>;

  constructor(private store: Store) {
    this.command$ = this.store.select(selectUpdateSchemeYearConfigurationCommand);
  }

  save() {
    this.store.dispatch(saveSchemeYearConfiguration());
  }
}
