import { WhoAmI } from '../stores/auth/services/authentication.service';

export const renewalPeriod = 120000;

export const isManufacturer = (whoAmI: WhoAmI): boolean =>
  whoAmI.roles.includes('Manufacturer');

export const isAdmin = (whoAmI: WhoAmI | null): boolean =>
  whoAmI?.roles.some((role) =>
    [
      'Regulatory Officer',
      'Senior Technical Officer',
      'Principal Technical Officer',
    ].includes(role)
  ) ?? false;

export const hasRoles = (whoAmI: WhoAmI, roles: string[] | null): boolean =>
  roles === null ? true : whoAmI.roles.some((role) => roles.includes(role));

export const shouldRenew = (whoAmI: WhoAmI | null): boolean => {
  let shouldRenew = true;
  if (whoAmI !== null && whoAmI.exp !== null) {
    const expDate = Date.parse(whoAmI.exp);
    shouldRenew = Date.now() >= expDate;
  }
  return shouldRenew;
};
