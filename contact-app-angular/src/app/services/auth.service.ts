import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor() { }
  private http = inject(HttpClient);
  private registerApiUrl = environment.apiUrl + '/api/auth/register';
  private loginApiUrl = environment.apiUrl + '/api/auth/login';

  register(email: string, password: string): Observable<any> {
    const userCredentials = {email, password};

    return this.http.post<any>(this.registerApiUrl, userCredentials);
  }

  login(email: string, password: string): Observable<any> {
    const userCredentials = {email, password};

    return this.http.post<any>(this.loginApiUrl, userCredentials);
  }
}
