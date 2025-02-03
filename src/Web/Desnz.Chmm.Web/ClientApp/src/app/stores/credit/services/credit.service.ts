import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreditBalanceDto } from '../dtos/credit-balance.dto';
import { CreditTransfer } from '../../../manufacturer/credits/models/credit-transfer';
import { CreditAmendment } from '../../../admin/manufacturers/amend-credit/models/amend-credit.model';
import { AmendCreditCommand } from '../commands/amend-credit-command';
import { CreditLedgerSummaryDto } from '../../organisation-summary/dtos/credit-ledger-summary.dto';
import { TransferHistoryDto } from '../dtos/transfer-history.dto';
import { PeriodCreditTotalsDto } from '../../heat-pumps/dtos/period-credit-totals.dto';

@Injectable()
export class CreditService {
  private readonly baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = '/api/creditledger';
  }

  getCreditLedgerSummary(
    organisationId: string,
    schemeYearId: string
  ): Observable<CreditLedgerSummaryDto> {
    return this.httpClient.get<CreditLedgerSummaryDto>(
      `${this.baseUrl}/manufacturer/${organisationId}/year/${schemeYearId}/summary`,
      {
        responseType: 'json',
      }
    );
  }

  submitCreditAmendment(
    organisationId: string,
    creditAmendment: CreditAmendment,
    schemeYearId: string
  ) {
    const command: AmendCreditCommand = {
      organisationId: organisationId,
      schemeYearId: schemeYearId,
      value: this.GetCreditAmendmentValue(creditAmendment),
    };
    return this.httpClient.post<string>(
      `${this.baseUrl}/adjust-credits`,
      command,
      {
        responseType: 'json',
      }
    );
  }

  getCreditBalance(
    organisationId: string,
    schemeYearId: string
  ): Observable<CreditBalanceDto> {
    return this.httpClient.get<CreditBalanceDto>(
      `${this.baseUrl}/manufacturer/${organisationId}/year/${schemeYearId}/credit-balance`,
      {
        responseType: 'json',
      }
    );
  }

  submitCreditTransfer(
    organisationId: string,
    creditTransfer: CreditTransfer,
    schemeYearId: string
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/transfer-credits`,
      {
        organisationId: organisationId,
        destinationOrganisationId: creditTransfer.organisationId,
        schemeYearId: schemeYearId,
        value: creditTransfer.noOfCredits,
      },
      {
        responseType: 'json',
      }
    );
  }

  getCreditTransferHistory(
    organisationId: string,
    schemeYearId: string
  ): Observable<TransferHistoryDto> {
    return this.httpClient.get<TransferHistoryDto>(
      `${this.baseUrl}/manufacturer/${organisationId}/year/${schemeYearId}/transfers`,
      {
        responseType: 'json',
      }
    );
  }
  
  getHeatPumpInstallations(organisationId: string, schemeYearId: string): Observable<PeriodCreditTotalsDto[]> {
    let url = `${this.baseUrl}/manufacturer/${organisationId}/year/${schemeYearId}/credit-totals`;    
    return this.httpClient.get<PeriodCreditTotalsDto[]>(url, {responseType: 'json'});
  }

  private GetCreditAmendmentValue(creditAmendment: CreditAmendment): number {
    const value = Number(creditAmendment.amount);
    return creditAmendment.addingOrRemoving === 'Adding' ? value : -value;
  }
}
