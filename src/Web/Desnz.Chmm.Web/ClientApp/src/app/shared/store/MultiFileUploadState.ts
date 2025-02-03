export interface MultiFileUploadState {
  files: FileList | null;
  fileNames: string[];
  uploadingFiles: boolean;
  retrievingFiles: boolean;
  deletingFile: boolean;
  error?: string | null;
  // TODO: Temporary as the AccountManagementState -> manufacturerNewNoteFiles state needs to be refactored
  loading?: boolean | null;
}
