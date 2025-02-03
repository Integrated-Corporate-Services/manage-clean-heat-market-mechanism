import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs';
import { selectManufacturerUser, selectManufacturerUserBeingEdited } from '../../../stores/account-management/selectors';
import { clearManufacturerUserBeingEdited, getManufacturerUser, checkAnswersManufacturerUserBeingEdited } from '../../../stores/account-management/actions';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { FormsModule } from '@angular/forms';
import { EditManufacturerUserCommand } from '../../../stores/account-management/commands/edit-manufacturer-user-command';
import { ChmmUser } from '../../../stores/account-management/dtos/chmm-user';
import { FormControl, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

@Component({
  selector: 'manufacturer-edit-user',
  templateUrl: './manufacturer-edit-user.component.html',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor,
    AsyncPipe,
    ReactiveFormsModule
  ]
})
export class ManufacturerEditUserComponent implements OnInit, OnDestroy {

  @Input({ required: true }) organisationId?: string;
  @Input({ required: true }) userId?: string;

  errors: any = {};
  user: ChmmUser | undefined | null = null;
  command: EditManufacturerUserCommand = {
    email: '',
    jobTitle: '',
    name: '',
    organisationId: '',
    telephoneNumber: '',
    id: ''
  };

  form: FormGroup;

  UserSubscription: Subscription | null = null;
  EditUserSubscription: Subscription | null = null;

  constructor(
    private store: Store,
    private router: Router,
    private backLinkProvider: BackLinkProvider) {

      this.form = new FormGroup({
        name: new FormControl('', {
          nonNullable: true,
          validators: Validators.required,
        }),
        jobTitle: new FormControl('',{
          nonNullable: true,
          validators: Validators.required,
        }),
        telephoneNumber: new FormControl('',{
          nonNullable: true,
          validators: [Validators.required, Validators.maxLength(100), Validators.minLength(10), Validators.pattern('^[0-9]*$')]
        }),
        email: new FormControl('',{
        })
      });
  }

  ngOnInit() {
    this.backLinkProvider.clear();
    this.command.organisationId = this.organisationId!;
    this.command.id = this.userId!;

    this.UserSubscription = this.store.select(selectManufacturerUser).subscribe(user => {
      if(!user) return;

      this.user = user.data;
    });

    this.EditUserSubscription = this.store.select(selectManufacturerUserBeingEdited).subscribe(user => {
      if (!user) return;

      this.form.patchValue({
        name: user.name,
        jobTitle: user.jobTitle,
        telephoneNumber: user.telephoneNumber,
        email:user.email
      });
    });

    this.store.dispatch(getManufacturerUser({
      organisationId: this.organisationId!,
      userId: this.userId!
    }));
  }

  setValidationErrorMessages() {
    Object.keys(this.form.controls).forEach((key) => {
      const validationErrors = this.form.get(key)?.errors;
      if (validationErrors) {
        if (validationErrors['required']) {
          switch (key) {
            case 'name':
              this.errors[key] = 'Enter full name';
              break;
            case 'jobTitle':
              this.errors[key] = 'Enter job title';
              break;
            case 'telephoneNumber':
              this.errors[key] = 'Enter telephone number';
              break;
          }
        }
        else if (validationErrors['maxlength']) {
          this.errors[key] = 'Telephone number must be a maximum of 100 characters';
        }
        else if (validationErrors['minlength']) {
          this.errors[key] = 'Telephone number must be a minimum of 10 characters';
        }
        else if (validationErrors['pattern']) {
          this.errors[key] = 'Enter a valid telephone number';
        }
      }
    });
  }

  onSubmit() {
    this.errors = {};
    if (this.form.valid) {
      const updatedCommand: EditManufacturerUserCommand = { 
        ...this.command, 
        ...this.form.getRawValue() 
      };
      this.store.dispatch(
        checkAnswersManufacturerUserBeingEdited({
          command: updatedCommand
        })
      );
      this.router.navigateByUrl(`/organisation/${this.organisationId}/users/${this.userId}/check-answers`);
    } else {
      this.setValidationErrorMessages();
    }
  }

  ngOnDestroy() {
    if (this.EditUserSubscription) this.EditUserSubscription.unsubscribe();
    if (this.UserSubscription) this.UserSubscription.unsubscribe();
  }

  deactivate() {
    this.router.navigateByUrl(`/organisation/${this.organisationId}/users/${this.userId}/deactivate`);
  }

  cancel() {
    this.store.dispatch(clearManufacturerUserBeingEdited());
    this.router.navigateByUrl(`/organisation/${this.organisationId}/users/${this.userId}/view`);
  }
}
