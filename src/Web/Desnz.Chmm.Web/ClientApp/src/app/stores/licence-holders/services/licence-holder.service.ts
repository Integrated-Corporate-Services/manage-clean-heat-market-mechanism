import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LicenceHolderDto } from '../dto/licence-holder';
import { LinkLicenceHolderDto } from '../dto/link-licence-holder.dto';
import { LicenceHolderLinkDto } from '../dto/licence-holder-link.dto';
import { EditLinkedLicenceHolderDto } from '../dto/end-link-licence-holder-dto';

@Injectable()
export class LicenceHolderService {
  private readonly baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = '/api/identity/licenceholders';
  }

  getAllLicenceHolders(): Observable<LicenceHolderDto[]> {
    return this.httpClient.get<LicenceHolderDto[]>(`${this.baseUrl}/all`, {
      responseType: 'json',
    });
  }

  getLinkedLicenceHolders(
    organisationId: string
  ): Observable<LicenceHolderLinkDto[]> {
    return this.httpClient.get<LicenceHolderLinkDto[]>(
      `${this.baseUrl}/linked-to/${organisationId}/all`,
      { responseType: 'json' }
    );
  }

  getUnlinkedLicenceHolders(): Observable<LicenceHolderDto[]> {
    return this.httpClient.get<LicenceHolderDto[]>(`${this.baseUrl}/unlinked`, {
      responseType: 'json',
    });
  }

  linkLicenceHolder(
    licenceHolderId: string,
    organisationId: string,
    linkLicenceHolderDto: LinkLicenceHolderDto
  ) {
    return this.httpClient.post(
      `${this.baseUrl}/${licenceHolderId}/link-to/${organisationId}`,
      linkLicenceHolderDto,
      { responseType: 'json' }
    );
  }

  editLinkedLicenceHolder(
    licenceHolderId: string,
    editLinkedLicenseholderDto: EditLinkedLicenceHolderDto
  ) {
    return this.httpClient.post(
      `${this.baseUrl}/${licenceHolderId}/edit`,
      editLinkedLicenseholderDto,
      { responseType: 'json' }
    );
  }

  endLink(
    licenceHolderId: string,
    organisationId: string,
    editLinkedLicenceHolderDto: EditLinkedLicenceHolderDto
  ) {
    return this.httpClient.post(
      `${this.baseUrl}/${licenceHolderId}/endlink/${organisationId}`,
      editLinkedLicenceHolderDto,
      { responseType: 'json' }
    );
  }
}
