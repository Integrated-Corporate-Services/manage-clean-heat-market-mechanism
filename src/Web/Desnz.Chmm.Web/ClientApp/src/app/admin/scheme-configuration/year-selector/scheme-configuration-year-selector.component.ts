import { Component, OnInit } from '@angular/core';
import { NgIf, AsyncPipe, NgFor } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { SchemeYearDto } from '../../../stores/scheme-year-configuration/dtos/scheme-year.dto';
import { Store } from '@ngrx/store';
import { selectSchemeYears } from '../../../stores/scheme-year-configuration/selectors';
import { HttpState } from '../../../stores/http-state';
import { getSchemeYears } from '../../../stores/scheme-year-configuration/actions';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';

@Component({
  selector: 'scheme-configuration-year-selector',
  templateUrl: './scheme-configuration-year-selector.component.html',
  styleUrls: ['./scheme-configuration-year-selector.component.css'],
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, AsyncPipe],
})
export class SchemeConfigurationYearSelectorComponent implements OnInit {

  schemeYears$: Observable<HttpState<SchemeYearDto[]>>;

  constructor(private store: Store, backLinkProvider: BackLinkProvider) {
    backLinkProvider.clear();
    this.schemeYears$ = this.store.select(selectSchemeYears);
  }

  ngOnInit() {
    this.store.dispatch(getSchemeYears());
  }
}
