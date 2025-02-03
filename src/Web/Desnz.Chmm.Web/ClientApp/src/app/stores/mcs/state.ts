import { InstallationRequestSummaryDto } from './dto/installation-request-summary.dto';

export interface McsState {
  installationRequestSummaries: InstallationRequestSummaryDto[];
  errorMessage: string | null;
  loading: boolean;
}
