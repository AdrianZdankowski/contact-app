import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ContactsService, GetContactDTO } from '../../services/contacts.service';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-contact-details',
  imports: [CommonModule, RouterLink],
  templateUrl: './contact-details.component.html',
  styleUrl: './contact-details.component.css'
})
export class ContactDetailsComponent implements OnInit{
  contact?: GetContactDTO;

  constructor(private contactsService: ContactsService, 
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.contactsService.getContact(id).subscribe({
        next: (response) => this.contact = response,
        error: (error) => console.error('Error with loading the contact details', error)
      });
    }
  }
}
