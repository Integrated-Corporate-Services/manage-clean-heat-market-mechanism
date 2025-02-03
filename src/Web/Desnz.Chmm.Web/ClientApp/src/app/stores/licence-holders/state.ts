import { LinkLicenceHolderFormValue } from 'src/app/admin/manufacturers/licence-holders/link-licence-holder/link-licence-holder.component';
import { HttpState } from '../http-state';
import { LicenceHolderDto } from './dto/licence-holder';
import { SchemeYearDto } from '../organisation-summary/dtos/scheme-year.dto';
import { LicenceHolderLinkDto } from './dto/licence-holder-link.dto';
import { EditLinkedLicenceHolderFormValue } from 'src/app/admin/manufacturers/licence-holders/edit-linked-licence-holder/edit-linked-licence-holder.component';

export interface LicenceHolderState {
  currentSchemeYear: HttpState<SchemeYearDto>;
  firstSchemeYear: HttpState<SchemeYearDto>;
  linked: HttpState<LicenceHolderLinkDto[]>;
  unlinked: HttpState<LicenceHolderDto[]>;
  editLinkedLicenceHolder: HttpState;
  selectedLicenceHolder: LicenceHolderDto | null;
  selectedLicenceHolderLink: LicenceHolderLinkDto | null;
  linkLicenceHolderFormValue: LinkLicenceHolderFormValue | null;
  editLinkedLicenceHolderFormValue: EditLinkedLicenceHolderFormValue | null;
  startOfLink: string | null;
  endOfLink: string | null;
}
