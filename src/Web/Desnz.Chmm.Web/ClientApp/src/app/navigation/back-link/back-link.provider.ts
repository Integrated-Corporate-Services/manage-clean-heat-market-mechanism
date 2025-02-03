import { Injectable } from '@angular/core';
import { Params } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class BackLinkProvider {
  public link: string = '';
  public linkText: string = 'Back';
  public queryParams: Params | null = null;

  public clear() {
    this.link = '';
    this.linkText = 'Back';
  }
}
