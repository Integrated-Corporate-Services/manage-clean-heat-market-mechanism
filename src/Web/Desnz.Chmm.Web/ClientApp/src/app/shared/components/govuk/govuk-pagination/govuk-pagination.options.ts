import { BehaviorSubject } from "rxjs";

export interface GovukPagination {
    currentPage: number;
    pageSize: number;
    data: any[];
};