import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ManufacturerNote } from '../dtos/manufacturer-note';
import { addFilesToFormData } from '../../../shared/file-utils';
import { AddManufacturerNoteCommand } from '../commands/add-manufacturer-note-command';

@Injectable()
export class NotesService {
  private readonly _baseUrl: string;

  constructor(private _httpClient: HttpClient) {
    this._baseUrl = '/api/notes';
  }

  getNotes(organisationId: string, schemeYearId: string): Observable<ManufacturerNote[]> {
    return this._httpClient.get<ManufacturerNote[]>(`${this._baseUrl}/manufacturer/${organisationId}/year/${schemeYearId}/notes`, {
      responseType: 'json',
    });
  }

  getExistingNoteFileNames(organisationId: string, schemeYearId: string, noteId: string): Observable<string[]> {
    return this._httpClient.get<string[]>(`${this._baseUrl}/manufacturer/${organisationId}/year/${schemeYearId}/note/${noteId}/file-names`, {
      responseType: 'json',
    });
  }

  getNewNoteFileNames(organisationId: string, schemeYearId: string): Observable<string[]> {
    return this._httpClient.get<string[]>(`${this._baseUrl}/manufacturer/${organisationId}/year/${schemeYearId}/new-note/file-names`, {
      responseType: 'json',
    });
  }

  uploadNoteFile(files: FileList, organisationId: string, schemeYearId: string): Observable<void> {
    let formData = addFilesToFormData(files, 'files');
    return this._httpClient.post<void>(
      `${this._baseUrl}/manufacturer/${organisationId}/year/${schemeYearId}/new-note/file`,
      formData, { responseType: 'json' });
  }

  deleteNoteFile(fileName: string, organisationId: string, schemeYearId: string): Observable<void> {
    return this._httpClient.post<void>(
      `${this._baseUrl}/manufacturer/${organisationId}/year/${schemeYearId}/new-note/file/delete`,
      { fileName: fileName }, { responseType: 'json' });
  }

  submit(organisationId: string, schemeYearId: string, details: string) {
    return this._httpClient.post<void>(
      `${this._baseUrl}/manufacturer/note`,
      { organisationId, schemeYearId, details }, { responseType: 'json' });
  }

  clearManufacturerNewNoteFiles(organisationId: string, schemeYearId: string) {
    return this._httpClient.post<void>(`${this._baseUrl}/manufacturer/${organisationId}/year/${schemeYearId}/new-note/clear-files`, {
      responseType: 'json'
    });
  }
}
