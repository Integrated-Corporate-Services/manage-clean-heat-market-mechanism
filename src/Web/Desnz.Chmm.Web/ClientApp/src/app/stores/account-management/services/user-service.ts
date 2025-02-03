import { HttpClient, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { InviteAdminUserCommand } from '../commands/invite-admin-user-command';
import { EditUserCommand } from '../commands/edit-user-command';
import { ChmmUser } from 'src/app/stores/account-management/dtos/chmm-user';
import { ActivateAdminUserCommand } from '../commands/activate-admin-user-command';
import { DeactivateAdminUserCommand } from '../commands/deactivate-admin-user-command';
import { InviteManufacturerUserCommand } from '../commands/invite-manufacturer-user-command';
import { EditManufacturerUserCommand } from '../commands/edit-manufacturer-user-command';

@Injectable()
export class UserService {
  private readonly _baseUrl: string;

  constructor(private _httpClient: HttpClient) {
    this._baseUrl = '/api/identity/users';
  }

  getAllUsers(): Observable<ChmmUser[]> {
    return this._httpClient.get<ChmmUser[]>(`${this._baseUrl}/all`, {
      responseType: 'json',
    });
  }

  getAdminUsers(): Observable<ChmmUser[]> {
    return this._httpClient.get<ChmmUser[]>(`${this._baseUrl}/admin`, {
      responseType: 'json',
    });
  }

  getManufacturerUsers(organisationId?: string | null): Observable<ChmmUser[]> {
    return this._httpClient.get<ChmmUser[]>(
      `${this._baseUrl}/manufacturer/${organisationId}`,
      {
        responseType: 'json',
      }
    );
  }

  getAdminUser(userId: string): Observable<ChmmUser> {
    return this._httpClient.get<ChmmUser>(`${this._baseUrl}/admin/${userId}`, {
      responseType: 'json',
    });
  }

  inviteAdminUser(command: InviteAdminUserCommand): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/admin`, command, {
      responseType: 'json',
    });
  }

  editAdminUser(command: EditUserCommand): Observable<void> {
    return this._httpClient.put<void>(`${this._baseUrl}/admin`, command, {
      responseType: 'json',
    });
  }

  editManufacturerUser(command: EditManufacturerUserCommand | null): Observable<void> {
    return this._httpClient.put<void>(`${this._baseUrl}/manufacturer`, command, {
      responseType: 'json',
    });
  }

  activateAdminUser(command: ActivateAdminUserCommand): Observable<void> {
    return this._httpClient.put<void>(
      `${this._baseUrl}/admin/activate`,
      command,
      {
        responseType: 'json',
      }
    );
  }

  deactivateAdminUser(command: DeactivateAdminUserCommand): Observable<void> {
    return this._httpClient.put<void>(
      `${this._baseUrl}/admin/deactivate`,
      command,
      {
        responseType: 'json',
      }
    );
  }

  inviteManufacturerUser(command: InviteManufacturerUserCommand): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/manufacturer`, command, {
      responseType: 'json',
    });
  }

  getManufacturerUser(organisationId: string, userId: string): Observable<ChmmUser> {
    return this._httpClient.get<ChmmUser>(`${this._baseUrl}/manufacturer/${organisationId}/users/${userId}`, {
      responseType: 'json',
    });
  }

  deactivateManufacturerUser(userId: string, organisationId: string): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/manufacturer/deactivate`, { userId, organisationId }, {
      responseType: 'json',
    });
  }
}
