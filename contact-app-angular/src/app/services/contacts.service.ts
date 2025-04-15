import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';

export interface GetContactsDTO {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
}

export interface GetContactDTO {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  category: string;
  subcategory?: string;
  phoneNumber: string;
  birthDate: string;
}

export interface PostContactDTO {
  firstName: string;
  lastName: string;
  category: string;
  subcategory?: string;
  email: string;
  phoneNumber: string;
  birthDate: string;
  password: string;
}

export interface PutContactDTO {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  birthDate: string;
  category: string;
  subcategory?: string;
  oldPassword: string;
  newPassword: string;
}

export interface DeleteContactDTO {
  password: string;
}

export interface ApiResponse {
  status: 'success' | 'error';
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ContactsService {

  constructor() { }
  private http = inject(HttpClient);
  private contactUrl = environment.apiUrl + '/api/contact';

  getContacts(): Observable<GetContactsDTO[]> {
    return this.http.get<GetContactsDTO[]>(this.contactUrl);
  }

  getContact(id: number): Observable<GetContactDTO> {
    return this.http.get<GetContactDTO>(`${this.contactUrl}/${id}`);
  }

  createContact(contact: PostContactDTO): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this.contactUrl, contact);
  }

  deleteContact(id: number, dto: DeleteContactDTO): Observable<ApiResponse> {
    return this.http.request<ApiResponse>('delete', `${this.contactUrl}/${id}`, {
      body: dto
    });
  }

  updateContact(id: number, dto: PutContactDTO): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.contactUrl}/${id}`, dto);
  }
}
