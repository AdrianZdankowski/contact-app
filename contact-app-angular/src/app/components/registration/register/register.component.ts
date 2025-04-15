import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;

  constructor(private authService: AuthService, private router: Router, private location:Location) {
    this.registerForm = new FormGroup({
      email: new FormControl('',[Validators.required, Validators.email]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(
          /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/
        )
      ])
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
          this.authService.setAuthToken(response.token);
          this.router.navigate(['/contacts']);
          this.location.go(this.router.url);
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
