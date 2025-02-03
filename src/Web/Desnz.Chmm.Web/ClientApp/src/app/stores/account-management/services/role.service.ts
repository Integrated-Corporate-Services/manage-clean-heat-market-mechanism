import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ChmmRole } from '../dtos/chmm-role';

@Injectable()
export class RoleService {
  private readonly _baseUrl: string;

  constructor(private _httpClient: HttpClient) {
    this._baseUrl = '/api/identity/roles';
  }

  getAdminRoles(): Observable<ChmmRole[]> {
    return this._httpClient.get<ChmmRole[]>(`${this._baseUrl}/admin`, {
      responseType: 'json',
    });
  }
}
