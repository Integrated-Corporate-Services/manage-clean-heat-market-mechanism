import { NgFor, NgIf } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { getFileNames, validateFiles } from 'src/app/shared/file-utils';

@Component({
  selector: 'govuk-file-upload',
  standalone: true,
  imports: [FormsModule, NgIf, NgFor],
  templateUrl: './govuk-file-upload.component.html',
  styleUrls: ['./govuk-file-upload.component.css'],
})
export class GovukFileUploadComponent implements OnInit {
  @Input() id: string = 'files';
  @Input() name: string = 'files';
  @Input() label: string = 'Upload files';
  @Input() hint: string | null = null;
  @Input() uploadingFiles: boolean = false;
  @Input() retrievingFiles: boolean = false;
  @Input() deletingFile: boolean = false;
  @Input() error?: string | null = null;
  @Input() set fileNames(value: string[] | null) {
    if (value) {
      this._fileNames = value;
    }
  }
  get fileNames(): string[] {
    return this._fileNames;
  }
  _fileNames: string[] = [];

  @Output() upload = new EventEmitter<FileList>();
  @Output() delete = new EventEmitter<string>();

  ngOnInit() {}

  onFileChange(event: Event) {
    let fileInput = event.currentTarget as HTMLInputElement;
    let files = fileInput.files;

    this.error = validateFiles(files);
    if (!this.error && files !== null) {
      this.upload.emit(files);
      fileInput.value = '';
    }
  }

  onRemoveFile(name: string) {
    this.delete.emit(name);
  }
}
