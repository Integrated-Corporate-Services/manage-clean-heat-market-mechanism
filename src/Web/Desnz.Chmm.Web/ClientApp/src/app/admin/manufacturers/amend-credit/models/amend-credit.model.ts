import { AddingOrRemoving } from './adding-or-removing.type';

export interface CreditAmendment {
  addingOrRemoving: AddingOrRemoving;
  amount: string;
}
