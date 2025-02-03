import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ObligationSummaryDto } from '../../organisation-summary/dtos/obligation-summary.dto';
import { AmmendObligationCommand } from '../commands/ammend-obligation.command';
import { ObligationAmendment } from 'src/app/admin/manufacturers/amend-obligation/models/amend-obligation.model';
import { AdminAmendmentDto } from 'src/app/shared/dtos/admin-amendment.dto';

@Injectable()
export class ObligationService {
  private readonly baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = '/api/obligation';
  }

  getObligationSummary(
    organisationId: string,
    schemeYearId: string
  ): Observable<ObligationSummaryDto> {
    return this.httpClient.get<ObligationSummaryDto>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/summary`,
      { responseType: 'json' }
    );
  }

  getObligationAmendments(
    organisationId: string,
    schemeYearId: string
  ): Observable<{ [id: string]: AdminAmendmentDto }> {
    return this.httpClient.get<{ [id: string]: AdminAmendmentDto }>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/amendments`,
      {
        responseType: 'json',
      }
    );
  }

  submitObligationAmendment(
    organisationId: string,
    schemeYearId: string,
    obligationAmendment: ObligationAmendment
  ): Observable<string> {
    const command: AmmendObligationCommand = {
      organisationId: organisationId,
      schemeYearId: schemeYearId,
      value: this.GetObligationAmendmentValue(obligationAmendment),
    };
    return this.httpClient.post<string>(`${this.baseUrl}/amend`, command, {
      responseType: 'json',
    });
  }

  private GetObligationAmendmentValue(
    obligationAmendment: ObligationAmendment
  ): number {
    const value = Number(obligationAmendment.amount);
    return obligationAmendment.addingOrRemoving === 'Adding' ? value : -value;
  }
}
