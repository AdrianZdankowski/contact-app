import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ContactsService } from '../../services/contacts.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-contact',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-contact.component.html',
  styleUrl: './edit-contact.component.css'
})
export class EditContactComponent implements OnInit {
  editForm!: FormGroup;
  contactId!: number;
  selectedCategory: string = '';

  constructor(
    private route: ActivatedRoute,
    private contactService: ContactsService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.contactId = Number(this.route.snapshot.paramMap.get('id'));

    this.editForm = new FormGroup({
      firstName: new FormControl('', [Validators.maxLength(50)]),
      lastName: new FormControl('', [Validators.maxLength(50)]),
      email: new FormControl('', [Validators.email]),
      phoneNumber: new FormControl('', [Validators.maxLength(9)]),
      birthDate: new FormControl('', Validators.required),
      category: new FormControl('', Validators.required),
      subCategory: new FormControl(''),
      oldPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\W_]).{8,}$')
      ]),
      newPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\W_]).{8,}$')
      ])
    });

    this.contactService.getContact(this.contactId).subscribe(contact => {
      this.selectedCategory = contact.category;
      this.editForm.patchValue({
        firstName: contact.firstName,
        lastName: contact.lastName,
        email: contact.email,
        phoneNumber: contact.phoneNumber,
        birthDate: contact.birthDate,
        category: contact.category,
        subCategory: contact.subcategory ?? ''
      });
    });
  }

  onCategoryChange(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.selectedCategory = value;

    if (value === 'Prywatny') {
      this.editForm.get('subCategory')?.setValue('');
    }
  }

  onSubmit() {
    if (this.editForm.valid) {
      this.contactService.updateContact(this.contactId, this.editForm.value).subscribe({
        next: () => {
          alert('Contact updated!');
          this.router.navigate(['/contacts']);
        },
        error: (error) => {
          console.error('Error with update:', error);
        }
      });
    } else {
      console.log('Form is not valid!');
    }
  }
}
