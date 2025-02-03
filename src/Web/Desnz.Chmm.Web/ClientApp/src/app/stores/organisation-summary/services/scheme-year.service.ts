import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { SchemeYearSummaryConfigurationDto } from '../dtos/scheme-year-summary-configuration.dto';
import { SchemeYearDto } from '../dtos/scheme-year.dto';

@Injectable()
export class SchemeYearService {
  private readonly baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = '/api/configuration/schemeyear';
  }

  getSchemeYearSummaryConfiguration(
    schemeYearId: string
  ): Observable<SchemeYearSummaryConfigurationDto> {
    return this.httpClient.get<SchemeYearSummaryConfigurationDto>(
      `${this.baseUrl}/${schemeYearId}/summary`,
      {
        responseType: 'json',
      }
    );
  }

  getCurrentSchemeYear(): Observable<SchemeYearDto> {
    return this.httpClient.get<SchemeYearDto>(`${this.baseUrl}/current`, {
      responseType: 'json',
    });
  }

  getSchemeYear(schemeYearId: string): Observable<SchemeYearDto> {
    return this.httpClient.get<SchemeYearDto>(`${this.baseUrl}/${schemeYearId}`, {
      responseType: 'json',
    });
  }

  getFirstSchemeYear(): Observable<SchemeYearDto> {
    return this.httpClient.get<SchemeYearDto>(`${this.baseUrl}/first`, {
      responseType: 'json',
    });
  }

  getAllSchemeYears(): Observable<SchemeYearDto[]> {
    return this.httpClient.get<SchemeYearDto[]>(`${this.baseUrl}/all`, {
      responseType: 'json',
    });
  }
}
