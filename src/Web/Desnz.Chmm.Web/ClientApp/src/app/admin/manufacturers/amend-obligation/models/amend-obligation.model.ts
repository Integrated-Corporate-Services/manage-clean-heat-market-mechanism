import { AddingOrRemoving } from './adding-or-removing.type';

export interface ObligationAmendment {
  addingOrRemoving: AddingOrRemoving;
  amount: string;
}
