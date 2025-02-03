const supportedMimeTypes = [
  'application/msword',
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
  'application/vnd.ms-powerpoint',
  'application/vnd.openxmlformats-officedocument.presentationml.presentation',
  'application/vnd.ms-excel',
  'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  'application/pdf',
  'image/jpeg',
  'image/png',
  'image/bmp',
  'text/csv',
  'message/rfc822',
];
const supportedFileExtensions = ['msg'];
const maxSize = 5242880;

export function addFilesToFormData(
  files: FileList | null,
  fieldName: string,
  formData = new FormData()
) {
  if (files !== null) {
    Object.keys(files).forEach((key, idx) => {
      formData.append(fieldName, files[idx]);
    });
  }
  return formData;
}

export function getFileNames(files: FileList | null) {
  let names = [];
  if (files !== null) {
    for (let idx = 0; idx < files.length; idx++) {
      names.push(files[idx].name);
    }
  }
  return names;
}

export function setFileValidationErrors(
  files: FileList | null,
  errors: { [key: string]: string },
  fileControlName: string = 'files'
) {
  if (files !== null) {
    for (let idx = 0; idx < files.length; idx++) {
      let file = files[idx];
      const fileExtension = file.name.split('.').pop();
      validateFileType(file.type, fileExtension!, errors, fileControlName);
      validateFileSize(file.size, errors, fileControlName);
    }
  }
}

function validateFileType(
  type: string,
  extension: string,
  errors: { [key: string]: string },
  fileControlName: string = 'files'
) {
  if (
    !(
      supportedMimeTypes.includes(type) ||
      supportedFileExtensions.includes(extension!)
    )
  ) {
    errors[fileControlName] =
      'The selected file must be a .doc, .docx, .pdf, .ppt, .pptx, .xls, .csv, .xlsx, .jpg, .png, .bmp, .eml, or .msg';
  }
}

function validateFileSize(
  size: number,
  errors: { [key: string]: string },
  fileControlName: string = 'files'
) {
  if (size > maxSize) {
    errors[fileControlName] = 'The selected file must be smaller than 5MB';
  }
}

export function validateFiles(files: FileList | null): string | null {
  if (files !== null) {
    for (let idx = 0; idx < files.length; idx++) {
      const file = files[idx];
      const extension = file.name.split('.').pop();
      if (
        !(
          supportedMimeTypes.includes(file.type) ||
          supportedFileExtensions.includes(extension!)
        )
      ) {
        return `The selected file: ${file.name} must be a .doc, .docx, .pdf, .ppt, .pptx, .xls, .csv, .xlsx, .jpg, .png, .bmp, .eml, or .msg'`;
      }
      if (file.size > maxSize) {
        return `The selected file: ${file.name} must be smaller than 5MB`;
      }
    }
  }
  return null;
}
