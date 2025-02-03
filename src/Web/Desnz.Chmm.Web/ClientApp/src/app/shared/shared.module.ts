import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukButtonComponent } from './components/govuk/govuk-button/govuk-button.component';
import { HttpClientModule } from '@angular/common/http';
import { GovukCheckboxesComponent } from './components/govuk/govuk-checkboxes/govuk-checkboxes.component';
import { GovukDateComponent } from './components/govuk/govuk-date/govuk-date.component';
import { GovukDetailsComponent } from './components/govuk/govuk-details/govuk-details.component';
import { GovukErrorSummaryComponent } from './components/govuk/govuk-error-summary/govuk-error-summary.component';
import { GovukFileUploadComponent } from './components/govuk/govuk-file-upload/govuk-file-upload.component';
import { GovukInputComponent } from './components/govuk/govuk-input/govuk-input.component';
import { GovukInsetTextComponent } from './components/govuk/govuk-inset-text/govuk-inset-text.component';
import { GovukNotificationBannerComponent } from './components/govuk/govuk-notification-banner/govuk-notification-banner.component';
import { GovukPanelComponent } from './components/govuk/govuk-panel/govuk-panel.component';
import { GovukRadiosComponent } from './components/govuk/govuk-radios/govuk-radios.component';
import { GovukSelectComponent } from './components/govuk/govuk-select/govuk-select.component';
import { GovukSummaryListComponent } from './components/govuk/govuk-summary-list/govuk-summary-list.component';
import { GovukTableComponent } from './components/govuk/govuk-table/govuk-table.component';
import { GovukTagComponent } from './components/govuk/govuk-tag/govuk-tag.component';
import { GovukTextareaComponent } from './components/govuk/govuk-textarea/govuk-textarea.component';
import { GovukWarningTextComponent } from './components/govuk/govuk-warning-text/govuk-warning-text.component';

const exports = [
  CommonModule,
  HttpClientModule,
  GovukButtonComponent,
  GovukCheckboxesComponent,
  GovukDateComponent,
  GovukDetailsComponent,
  GovukErrorSummaryComponent,
  GovukFileUploadComponent,
  GovukInputComponent,
  GovukInsetTextComponent,
  GovukNotificationBannerComponent,
  GovukPanelComponent,
  GovukRadiosComponent,
  GovukSelectComponent,
  GovukSummaryListComponent,
  GovukTableComponent,
  GovukTagComponent,
  GovukTextareaComponent,
  GovukWarningTextComponent,
];

@NgModule({
  imports: exports,
  exports: exports,
})
export class SharedModule {}
