import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ContactsService } from '../../services/contacts.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-delete-contact',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './delete-contact.component.html',
  styleUrl: './delete-contact.component.css'
})
export class DeleteContactComponent {

  deleteForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private contactsService: ContactsService
  ) {
    this.deleteForm = new FormGroup({
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(
          '^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[\\W_]).{8,}$'
        )
      ])
    });
  }

  onSubmit() {
    if (this.deleteForm.invalid) return;

    const id = Number(this.route.snapshot.paramMap.get('id'));
    const { password } = this.deleteForm.value;

    this.contactsService.deleteContact(id, { password }).subscribe({
      next: () => {
        alert('Kontakt usunięty');
        this.router.navigate(['/contacts']);
      },
      error: (err) => {
        console.error(err);
        alert('Nie udało się usunąć kontaktu');
      }
    });
  }

}
