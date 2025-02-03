import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ApproveManufacturerOrganisationCommand } from '../commands/approve-manufacture-organisation-command';
import { ViewOrganisationDto } from '../dtos/view-organisation-dto';
import { GetEditableOrganisationDto } from '../dtos/get-editable-organisation-dto';
import { OnboardManufacturerCommand } from '../commands/onboard-manufacturer-command';
import { addFilesToFormData } from 'src/app/shared/file-utils';
import { OrganisationApprovalCommentsDto } from '../../administrator-approval/dtos/organisation-approval-comments.dto';
import { EditManufacturerOrganisationCommand } from '../commands/edit-manufacture-organisation-command';
import { EditAccountArea } from '../actions';
import { EditOrganisationSchemeParticipationCommand } from '../commands/edit-organisation-scheme-participation.command';
import { ContactOrganisationDto } from '../dtos/contact-organisation-dto';
import { RejectManufacturerOrganisationCommand } from '../commands/reject-manufacture-organisation-command';
import { OrganisationRejectionCommentsDto } from '../../administrator-rejection/dtos/organisation-rejection-comments.dto';

@Injectable()
export class OrganisationService {
  private readonly _baseUrl: string;

  constructor(private _httpClient: HttpClient) {
    this._baseUrl = '/api/identity/organisations';
  }

  getAvailableForTransfer(
    organisationId: string
  ): Observable<ViewOrganisationDto[]> {
    return this._httpClient.get<ViewOrganisationDto[]>(
      `${this._baseUrl}/${organisationId}/available-for-transfer`,
      {
        responseType: 'json',
      }
    );
  }

  getContactableOrganisations(
    organisationId: string
  ): Observable<ContactOrganisationDto[]> {
    return this._httpClient.get<ContactOrganisationDto[]>(
      `${this._baseUrl}/${organisationId}/contactable`,
      {
        responseType: 'json',
      }
    );
  }

  getManufacturers(): Observable<ViewOrganisationDto[]> {
    return this._httpClient.get<ViewOrganisationDto[]>(`${this._baseUrl}`, {
      responseType: 'json',
    });
  }

  getManufacturer(orgId: string): Observable<GetEditableOrganisationDto> {
    return this._httpClient.get<GetEditableOrganisationDto>(
      `${this._baseUrl}/${orgId}`,
      {
        responseType: 'json',
      }
    );
  }

  saveOnboardingDetails(command: OnboardManufacturerCommand): Observable<void> {
    let formData = addFilesToFormData(
      command.organisationStructureFiles,
      'files'
    );
    formData.append(
      'organisationDetailsJson',
      JSON.stringify(command.organisationDetails)
    );
    return this._httpClient.post<void>(`${this._baseUrl}/onboard`, formData, {
      responseType: 'json',
    });
  }

  getAccountApprovalFiles(organisationId: string): Observable<string[]> {
    return this._httpClient.get<string[]>(`${this._baseUrl}/${organisationId}/approval-files`);
  }

  uploadAccountApprovalFiles(files: FileList, organisationId: string): Observable<void> {
    let formData = addFilesToFormData(files, 'files');
    return this._httpClient.post<void>(
      `${this._baseUrl}/${organisationId}/approval-files/upload`,
      formData, { responseType: 'json' });
  }

  deleteAccountApprovalFile(fileName: string, organisationId: string): Observable<void> {
    let formData = new FormData();
    formData.append('fileName', fileName);
    return this._httpClient.post<void>(
      `${this._baseUrl}/${organisationId}/approval-files/delete`,
      formData, { responseType: 'json' });
  }

  approveAccount(
    command: ApproveManufacturerOrganisationCommand
  ): Observable<void> {
    let formData = new FormData();
    formData.append('organisationId', command.organisationId);
    formData.append('comment', command.comment);

    return this._httpClient.put<void>(`${this._baseUrl}/approve`, formData, {
      responseType: 'json',
    });
  }
  
  getAccountRejectionFiles(organisationId: string): Observable<string[]> {
    return this._httpClient.get<string[]>(`${this._baseUrl}/${organisationId}/rejection-files`);
  }

  uploadAccountRejectionFiles(files: FileList, organisationId: string): Observable<void> {
    let formData = addFilesToFormData(files, 'files');
    return this._httpClient.post<void>(
      `${this._baseUrl}/${organisationId}/rejection-files/upload`,
      formData, { responseType: 'json' });
  }

