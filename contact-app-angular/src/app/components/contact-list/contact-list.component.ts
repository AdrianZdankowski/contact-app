import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContactsService, GetContactsDTO } from '../../services/contacts.service';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-contact-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './contact-list.component.html',
  styleUrl: './contact-list.component.css'
})
export class ContactListComponent implements OnInit {
  contacts: GetContactsDTO[] = [];

  constructor(
    private contactsService: ContactsService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.contactsService.getContacts().subscribe({
      next: (response) => this.contacts = response,
      error: (error) => console.error("Error with loading a contact", error)
    });
  }

}
