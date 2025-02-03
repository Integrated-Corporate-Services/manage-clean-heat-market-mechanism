import { Component, Input, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe, DecimalPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';
import { Router, RouterLink } from '@angular/router';
import { selectSubmitAnnual } from 'src/app/stores/boiler-sales/selectors';
import { submitAnnualBoilerSales } from 'src/app/stores/boiler-sales/actions';
import { SubmitAnnualState } from 'src/app/stores/boiler-sales/state';

@Component({
  selector: 'boiler-sales-annual-check-answers',
  templateUrl: './boiler-sales-annual-check-answers.component.html',
  standalone: true,
  imports: [NgIf, NgFor, AsyncPipe, FormsModule, RouterLink, DecimalPipe],
})
export class BoilerSalesAnnualCheckAnswersComponent implements OnInit {
  @Input() organisationId!: string;
  @Input() schemeYearId!: string;

  year: number = 2024;
  hasConfirmed: boolean = false;
  error: string | null = null;

  boilerSales$: Observable<SubmitAnnualState>;

  constructor(private store: Store, private router: Router) {
    this.boilerSales$ = this.store.select(selectSubmitAnnual);
  }

  ngOnInit() {
    this.validateInputs();
  }

  onSubmit() {
    this.error = null;
    if (!this.hasConfirmed) {
      this.error =
        'Confirm that these details are true and correct to the best of your knowledge.';
    } else {
      this.store.dispatch(
        submitAnnualBoilerSales({
          organisationId: this.organisationId,
          schemeYearId: this.schemeYearId,
        })
      );
    }
  }

  validateInputs() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }

    if (this.schemeYearId === null || this.schemeYearId === undefined) {
      throw TypeError('schemeYearId cannot be null or undefined');
    }
  }
}
