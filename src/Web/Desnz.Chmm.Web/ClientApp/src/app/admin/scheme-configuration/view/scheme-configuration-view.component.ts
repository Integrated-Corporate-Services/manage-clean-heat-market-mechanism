import { Component, Input, OnInit } from '@angular/core';
import { NgIf, AsyncPipe, NgFor, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { SchemeYearDto } from '../../../stores/scheme-year-configuration/dtos/scheme-year.dto';
import { Store } from '@ngrx/store';
import { selectSchemeYear, selectSchemeYearConfiguration } from '../../../stores/scheme-year-configuration/selectors';
import { HttpState } from '../../../stores/http-state';
import { getSchemeYear } from '../../../stores/scheme-year-configuration/actions';
import { SchemeYearConfigurationDto } from '../../../stores/scheme-year-configuration/dtos/scheme-year-configuration.dto';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { SchemeConfigurationViewDisplayPropertiesComponent } from './display-properties/scheme-configuration-view-display-properties.component';

@Component({
  selector: 'scheme-configuration-view',
  templateUrl: './scheme-configuration-view.component.html',
  styleUrls: ['./scheme-configuration-view.component.css'],
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, AsyncPipe, SchemeConfigurationViewDisplayPropertiesComponent],
})
export class SchemeConfigurationViewComponent implements OnInit {

  @Input({ required: true }) schemeYearId!: string;

  schemeYear$: Observable<HttpState<SchemeYearDto>>;
  schemeYearConfiguration$: Observable<HttpState<SchemeYearConfigurationDto>>;

  constructor(private store: Store, backLinkProvider: BackLinkProvider) {
    backLinkProvider.link = '/admin/scheme-year-configuration';
    this.schemeYear$ = this.store.select(selectSchemeYear);
    this.schemeYearConfiguration$ = this.store.select(selectSchemeYearConfiguration);
  }

  ngOnInit() {
    this.store.dispatch(
      getSchemeYear({
        schemeYearId: this.schemeYearId
      })
    );
  }
}
