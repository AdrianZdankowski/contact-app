import { Routes } from '@angular/router';
import { RegisterComponent } from './components/registration/register/register.component';
import { LoginComponent } from './components/login/login/login.component';

export const routes: Routes = [
    {path: '', redirectTo: '/register', pathMatch: 'full'},
    {path: 'register', component: RegisterComponent},
    {path: 'login', component: LoginComponent},
];
