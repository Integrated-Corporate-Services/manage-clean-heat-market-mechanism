import { inject } from '@angular/core';
import {
  HttpInterceptorFn,
  HttpBackend,
  HttpClient,
} from '@angular/common/http';

import { EMPTY, catchError, switchMap } from 'rxjs';
import { Store } from '@ngrx/store';
import { WhoAmI } from '../stores/auth/services/authentication.service';
import { onGetWhoAmI } from '../stores/auth/actions';
import { SessionStorageService } from '../shared/services/session-storage.service';
import { shouldRenew } from '../shared/auth-utils';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const store = inject(Store);
  const sessionStorageService = inject(SessionStorageService);
  const httpBackend = inject(HttpBackend);
  const httpClient = new HttpClient(httpBackend);

  if (req.url.startsWith('oidc/login') || req.url.startsWith('oidc/whoami')) {
    return next(req);
  } else {
    const whoAmI = sessionStorageService.getObject<WhoAmI>('whoAmI');
    if (whoAmI === null || shouldRenew(whoAmI)) {
      return httpClient.get<WhoAmI>('oidc/whoami').pipe(
        switchMap((whoAmI) => {
          if (whoAmI !== null) {
            store.dispatch(onGetWhoAmI({ whoAmI }));
            sessionStorageService.setObject('whoAmI', whoAmI);
            return next(req);
          }
          window.location.href = '/oidc/login';
          return EMPTY;
        }),
        catchError((error) => {
          window.location.href = '/oidc/login';
          return EMPTY;
        })
      );
    } else {
      store.dispatch(onGetWhoAmI({ whoAmI }));
      return next(req);
    }
  }
};
