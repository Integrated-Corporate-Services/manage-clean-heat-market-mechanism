import { Injectable, Pipe, PipeTransform } from "@angular/core";
import { SchemeYearDto } from "../../../../stores/scheme-year-configuration/dtos/scheme-year.dto";

@Pipe({
  name: 'endDate',
  pure: true,
  standalone: true
})
export class SchemeConfigurationEndDateFilterPipe implements PipeTransform {
  transform(schemeYear: SchemeYearDto): string {
    return '31st December ' + (Number(schemeYear.year) - 1);
  }
}
