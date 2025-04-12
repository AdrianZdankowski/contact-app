import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;

  constructor(private authService: AuthService) {
    this.registerForm = new FormGroup({
      email: new FormControl('',[Validators.required]),
      password: new FormControl('',[Validators.required, Validators.minLength(8)])
    });
  }

  ngOnInit(): void {
    
  }

  onSubmit() {
    if (this.registerForm.valid) {
      const { email, password } = this.registerForm.value;

      this.authService.register(email, password).subscribe(
        (response) => {
          console.log('Registration successfull', response);
          sessionStorage.setItem('authToken', response.token);
          console.log('Token saved: ', response.token);
        },
        (error) => {
          console.error('Registration error', error);
        }
      );
    } else {
      console.log('Form is not valid!');
    }
  }
}
