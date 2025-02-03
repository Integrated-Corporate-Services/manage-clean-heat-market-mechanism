import { JsonPipe, NgFor, NgIf, NgStyle } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { AuditItemRow } from 'src/app/stores/history/dtos/AuditItemDto';
import { EditOrganisationDto } from 'src/app/stores/onboarding/dtos/edit-organisation-dto';
import { isEqualWith } from 'lodash-es';

@Component({
  selector: 'onboarding-audit-item-value',
  templateUrl: './onboarding-audit-item-value.component.html',
  standalone: true,
  imports: [NgIf, NgFor, JsonPipe, NgStyle],
  styleUrls: ['./onboarding-audit-item-value.component.css'],
})
export class OnboardingAuditItemValueComponent implements OnInit {
  @Input({ required: true }) auditItemRows!: AuditItemRow[];
  organisationDetailsRows: {
    label: string;
    values: (string | undefined | null)[];
  }[] = [];

  constructor() {}

  ngOnInit() {
    const organisationDetailsJson: EditOrganisationDto =
      this.auditItemRows.find(
        (a) => a.label === 'Organisation Details Json'
      )?.objectValue;

    if (!organisationDetailsJson) {
      throw Error('organisationDetailsJson not found');
    }

    const address = organisationDetailsJson.addresses[0];
    const addressValues = [
      address.lineOne,
      address.lineTwo,
      address.city,
      address.county,
      address.postcode,
    ];

    const legalAddress = organisationDetailsJson.addresses[1];
    const legalAddressValues = [
      legalAddress.lineOne,
      legalAddress.lineTwo,
      legalAddress.city,
      legalAddress.county,
      legalAddress.postcode,
    ];

    let isUsedAsLegalCorrespondence = isEqualWith(
      legalAddressValues,
      address,
      (value, other, key) => {
        return key === 'id' || key === 'isUsedAsLegalCorrespondence'
          ? true
          : undefined;
      }
    );

    const applicant = organisationDetailsJson.users[0];
    const applicantValues = [
      applicant.name,
      applicant.jobTitle,
      applicant.organisation,
      applicant.responsibleOfficerOrganisationName,
      applicant.email,
      applicant.telephoneNumber,
    ];

    const responsibleOfficer = organisationDetailsJson.users[1];
    const responsibleOfficerValues = responsibleOfficer
      ? [
          responsibleOfficer.name,
          responsibleOfficer.jobTitle,
          responsibleOfficer.organisation,
          responsibleOfficer.responsibleOfficerOrganisationName,
          responsibleOfficer.email,
          responsibleOfficer.telephoneNumber,
        ]
      : [];

    const creditContactDetails = organisationDetailsJson.creditContactDetails;

    const hasOptedIn =
      creditContactDetails?.email ||
      creditContactDetails?.name ||
      creditContactDetails?.telephoneNumber;

    const creditContactDetailsValues = [
      creditContactDetails?.name,
      creditContactDetails?.email,
      creditContactDetails?.telephoneNumber,
    ];

    this.organisationDetailsRows = [
      {
        values: organisationDetailsJson.isOnBehalfOfGroup ? ['Yes'] : ['No'],
        label: 'Is this registration on behalf of a group of organisations?',
      },
      {
        values: [organisationDetailsJson.responsibleUndertaking?.name],
        label: 'Name of the organisation which is the Responsible Undertaking',
      },
      {
        values: organisationDetailsJson.responsibleUndertaking
          ?.companiesHouseNumber
          ? ['Yes']
          : ['No'],
        label:
          'Does the Responsible Undertaking have a Companies House number?',
      },
      {
        values: [
          organisationDetailsJson.responsibleUndertaking?.companiesHouseNumber,
        ],
        label: 'Companies House number',
      },
      {
        values: addressValues,
        label: 'Registered office address',
      },
      {
        values: isUsedAsLegalCorrespondence ? ['Yes'] : ['No'],
        label: 'Should this address be used for legal correspondence?',
      },
      {
        values: legalAddressValues,
        label: 'Address for legal correspondence',
      },
      {
        values: organisationDetailsJson.isFossilFuelBoilerSeller
          ? ['Yes']
          : ['No'],
        label: 'Do you sell relevant fossil fuel boilers?',
      },
      {
        values:
          organisationDetailsJson.heatPumpBrands.length > 0 ? ['Yes'] : ['No'],
        label: 'Do you sell heat pumps?',
      },
      {
        values: organisationDetailsJson.heatPumpBrands,
        label: 'Heat pump brands',
      },
      {
        values: applicantValues,
        label: 'Your details',
      },
      {
        values: applicant.isResponsibleOfficer ? ['Yes'] : ['No'],
        label: 'Are you the Senior Responsible Officer for your organisation?',
      },
      {
        values: applicant.isResponsibleOfficer
          ? applicantValues
          : responsibleOfficerValues,
        label:
          'Details of the Senior Responsible Officer for your organisation',
      },
      {
        values: hasOptedIn ? ['Yes'] : ['No'],
        label: 'Would you like to opt-in to be contacted for credit transfers?',
      },
      {
        values: creditContactDetailsValues,
        label: 'Contact details',
      },
    ];
  }
}