  deleteAccountRejectionFile(fileName: string, organisationId: string): Observable<void> {
    let formData = new FormData();
    formData.append('fileName', fileName);
    return this._httpClient.post<void>(
      `${this._baseUrl}/${organisationId}/rejection-files/delete`,
      formData, { responseType: 'json' });
  }

  rejectAccount(
    command: RejectManufacturerOrganisationCommand
  ): Observable<void> {
    let formData = new FormData();
    formData.append('organisationId', command.organisationId);
    formData.append('comment', command.comment);

    return this._httpClient.put<void>(`${this._baseUrl}/reject`, formData, {
      responseType: 'json',
    });
  }

  editAccount(
    area: EditAccountArea,
    dto: EditManufacturerOrganisationCommand
  ): Observable<void> {
    let formData = new FormData();
    formData.append('organisationId', dto.organisationDetails.id || '');

    switch (area) {
      case '*':
        return this.editAccountAll(dto, formData);
      case 'IsOnBehalfOfGroup':
        return this.editAccountIsOnBehalfOfGroup(dto, formData);
      case 'ResponsibleUndertaking':
        return this.editAccountResponsibleUndertaking(dto, formData);
      case 'Address':
        return this.editAccountRegisteredOfficeAddress(dto, formData);
      case 'LegalCorrespondenceAddress':
        return this.editAccountLegalCorrespondenceAddress(dto);
      case 'IsNonSchemeParticipant':
        return this.editAccountLegalCorrespondenceAddress(dto);
      case 'IsFossilFuelBoilerSeller':
        return this.editAccountIsFossilFuelSeller(dto, formData);
      case 'HeatPumpBrands':
        return this.editAccountHeatPumpBrands(dto, formData);
      case 'UserDetails':
        return this.editAccountUserDetails(dto, formData);
      case 'ResponsibleOfficer':
        return this.editAccountSeniorResponsibleOfficer(dto, formData);
      case 'ContactDetailsForCt':
        return this.editAccountContactDetailsForCt(dto, formData);
      default:
        return of();
    }
  }

  private editAccountAll(
    dto: EditManufacturerOrganisationCommand,
    formData: FormData
  ) {
    addFilesToFormData(
      dto.organisationStructureFiles,
      'organisationStructureFiles',
      formData
    );
    formData.append(
      'organisationDetailsJson',
      JSON.stringify(dto.organisationDetails)
    );
    return this._httpClient.put<void>(`${this._baseUrl}/edit`, formData, {
      responseType: 'json',
    });
  }

