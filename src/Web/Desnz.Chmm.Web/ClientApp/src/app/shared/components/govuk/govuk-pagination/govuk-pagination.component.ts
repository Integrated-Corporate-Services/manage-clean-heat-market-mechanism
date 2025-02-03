import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { GovukPagination } from './govuk-pagination.options';

const defaultOptions: GovukPagination = {
    currentPage: 1,
    pageSize: 10,
    data: []
};

@Component({
  selector: 'govuk-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './govuk-pagination.component.html',
  styleUrls: ['./govuk-pagination.component.css'],
})
export class GovukPaginationComponent implements OnChanges {

    @Input({ required: true }) options!: Partial<GovukPagination>;

    currentPageSubject: BehaviorSubject<number>;
    pageSizeSubject: BehaviorSubject<number>;
    pagesSubject: BehaviorSubject<number[]>;

    currentPage$: Observable<number>;
    pageSize$: Observable<number>;
    pages$: Observable<number[]>;

    @Output() onPageChange = new EventEmitter<any[]>();

    private state: GovukPagination;

    constructor() {
        this.currentPageSubject = new BehaviorSubject<number>(1);
        this.pageSizeSubject = new BehaviorSubject<number>(10);
        this.pagesSubject = new BehaviorSubject<number[]>([]);
    
        this.currentPage$ = this.currentPageSubject.asObservable();
        this.pageSize$ = this.pageSizeSubject.asObservable();
        this.pages$ = this.pagesSubject.asObservable();

        this.state = { ...defaultOptions };
    }

    updateData() {
        const startIndex = (this.currentPageSubject.value - 1) * this.pageSizeSubject.value;
        const endIndex = startIndex + this.pageSizeSubject.value;
        const data = this.state.data.slice(startIndex, endIndex);
        this.onPageChange.emit(data);
    }

    ngOnChanges(changes: SimpleChanges) {
        this.state = { ...defaultOptions, ...changes['options'].currentValue };
        console.log(this.state)
        this.currentPageSubject.next(this.state.currentPage);
        this.pageSizeSubject.next(this.state.pageSize);
        const pages = Array.from({
            length: Math.ceil(this.state.data.length / this.state.pageSize)
        }, (_, i) => i + 1);
        this.pagesSubject.next(pages);
        setTimeout(() => this.changePage(this.state.currentPage), 0);
    }

    changePage(pageNumber: number) {
        this.currentPageSubject.next(pageNumber);
        this.updateData();
    }
}
