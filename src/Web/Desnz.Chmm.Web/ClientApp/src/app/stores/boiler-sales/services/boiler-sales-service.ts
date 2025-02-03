import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { addFilesToFormData } from 'src/app/shared/file-utils';
import { AnnualBoilerSalesDto } from '../dtos/annual-boiler-sales';
import { QuarterlyBoilerSalesDto } from '../dtos/quarterly-boiler-sales';
import { SubmitAnnualBoilerSalesCommand } from '../commands/SubmitAnnualBoilerSalesCommand';
import { SubmitQuarterlyBoilerSalesCommand } from '../commands/SubmitQuarterlyBoilerSalesCommand';
import { BoilerSalesSummaryDto } from '../../organisation-summary/dtos/boiler-sales-summary.dto';

@Injectable()
export class BoilerSalesService {
  private readonly baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = '/api/boilersales';
  }

  getBoilerSalesSummary(
    organisationId: string,
    schemeYearId: string
  ): Observable<BoilerSalesSummaryDto> {
    return this.httpClient.get<BoilerSalesSummaryDto>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/summary`,
      {
        responseType: 'json',
      }
    );
  }

  getAnnualBoilerSalesSummary(
    organisationId: string,
    schemeYearId: string
  ): Observable<AnnualBoilerSalesDto> {
    return this.httpClient.get<AnnualBoilerSalesDto>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/annual`,
      {
        responseType: 'json',
      }
    );
  }

  approveAnnualBoilerSalesSummary(
    organisationId: string,
    schemeYearId: string
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/annual/approve`,
      {
        organisationId: organisationId,
        schemeYearId: schemeYearId,
      },
      {
        responseType: 'json',
      }
    );
  }

  getQuarterlyBoilerSalesSummary(
    organisationId: string,
    schemeYearId: string
  ): Observable<QuarterlyBoilerSalesDto[]> {
    return this.httpClient.get<QuarterlyBoilerSalesDto[]>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/quarters`,
      {
        responseType: 'json',
      }
    );
  }

  copyAnnualBoilerSalesFilesForEditing(
    organisationId: string,
    schemeYearId: string
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/annual/edit/copy-files`,
      { organisationId, schemeYearId },
      {
        responseType: 'json',
      }
    );
  }

  copyQuarterlyBoilerSalesFilesForEditing(
    organisationId: string,
    schemeYearId: string,
    schemeYearQuarterId: string
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/quarter/${schemeYearQuarterId}/edit/copy-files`,
      { organisationId, schemeYearId, schemeYearQuarterId },
      {
        responseType: 'json',
      }
    );
  }

  getAnnualVerificationStatementFileNames(
    organisationId: string,
    schemeYearId: string,
    isEditing: boolean
  ): Observable<string[]> {
    return this.httpClient.get<string[]>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/annual/verification-statement?isEditing=${isEditing}`,
      {
        responseType: 'json',
      }
    );
  }

  getAnnualSupportingEvidenceFileNames(
    organisationId: string,
    schemeYearId: string,
    isEditing: boolean
  ): Observable<string[]> {
    return this.httpClient.get<string[]>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/annual/supporting-evidence?isEditing=${isEditing}`,
      {
        responseType: 'json',
      }
    );
  }

  uploadAnnualVerificationStatement(
    verificationStatement: FileList,
    organisationId: string,
    schemeYearId: string,
    isEditing: boolean
  ): Observable<void> {
    let formData = addFilesToFormData(
      verificationStatement,
      'verificationStatement'
    );
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/annual/verification-statement?isEditing=${isEditing}`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  uploadAnnualSupportingEvidence(
    supportingEvidence: FileList,
    organisationId: string,
    schemeYearId: string,
    isEditing: boolean
  ): Observable<void> {
    let formData = addFilesToFormData(supportingEvidence, 'supportingEvidence');
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/annual/supporting-evidence?isEditing=${isEditing}`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  deleteAnnualVerificationStatement(
    fileName: string,
    organisationId: string,
    schemeYearId: string,
    isEditing: boolean
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/annual/verification-statement/delete?isEditing=${isEditing}`,
      { fileName: fileName },
      {
        responseType: 'json',
      }
    );
  }

  deleteAnnualSupportingEvidence(
    fileName: string,
    organisationId: string,
    schemeYearId: string,
    isEditing: boolean
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/annual/supporting-evidence/delete?isEditing=${isEditing}`,
      { fileName: fileName },
      {
        responseType: 'json',
      }
    );
  }

  submitAnnualBoilerSales(
    command: SubmitAnnualBoilerSalesCommand
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${command.organisationId}/year/${command.schemeYearId}/annual`,
      command,
      {
        responseType: 'json',
      }
    );
  }

  editAnnualBoilerSales(
    command: SubmitAnnualBoilerSalesCommand
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${command.organisationId}/year/${command.schemeYearId}/annual/edit`,
      command,
      {
        responseType: 'json',
      }
    );
  }

  getQuarterlySupportingEvidenceFileNames(
    organisationId: string,
    schemeYearQuarterId: string,
    schemeYearId: string,
    isEditing: boolean
  ): Observable<string[]> {
    return this.httpClient.get<string[]>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/quarter/${schemeYearQuarterId}/supporting-evidence?isEditing=${isEditing}`,
      {
        responseType: 'json',
      }
    );
  }

  uploadQuarterlySupportingEvidence(
    supportingEvidence: FileList,
    organisationId: string,
    schemeYearQuarterId: string,
    schemeYearId: string,
    isEditing: boolean
  ): Observable<void> {
    let formData = addFilesToFormData(supportingEvidence, 'supportingEvidence');
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/quarter/${schemeYearQuarterId}/supporting-evidence?isEditing=${isEditing}`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  deleteQuarterlySupportingEvidence(
    fileName: string,
    organisationId: string,
    schemeYearQuarterId: string,
    schemeYearId: string,
    isEditing: boolean
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/quarter/${schemeYearQuarterId}/supporting-evidence/delete?isEditing=${isEditing}`,
      { fileName: fileName },
      {
        responseType: 'json',
      }
    );
  }

  downloadQuarterlySupportingEvidence(
    fileName: string,
    organisationId: string,
    schemeYearQuarterId: string,
    schemeYearId: string
  ): Observable<Blob> {
    return this.httpClient.get<Blob>(
      `${this.baseUrl}/organisation/${organisationId}/year/${schemeYearId}/quarter/${schemeYearQuarterId}/supporting-evidence/download`,
      {
        params: {
          fileName: fileName,
        },
        responseType: 'blob' as 'json',
      }
    );
  }

  editQuarterlyBoilerSales(
    command: SubmitQuarterlyBoilerSalesCommand
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${command.organisationId}/year/${command.schemeYearId}/quarter/${command.schemeYearQuarterId}/edit`,
      command,
      {
        responseType: 'json',
      }
    );
  }

  submitQuarterlyBoilerSales(
    command: SubmitQuarterlyBoilerSalesCommand
  ): Observable<void> {
    return this.httpClient.post<void>(
      `${this.baseUrl}/organisation/${command.organisationId}/year/${command.schemeYearId}/quarter/${command.schemeYearQuarterId}`,
      command,
      {
        responseType: 'json',
      }
    );
  }
}
