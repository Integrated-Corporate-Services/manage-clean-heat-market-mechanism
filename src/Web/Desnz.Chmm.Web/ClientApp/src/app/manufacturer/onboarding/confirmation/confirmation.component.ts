import { Component, OnInit } from '@angular/core';
import { NgIf, NgFor, AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

export interface ILink {
  route: string;
  name: string;
  onClick: (() => any) | null;
}

@Component({
  selector: 'confirmation',
  templateUrl: './confirmation.component.html',
  standalone: true,
  imports: [NgFor, NgIf, RouterLink],
})
export class ConfirmationComponent implements OnInit {
  links: Partial<ILink>[] = [];

  constructor() {}

  ngOnInit() {}
}
