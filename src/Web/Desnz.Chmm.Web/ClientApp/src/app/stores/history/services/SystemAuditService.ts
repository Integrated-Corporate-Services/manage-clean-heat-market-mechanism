import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { AuditItemDto } from '../dtos/AuditItemDto';

@Injectable()
export class SystemAuditService {
  private readonly baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = '/api/systemaudit';
  }

  getAuditItems(organisationId: string): Observable<AuditItemDto[]> {
    return this.httpClient.get<AuditItemDto[]>(
      `${this.baseUrl}/organisation/${organisationId}`,
      { responseType: 'json' }
    );
  }
}
