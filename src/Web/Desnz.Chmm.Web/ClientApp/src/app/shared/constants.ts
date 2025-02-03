export const navigationOrganisationSessionKey: string = 'navigation_organisation';
export const schemeYearSessionKey: string = 'scheme_year';
export const previousSchemeYearSessionKey: string = 'prev_scheme_year';
export const whoAmISessionKey: string = 'whoAmI';

export const formConstants = {
  validation: {
    greaterThanZeroRegex: '^([1-9][0-9]?\\d*)$|^([0]{1})$',
    positiveNumberRegex: '^([1-9][0-9]?\\d*)$',
    positiveNumberOrHalfRegex: '^([0-9]+)(\\.[05])?$',
    positiveDecimalRegex: '^([0-9]+)(\\.[0-9]{1,3})?$',
    month: '^(1[0-2]|[1-9])$',
    day: '^(3[0-1]|[1-2][0-9]|[1-9])$',
  },
};

export const boilerSalesConstants = {
  statuses: ['N/A', 'Due', 'Submitted', 'Approved'],
};

export const numberFormat = new Intl.NumberFormat('en-GB', {
  style: 'decimal',
});
