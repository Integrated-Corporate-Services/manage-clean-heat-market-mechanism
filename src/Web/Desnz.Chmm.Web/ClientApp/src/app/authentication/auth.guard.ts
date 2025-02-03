import {
  ActivatedRouteSnapshot,
  CanActivateChildFn,
  CanActivateFn,
  RouterStateSnapshot,
} from '@angular/router';
import {
  AuthenticationService,
  WhoAmI,
} from '../stores/auth/services/authentication.service';
import { inject } from '@angular/core';
import { catchError, map, of } from 'rxjs';
import { Store } from '@ngrx/store';
import { onGetWhoAmI } from '../stores/auth/actions';
import { SessionStorageService } from '../shared/services/session-storage.service';
import { hasRoles, shouldRenew } from '../shared/auth-utils';

export function authGuard(
  roles: string[] | null = null
): CanActivateFn | CanActivateChildFn {
  return (ars: ActivatedRouteSnapshot, rss: RouterStateSnapshot) => {
    const store = inject(Store);
    const sessionStorageService = inject(SessionStorageService);
    const authenticationService = inject(AuthenticationService);

    const whoAmI = sessionStorageService.getObject<WhoAmI>('whoAmI');
    if (whoAmI === null || shouldRenew(whoAmI)) {
      return authenticationService.getWhoAmI().pipe(
        map((whoAmI) => {
          if (whoAmI !== null) {
            store.dispatch(onGetWhoAmI({ whoAmI }));
            sessionStorageService.setObject('whoAmI', whoAmI);
            return hasRoles(whoAmI, roles);
          }
          window.location.href = '/oidc/login';
          return false;
        }),
        catchError((error) => {
          window.location.href = '/oidc/login';
          return of(false);
        })
      );
    } else {
      store.dispatch(onGetWhoAmI({ whoAmI }));
      return hasRoles(whoAmI, roles);
    }
  };
}
