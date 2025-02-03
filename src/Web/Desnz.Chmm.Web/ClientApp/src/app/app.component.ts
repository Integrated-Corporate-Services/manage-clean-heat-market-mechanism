import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  NavigationEnd,
  Router,
  RouterLink,
  RouterOutlet,
} from '@angular/router';
import { Store } from '@ngrx/store';
import { AppFooterComponent } from './navigation/footer/app-footer.component';
import { Subscription } from 'rxjs';
import { BackLinkComponent } from './navigation/back-link/back-link.component';
import { AppHeaderComponent } from './navigation/header/app-header.component';
import { isAdmin } from 'src/app/shared/auth-utils';
import { selectWhoAmI } from './stores/auth/selectors';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [
    NgIf,
    RouterLink,
    RouterOutlet,
    AppHeaderComponent,
    AppFooterComponent,
    BackLinkComponent,
  ],
})
export class AppComponent implements OnInit, OnDestroy {
  sub: Subscription = Subscription.EMPTY;

  isAdmin: boolean | null = null;
  subscription: Subscription | null = null;

  constructor(private store: Store, private router: Router) {
    this.subscription = this.store.select(selectWhoAmI).subscribe((whoAmI) => {
      if (!whoAmI) return;
      this.isAdmin = isAdmin(whoAmI);
    });}

  ngOnInit() {
    this.sub = this.router.events.subscribe((e) => {
      if (!(e instanceof NavigationEnd)) return;
      window.scroll(0, 0);
      (window as any).GOVUKFrontend.initAll();
    });
  }

  ngOnDestroy() {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
