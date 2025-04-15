import { Component, OnInit } from '@angular/core';
import { Router, RouterLink,RouterOutlet, NavigationEnd } from '@angular/router';
import { AuthService } from './services/auth.service';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'contact-app-angular';
  loggedIn = false;

  constructor(private authService: AuthService, private router: Router) {
    this.authService.isLoggedIn$.subscribe(status => {
      this.loggedIn = status;
    });

    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.loggedIn = this.authService.isLoggedIn();
      });
  }

  ngOnInit(): void {
    this.loggedIn = this.authService.isLoggedIn();
  }

  logout() : void {
    this.authService.logout();
    this.loggedIn = false;
  }


}
