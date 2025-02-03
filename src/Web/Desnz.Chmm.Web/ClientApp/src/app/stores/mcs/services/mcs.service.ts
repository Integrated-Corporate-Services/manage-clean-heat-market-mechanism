import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as moment from 'moment';
import { Observable, map, of } from 'rxjs';
import { InstallationRequestSummaryDto } from '../dto/installation-request-summary.dto';

@Injectable()
export class McsService {
  private readonly baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = '/api/mcssynchronisation/';
  }

  getMcsDownloads(schemeYearId: string | null): Observable<InstallationRequestSummaryDto[]> {
    let url = `${this.baseUrl}/data/requests/`;
    if(schemeYearId){
      url = `${this.baseUrl}/data/requests/year/${schemeYearId}`;
    }
    return this.httpClient
      .get<InstallationRequestSummaryDto[]>(url, {
        responseType: 'json',
      })
      .pipe(
        map((installationRequestSummaries) => {
          return installationRequestSummaries.map((summary) => {
            const startDate = moment(summary.startDate).format('DD MMMM YYYY');
            const endDate = moment(summary.endDate).format('DD MMMM YYYY');
            return { id: summary.id, startDate: startDate, endDate: endDate };
          });
        })
      );
  }
}
