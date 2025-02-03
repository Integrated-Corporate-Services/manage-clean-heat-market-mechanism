import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface WhoAmI {
  email: string | null;
  roles: string[];
  status: string | null;
  organisationId: string | null;
  exp: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  constructor(private http: HttpClient) {}

  public getWhoAmI(): Observable<WhoAmI> {
    return this.http.get<WhoAmI>('oidc/whoami');
  }
}
