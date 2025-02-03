import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs';
import { getOrganisationsAvailableForTransfer } from '../../../stores/organisations/actions';
import { BackLinkProvider } from '../../../navigation/back-link/back-link.provider';
import { ContactOrganisationDto } from '../../../stores/onboarding/dtos/contact-organisation-dto';
import { selectContactsState } from '../../../stores/contacts/selectors';

interface Accordion {
  sections: AccordionSection[];
}

interface AccordionSection {
  show: boolean;
  details: ContactOrganisationDto;
}

@Component({
  selector: 'transfer-contacts',
  templateUrl: './transfer-contacts.component.html',
  styleUrls: ['./transfer-contacts.component.css'],
  standalone: true,
  imports: [NgFor, NgIf, AsyncPipe],
})
export class TransferContactsComponent implements OnInit, OnDestroy {
  @Input({ required: true }) organisationId!: string;
  @Input({ required: true }) schemeYearId!: string;

  accordion: Accordion | null = null;
  subscription: Subscription = Subscription.EMPTY;

  constructor(
    private store: Store,
    private backLinkProvider: BackLinkProvider
  ) {
    this.subscription = this.store
      .select(selectContactsState)
      .subscribe((organisations) => {
        if (!organisations.data) return;
        this.accordion = {
          sections: organisations.data
            .map((organisation) => ({
            show: false,
            details: organisation,
          })),
        };
      });
  }

  toggle(section: AccordionSection) {
    section.show = !section.show;
  }

  ngOnInit() {
    if (this.organisationId === null || this.organisationId === undefined) {
      throw TypeError('organisationId cannot be null or undefined');
    }
    if (this.schemeYearId === null || this.schemeYearId === undefined) {
      throw TypeError('schemeYearId cannot be null or undefined');
    }

    this.backLinkProvider.link = `/scheme-year/${this.schemeYearId}/organisation/${this.organisationId}/summary`;

    this.store.dispatch(
      getOrganisationsAvailableForTransfer({
        organisationId: this.organisationId,
      })
    );
  }

  ngOnDestroy() {
    if (this.subscription) this.subscription.unsubscribe();
  }
}
