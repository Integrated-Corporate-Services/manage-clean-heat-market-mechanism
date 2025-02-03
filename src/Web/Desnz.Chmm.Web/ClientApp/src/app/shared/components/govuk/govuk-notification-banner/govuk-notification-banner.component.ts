import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GovukNotificationBanner } from './govuk-notification-banner.options';
import { Router } from '@angular/router';

const defaultOptions: GovukNotificationBanner = {
  title: '',
  text: '',
  link: null,
};

@Component({
  selector: 'govuk-notification-banner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-notification-banner.component.html',
  styleUrls: ['./govuk-notification-banner.component.css'],
})
export class GovukNotificationBannerComponent implements OnChanges {
  @Input({ required: true }) options!: Partial<GovukNotificationBanner>;

  state: GovukNotificationBanner = defaultOptions;

  constructor(private router: Router) {}

  ngOnChanges(changes: SimpleChanges): void {
    this.state = { ...defaultOptions, ...changes['options'].currentValue };
  }

  public redirect() {
    this.router.navigateByUrl(this.state?.link?.url ?? '/');
  }
}
