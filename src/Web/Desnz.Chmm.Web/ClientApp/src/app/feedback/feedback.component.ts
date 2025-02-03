import { Component, OnInit } from '@angular/core';
import { NgClass, NgIf, AsyncPipe, Location } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';


interface FeedbackForm {
  feelingsTowardsService: FormControl<string>;
  serviceMeetsNeeds: FormControl<string>;
  serviceImprovements: FormControl<string | null>;
}

@Component({
  selector: 'feedback',
  templateUrl: './feedback.component.html',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, NgClass, RouterLinkActive, NgIf, AsyncPipe],
})
export class FeedbackComponent implements OnInit {

  readonly feelingsTowardsService = 'Overall, how do you feel about the service being provided to you?';
  readonly serviceMeetsNeeds = 'Does the service meet all of your needs in relation to the Clean Heat Market Mechanism?';
  readonly serviceImprovements = 'How could we improve this service?';
  readonly feedbackEmail = 'chmm@environment-agency.gov.uk';
  readonly feedbackEmailSubject = 'M-CHMM User submitted feedback';

  form: FormGroup<FeedbackForm>;
  errors: { [key: string]: string } = {};
  triedSubmitting: boolean = false;

  constructor(
    private _location: Location,
    private _router: Router) {
    this.form = new FormGroup({
      feelingsTowardsService: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required]
      }),
      serviceMeetsNeeds: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required]
      }),
      serviceImprovements: new FormControl('', {
        validators: [Validators.maxLength(1000)]
      })
    });
  }

  ngOnInit() {
    this.form.valueChanges.subscribe(_ => this.setValidationErrors());
  }

  previousPage() {
    this._location.back();
  }

  onSubmit() {
    this.triedSubmitting = true;
    if (this.form.valid) {
      window.open(this.prepareUrl());
      this._router.navigateByUrl('/');
    } else {
      this.setValidationErrors();
    }
  }

  private setValidationErrors() {
    this.errors = {};
    if (!(this.triedSubmitting && !this.form.valid)) return;
    Object.keys(this.form.controls).forEach((key) => {
      const validationErrors = this.form.get(key)?.errors;
      if (validationErrors) {
        if (validationErrors['required']) {
          if (key == 'feelingsTowardsService') {
            this.errors[key] = 'Select how you feel about the service';
          } else if (key == 'serviceMeetsNeeds') {
            this.errors[key] = 'Select whether this service meets your needs';
          }
        } else if (validationErrors['maxlength']) {
          this.errors[key] = 'Enter less than 1,000 characters';
        }
      }
    });
  }

  private prepareUrl() {
    let url: string[] = [];
    url.push(`mailto:${this.feedbackEmail}`);
    url.push(`?subject=${this.feedbackEmailSubject}&body=`);
    let newLines = (count: number) => url.push([...Array(count).keys()].map(_ => '%0A').join(''));
    let add = (heading: string, control: FormControl<string> | FormControl<string | null>) => {
      if (!control.value) return;
      url.push(encodeURIComponent(heading));
      newLines(1);
      url.push(encodeURIComponent(control.value));
    };
    add(this.feelingsTowardsService, this.form.controls.feelingsTowardsService);
    newLines(2);
    add(this.serviceMeetsNeeds, this.form.controls.serviceMeetsNeeds);
    newLines(2);
    add(this.serviceImprovements, this.form.controls.serviceImprovements);
    return url.join('');
  }
}
