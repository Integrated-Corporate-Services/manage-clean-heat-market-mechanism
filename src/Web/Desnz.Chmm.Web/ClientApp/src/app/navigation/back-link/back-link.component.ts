import { Component } from '@angular/core';
import { BackLinkProvider } from './back-link.provider';
import { NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'back-link',
  template: `<a
    *ngIf="provider.link"
    class="govuk-back-link"
    routerLink="{{ provider.link }}"
    [queryParams]="provider.queryParams"
    >{{ provider.linkText }}</a
  >`,
  standalone: true,
  imports: [NgIf, RouterLink],
})
export class BackLinkComponent {
  constructor(public provider: BackLinkProvider) {}
}
