import { Component, Input, OnInit } from '@angular/core';
import { NgIf, AsyncPipe, NgFor, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { Store } from '@ngrx/store';
import { selectSchemeYear } from '../../../../stores/scheme-year-configuration/selectors';
import { getSchemeYear } from '../../../../stores/scheme-year-configuration/actions';
import { HttpState } from '../../../../stores/http-state';
import { SchemeYearDto } from '../../../../stores/scheme-year-configuration/dtos/scheme-year.dto';
import { SchemeConfigurationEndDateFilterPipe } from './scheme-configuration-end-date.filter';

@Component({
  selector: 'scheme-configuration-edit-confirmation',
  templateUrl: './scheme-configuration-edit-confirmation.component.html',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, AsyncPipe, SchemeConfigurationEndDateFilterPipe]
})
export class SchemeConfigurationEditConfirmationComponent implements OnInit {

  @Input({ required: true }) schemeYearId!: string;

  schemeYear$: Observable<HttpState<SchemeYearDto>>;

  constructor(private store: Store) {
    this.schemeYear$ = this.store.select(selectSchemeYear);
  }

  ngOnInit() {
    this.store.dispatch(
      getSchemeYear({
        schemeYearId: this.schemeYearId
      })
    );
  }
}
