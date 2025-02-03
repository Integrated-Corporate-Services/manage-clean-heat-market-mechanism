import { ChmmRole } from './chmm-role';

export interface ChmmUser {
  id: string;
  name: string;
  email: string;
  telephoneNumber: string;
  status: string;
  chmmRoles: ChmmRole[];
  creationDate: string;
  createdBy: string;
  jobTitle?: string;
}