  private editAccountIsOnBehalfOfGroup(
    dto: EditManufacturerOrganisationCommand,
    formData: FormData
  ) {
    formData.append(
      'isOnBehalfOfGroup',
      `${dto.organisationDetails.isOnBehalfOfGroup}`
    );
    return this._httpClient.put<void>(
      `${this._baseUrl}/edit/structure`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  private editAccountResponsibleUndertaking(
    dto: EditManufacturerOrganisationCommand,
    formData: FormData
  ) {
    formData.append(
      'name',
      dto.organisationDetails.responsibleUndertaking?.name || ''
    );
    formData.append(
      'companiesHouseNumber',
      dto.organisationDetails.responsibleUndertaking?.companiesHouseNumber || ''
    );
    return this._httpClient.put<void>(
      `${this._baseUrl}/edit/details`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  private editAccountRegisteredOfficeAddress(
    dto: EditManufacturerOrganisationCommand,
    formData: FormData
  ) {
    let registeredAddress = dto.organisationDetails.addresses[0];
    formData.append('lineOne', registeredAddress.lineOne);
    formData.append('lineTwo', registeredAddress.lineTwo || '');
    formData.append('city', registeredAddress.city);
    formData.append('county', registeredAddress.county || '');
    formData.append('postcode', registeredAddress.postcode);
    return this._httpClient.put<void>(
      `${this._baseUrl}/edit/registered-office-address`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  private editAccountLegalCorrespondenceAddress(
    dto: EditManufacturerOrganisationCommand
  ) {
    let legalAddress = dto.organisationDetails.addresses[1];
    let legalAddressType = dto.organisationDetails.legalAddressType;

    return this._httpClient.put<void>(
      `${this._baseUrl}/edit/legal-correspondence-address`,
      legalAddressType === 'Use Specified Address'
        ? {
          organisationId: dto.organisationDetails.id,
          lineOne: legalAddress.lineOne,
          lineTwo: legalAddress.lineTwo,
          city: legalAddress.city,
          county: legalAddress.county,
          postcode: legalAddress.postcode,
          legalAddressType: legalAddressType,
        }
        : {
          organisationId: dto.organisationDetails.id,
          legalAddressType: legalAddressType,
        },
      {
        responseType: 'json',
      }
    );
  }

  private editAccountIsFossilFuelSeller(
    dto: EditManufacturerOrganisationCommand,
    formData: FormData
  ) {
    formData.append(
      'isFossilFuelBoilerSeller',
      `${dto.organisationDetails.isFossilFuelBoilerSeller}`
    );
    return this._httpClient.put<void>(
      `${this._baseUrl}/edit/fossil-fuel-seller`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  private editAccountHeatPumpBrands(
    dto: EditManufacturerOrganisationCommand,
    formData: FormData
  ) {
    for (let i = 0; i < dto.organisationDetails.heatPumpBrands.length; i++) {
      formData.append(
        `heatPumpBrands[${i}]`,
        dto.organisationDetails.heatPumpBrands[i]
      );
    }
    return this._httpClient.put<void>(
      `${this._baseUrl}/edit/heatpump-seller`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  private editAccountUserDetails(
    dto: EditManufacturerOrganisationCommand,
    formData: FormData
  ) {
    let applicant = dto.organisationDetails.users[0];
    formData.append('name', applicant.name);
    formData.append('jobTitle', applicant.jobTitle);
    formData.append('telephoneNumber', applicant.telephoneNumber);
    return this._httpClient.put<void>(
      `${this._baseUrl}/edit/applicant`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  private editAccountSeniorResponsibleOfficer(
    dto: EditManufacturerOrganisationCommand,
    formData: FormData
  ) {
    let sro = dto.organisationDetails.users[1];
    formData.append('name', sro.name);
    formData.append('jobTitle', sro.jobTitle);
    formData.append('telephoneNumber', sro.telephoneNumber);
    formData.append(
      'isApplicantSeniorResponsibleOfficer',
      `${sro.isResponsibleOfficer}`
    );
    return this._httpClient.put<void>(
      `${this._baseUrl}/edit/senior-responsible-officer`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  private editAccountContactDetailsForCt(
    dto: EditManufacturerOrganisationCommand,
    formData: FormData
  ) {
    let contact = dto.organisationDetails.creditContactDetails!;
    formData.append('name', contact.name || '');
    formData.append('email', contact.email || '');
    formData.append('telephoneNumber', contact.telephoneNumber || '');
    return this._httpClient.put<void>(
      `${this._baseUrl}/edit/credit-contact-details`,
      formData,
      {
        responseType: 'json',
      }
    );
  }

  getApprovalComments(
    organisationId: string
  ): Observable<OrganisationApprovalCommentsDto> {
    return this._httpClient.get<OrganisationApprovalCommentsDto>(
      `${this._baseUrl}/${organisationId}/approval-comments`,
      {
        responseType: 'json',
      }
    );
  }

  getRejectionComments(
    organisationId: string
  ): Observable<OrganisationRejectionCommentsDto> {
    return this._httpClient.get<OrganisationRejectionCommentsDto>(
      `${this._baseUrl}/${organisationId}/rejection-comments`,
      {
        responseType: 'json',
      }
    );
  }

  downloadApprovalCommentsFile(
    organisationId: string,
    fileName: string
  ): Observable<Blob> {
    return this._httpClient.get<Blob>(
      `${this._baseUrl}/${organisationId}/approval-comments/download`,
      {
        params: {
          fileName: fileName,
        },
        responseType: 'blob' as 'json',
      }
    );
  }

  editOrganisationSchemeParticipation(
    command: EditOrganisationSchemeParticipationCommand
  ) {
    return this._httpClient.put<void>(
      `${this._baseUrl}/edit/scheme-participation`,
      command,
      {
        responseType: 'json',
      }
    );
  }
}
