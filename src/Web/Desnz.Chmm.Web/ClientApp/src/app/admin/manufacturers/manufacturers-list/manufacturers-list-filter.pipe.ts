import { Pipe, PipeTransform } from '@angular/core';
import { ViewOrganisationDto } from 'src/app/stores/onboarding/dtos/view-organisation-dto';
import * as _ from 'lodash-es';

@Pipe({
  name: 'manufacturersListFilter',
  standalone: true,
})
export class ManufacturersListFilterPipe implements PipeTransform {
  transform(
    manufacturers: ViewOrganisationDto[],
    filter: string,
    showArchived: boolean
  ) {
    // Ensure the array is not null
    manufacturers = manufacturers || [];

    // Do we want to see archived?
    if (!showArchived) {
      manufacturers = _.filter(manufacturers, (m) => m.status != 'Archived');
    }

    // Filter by name if a filter is specified
    if (filter) {
      let filterUpper = filter.toUpperCase();

      return manufacturers.filter((m) => {

        // Check if name matches
        let nameMatches = m.name.toUpperCase().indexOf(filterUpper) !== -1;

        // Check if licence holder name matches
        let licenceHolderMatches = false;
        if (!!m.licenceHolders) {
          licenceHolderMatches = m.licenceHolders.some(l => l.name.toUpperCase().indexOf(filterUpper) !== -1);
        }

        return nameMatches || licenceHolderMatches;
      });
    }

    // Apply sorting last
    const statusOrder: Record<string, number> = {
      Pending: 0,
      Active: 1,
      Retired: 2,
    };
    manufacturers = _.sortBy(manufacturers, [
      function (m) {
        return statusOrder[m.status || 'Pending'];
      },
      function (m) {
        return m.name;
      },
    ]);

    return manufacturers;
  }
}
