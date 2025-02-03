import { Component, OnInit } from '@angular/core';
import { NgClass, NgIf, AsyncPipe } from '@angular/common';
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive,
} from '@angular/router';
import { EMPTY, Subscription, catchError, map, of, tap } from 'rxjs';
import { AuthenticationService } from '../stores/auth/services/authentication.service';
import { isAdmin, isManufacturer } from '../shared/auth-utils';
import { SessionStorageService } from '../shared/services/session-storage.service';

@Component({
  selector: 'view-manage-chmm',
  templateUrl: './view-manage-chmm.component.html',
  styleUrls: ['./view-manage-chmm.component.scss'],
  standalone: true,
  imports: [RouterLink, NgClass, RouterLinkActive, NgIf, AsyncPipe],
})
export class ViewManageChmmComponent implements OnInit {
  showPage: boolean = false;
  queryParamsSubscription: Subscription = Subscription.EMPTY;
  whoAmISubscription: Subscription = Subscription.EMPTY;

  constructor(
    private authenticationService: AuthenticationService,
    private route: ActivatedRoute,
    private router: Router,
    private sessionStorageService: SessionStorageService
  ) {}

  ngOnInit() {
    // Check if user is already signed in
    this.whoAmISubscription = this.authenticationService
      .getWhoAmI()
      .pipe(
        tap((whoAmI) => {
          // User is not signed in
          if (whoAmI === null || !whoAmI.email) {
            this.showPage = true;
            return;
          }

          if (whoAmI.status === 'Inactive') {
            this.router.navigateByUrl('/account-inactive');
            return;
          } else if (whoAmI.status === 'Active') {
            // Active manufacturer user
            if (whoAmI.organisationId && isManufacturer(whoAmI)) {
              this.router.navigateByUrl(`/scheme-year`);
              return;
            }

            // Active admin user
            if (isAdmin(whoAmI)) {
              this.router.navigateByUrl('/admin/organisations');
              return;
            }
          }

          // OneLogin email does not match a user in CHMM
          this.router.navigateByUrl('/register-organisation');
        }),
        catchError((error) => {
          this.showPage = true;
          return EMPTY;
        })
      )
      .subscribe();

    // Check if the query parameter exists
    this.queryParamsSubscription = this.route.queryParams.subscribe(
      (params) => {
        const queryStringValue = params['dateTimeOverride'];

        // Check if the query string parameter is present
        if (queryStringValue) {
          // Set the value in session storage
          this.sessionStorageService.setObject<string>(
            'dateTimeOverride',
            queryStringValue
          );
        }
      }
    );
  }

  ngOnDestroy() {
    if (this.queryParamsSubscription) {
      this.queryParamsSubscription.unsubscribe();
    }
    if (this.whoAmISubscription) {
      this.whoAmISubscription.unsubscribe();
    }
  }
}
