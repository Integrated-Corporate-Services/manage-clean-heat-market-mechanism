export const hasErrors = (errors: { [key: string]: string | null }) =>
  !Object.keys(errors).every((key) => errors[key] == null);
