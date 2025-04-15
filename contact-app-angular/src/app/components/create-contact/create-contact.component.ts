import { Component } from '@angular/core';
import { FormGroup,FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { ContactsService, PostContactDTO } from '../../services/contacts.service';
import { Router } from '@angular/router';
import { CommonModule, Location } from '@angular/common';

@Component({
  selector: 'app-create-contact',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-contact.component.html',
  styleUrl: './create-contact.component.css'
})
export class CreateContactComponent {

  contactForm: FormGroup;
  selectedCategory: string = '';

  constructor( 
    private contactsService: ContactsService, 
    private router: Router,
    private location: Location) {
      this.contactForm = new FormGroup({
        firstName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
        lastName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
        category: new FormControl('', Validators.required),
        subcategory: new FormControl(''),
        email: new FormControl('', [Validators.required, Validators.email]),
        phoneNumber: new FormControl('', [
          Validators.required,
          Validators.maxLength(9),
          Validators.pattern(/^\d{9}$/) // tylko cyfry, dokładnie 9
        ]),
        birthDate: new FormControl('', Validators.required),
        password: new FormControl('', [
          Validators.required,
          Validators.minLength(8),
          Validators.pattern(
            /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/
          )
        ])
      });
  }

  onSubmit() {
    if (this.contactForm.valid) {

      const contact: PostContactDTO = this.contactForm.value;

      this.contactsService.createContact(contact).subscribe(
        (response) => {
          alert('Contact created');
          this.router.navigate(['/contacts']);
          this.location.go(this.router.url);
        },
        (error) => {
          console.error('Login error', error);
        }
      );
    } else {
      alert('Form is not valid!');
    }
  }

  onCategoryChange(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.selectedCategory = value;
  
    // Resetuj subkategorię jeśli nie dotyczy
    if (value === 'Prywatny') {
      this.contactForm.get('subcategory')?.setValue('');
    }
  }
}
