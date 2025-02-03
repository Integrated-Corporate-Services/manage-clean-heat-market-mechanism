import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';

import { SessionStorageService } from '../shared/services/session-storage.service';

export const dateTimeInterceptor: HttpInterceptorFn = (req, next) => {
  const sessionStorageService = inject(SessionStorageService);
  const sessionValue =
    sessionStorageService.getObject<string>('dateTimeOverride');
  if (sessionValue) {
    const modifiedReq = req.clone({
      setHeaders: {
        'date-time-override': sessionValue,
      },
    });
    return next(modifiedReq);
  } else {
    return next(req);
  }
};
