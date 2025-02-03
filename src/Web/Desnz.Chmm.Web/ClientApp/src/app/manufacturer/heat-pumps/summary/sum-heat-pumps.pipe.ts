import { Pipe, PipeTransform } from "@angular/core";
import { PeriodCreditTotalsDto } from "src/app/stores/heat-pumps/dtos/period-credit-totals.dto";

@Pipe({ name: 'sumHeatPumps', standalone: true })
export class SumHeatPumpsPipe implements PipeTransform {
  transform(
    heatPumpInstallations: PeriodCreditTotalsDto[],
    sumValue: 'installations' | 'credits',
    sumSource: 'non-hybrid' | 'hybrid'): number {
    if (!heatPumpInstallations.length) return 0;
    let source: (x: PeriodCreditTotalsDto) => number;
    if (sumValue === 'installations' && sumSource === 'non-hybrid') source = x => x.heatPumpsInstallations;
    else if (sumValue === 'installations' && sumSource === 'hybrid') source = x => x.hybridHeatPumpsInstallations;
    else if (sumValue === 'credits' && sumSource === 'non-hybrid') source = x => x.heatPumpsGeneratedCredits;
    else if (sumValue === 'credits' && sumSource === 'hybrid') source = x => x.hybridHeatPumpsGeneratedCredits;
    else return 0;
    return heatPumpInstallations.reduce((prev, curr) => prev + source(curr), 0);
  }
}