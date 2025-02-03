import { Component, DestroyRef, Input, OnDestroy, OnInit, inject } from '@angular/core';
import { NgIf, NgFor, AsyncPipe, DecimalPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { FormsModule } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { RouterLink } from '@angular/router';
import { selectBoilerSalesQuarters, selectSubmitQuarterly } from 'src/app/stores/boiler-sales/selectors';
import {
  downloadQuarterlySupportingEvidence,
  submitQuarterlyBoilerSales,
} from 'src/app/stores/boiler-sales/actions';
import { SubmitQuarterlyState } from 'src/app/stores/boiler-sales/state';
import { selectSelectedSchemeYear } from 'src/app/stores/scheme-years/selectors';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'boiler-sales-quarterly-check-answers',
  templateUrl: './boiler-sales-quarterly-check-answers.component.html',
  standalone: true,
  imports: [NgIf, NgFor, AsyncPipe, FormsModule, RouterLink, DecimalPipe],
})
export class BoilerSalesQuarterlyCheckAnswersComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;
  @Input({ required: true }) schemeYearQuarterId!: string;
  @Input({ required: true }) mode!: 'submit' | 'edit';

  year: string | null = null;
  quarter?: string | null = null;

  hasConfirmed: boolean = false;
  error: string | null = null;

  boilerSales$: Observable<SubmitQuarterlyState>;

  selectedSchemeYearSubscription = Subscription.EMPTY;

  constructor(private store: Store) {
    this.boilerSales$ = this.store.select(selectSubmitQuarterly);
  }

  ngOnInit() {
    this.validateInputs();

    this.selectedSchemeYearSubscription = this.store
      .select(selectSelectedSchemeYear)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((schemeYear) => {
        if (schemeYear) {
          this.year = schemeYear.year;
          let quarter = schemeYear.quarters.find(
            (q) => q.id === this.schemeYearQuarterId
          );
          if (!quarter) return;
          this.quarter = quarter.name;
        }
      });
  }

  ngOnDestroy() {
    if (this.selectedSchemeYearSubscription) {
      this.selectedSchemeYearSubscription.unsubscribe();
    }
  }

  downloadSupportingEvidence(fileName: string) {
    this.store.dispatch(
      downloadQuarterlySupportingEvidence({
        organisationId: this.organisationId,
        schemeYearQuarterId: this.schemeYearQuarterId!,
        fileName,
      })
    );
  }

  onSubmit() {
    this.error = null;
    if (!this.hasConfirmed) {
      this.error =
        'Confirm that these details are true and correct to the best of your knowledge.';
    } else {
      this.store.dispatch(
        submitQuarterlyBoilerSales({
          organisationId: this.organisationId,
          schemeYearId: this.schemeYearId,
          schemeYearQuarterId: this.schemeYearQuarterId,
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

    if (
      this.schemeYearQuarterId === null ||
      this.schemeYearQuarterId === undefined
    ) {
      throw TypeError('schemeYearQuarterId cannot be null or undefined');
    }
  }
}
