import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SchemeYearDto } from '../dtos/scheme-year.dto';
import { SchemeYearConfigurationDto } from '../dtos/scheme-year-configuration.dto';
import { UpdateSchemeYearConfigurationCommand } from '../commands/update-scheme-year-configuration.command';

@Injectable()
export class SchemeYearConfigurationService {
  private readonly baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = '/api/configuration';
  }

  getSchemeYears(): Observable<SchemeYearDto[]> {
    return this.httpClient.get<SchemeYearDto[]>(
      `${this.baseUrl}/schemeyear/all`,
      { responseType: 'json' }
    );
  }

  getSchemeYear(
    schemeYearId: string
  ): Observable<SchemeYearDto> {
    return this.httpClient.get<SchemeYearDto>(
      `${this.baseUrl}/schemeyear/${schemeYearId}`,
      { responseType: 'json' }
    );
  }

  getSchemeYearConfiguration(
    schemeYearId: string
  ): Observable<SchemeYearConfigurationDto> {
    return this.httpClient.get<SchemeYearConfigurationDto>(
      `${this.baseUrl}/obligationcalculations/${schemeYearId}`,
      { responseType: 'json' }
    );
  }

  updateSchemeYearConfiguration(
    command: UpdateSchemeYearConfigurationCommand
  ): Observable<void> {
    return this.httpClient.put<void>(
      `${this.baseUrl}/schemeyear/configuration`,
      command, { responseType: 'json' }
    );
  }
}
