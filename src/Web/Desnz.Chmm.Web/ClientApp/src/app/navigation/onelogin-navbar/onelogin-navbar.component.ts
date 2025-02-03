import { NgIf } from '@angular/common';
import { Component, Input } from '@angular/core';
import {
  navigationOrganisationSessionKey,
  schemeYearSessionKey,
  whoAmISessionKey,
} from 'src/app/shared/constants';
import { SessionStorageService } from 'src/app/shared/services/session-storage.service';

@Component({
  selector: 'onelogin-navbar',
  templateUrl: './onelogin-navbar.component.html',
  standalone: true,
  imports: [NgIf],
})
export class OneloginNavbarComponent {
  @Input({ required: true }) isAuthenticated: boolean | null = null;

  serviceName = 'Clean Heat Market Mechanism';
  changeCredentialsUrl = 'https://home.integration.account.gov.uk/settings';
  signOutUrl = 'oidc/logout';

  constructor(private sessionStorageService: SessionStorageService) {}

  onSignOut() {
    this.sessionStorageService.remove(navigationOrganisationSessionKey);
    this.sessionStorageService.remove(whoAmISessionKey);
    this.sessionStorageService.remove(schemeYearSessionKey);
  }
}
