import { Injectable } from '@angular/core';
import { InviteManufacturerUserCommand } from '../../../stores/account-management/commands/invite-manufacturer-user-command';
import { EMPTY_REGEXP, EMAIL_REGEXP, PHONE_REGEXP } from '../../../shared/validation';

export interface ManufacturerInviteUserValidatorResponse {
  valid: boolean;
  errors: any;
}

@Injectable({
  providedIn: 'root'
})
export class ManufacturerInviteUserValidator {

  validate(command: InviteManufacturerUserCommand): ManufacturerInviteUserValidatorResponse {
    let errors: any = {};

    // Special validators
    if (!EMAIL_REGEXP.test(command.email)) errors['email'] = 'Enter a valid email address';
    if (!PHONE_REGEXP.test(command.telephoneNumber)) errors['telephoneNumber'] = 'Enter a valid telephone number';

    // Minimum length
    if (command.telephoneNumber.length < 10) errors['telephoneNumber'] = 'Telephone number must be a minimum of 10 characters';

    // Maximum length
    if (command.name.length > 100) errors['name'] = 'Full name must be a maximum of 100 characters';
    if (command.jobTitle.length > 100) errors['jobTitle'] = 'Job title must be a maximum of 100 characters';
    if (command.telephoneNumber.length > 100) errors['telephoneNumber'] = 'Telephone number must be a maximum of 100 characters';
    if (command.email.length > 100) errors['email'] = 'Email address must be a maximum of 100 characters';

    // Required fields
    if (EMPTY_REGEXP.test(command.name)) errors['name'] = 'Enter full name';
    if (EMPTY_REGEXP.test(command.jobTitle)) errors['jobTitle'] = 'Enter job title';
    if (EMPTY_REGEXP.test(command.telephoneNumber)) errors['telephoneNumber'] = 'Enter telephone number';
    if (EMPTY_REGEXP.test(command.email)) errors['email'] = 'Enter email address';

    return {
      errors,
      valid: Object.keys(errors).length === 0
    };
  }
}
