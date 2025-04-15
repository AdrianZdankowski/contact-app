import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor() { }
  private http = inject(HttpClient);
  private registerApiUrl = environment.apiUrl + '/api/auth/register';
  private loginApiUrl = environment.apiUrl + '/api/auth/login';

  private loggedIn$ = new BehaviorSubject<boolean>(this.isLoggedIn());

  isLoggedIn$ = this.loggedIn$.asObservable();

  register(email: string, password: string): Observable<any> {
    const userCredentials = {email, password};

    return this.http.post<any>(this.registerApiUrl, userCredentials);
  }

  login(email: string, password: string): Observable<any> {
    const userCredentials = {email, password};

    return this.http.post<any>(this.loginApiUrl, userCredentials);
  }

  setAuthToken(token: string) {
    sessionStorage.setItem('authToken', token);
    this.loggedIn$.next(true);
  }

  isLoggedIn(): boolean {
    return !!sessionStorage.getItem('authToken');
  }

  logout(): void {
    sessionStorage.removeItem('authToken');
    this.loggedIn$.next(false);
  }
}
