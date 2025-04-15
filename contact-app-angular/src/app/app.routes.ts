import { Routes } from '@angular/router';
import { RegisterComponent } from './components/registration/register/register.component';
import { LoginComponent } from './components/login/login/login.component';
import { ContactListComponent } from './components/contact-list/contact-list.component';
import { ContactDetailsComponent } from './components/contact-details/contact-details.component';
import { CreateContactComponent } from './components/create-contact/create-contact.component';
import { DeleteContactComponent } from './components/delete-contact/delete-contact.component';
import { authGuard } from './auth.guard';
import { EditContactComponent } from './components/edit-contact/edit-contact.component';

export const routes: Routes = [
    {path: '', redirectTo: '/contacts', pathMatch: 'full'},
    {path: 'contacts', component: ContactListComponent},
    {path: 'contacts/add', component: CreateContactComponent, canActivate: [authGuard]},
    {path: 'contacts/edit/:id', component: EditContactComponent, canActivate: [authGuard]},
    {path: 'contacts/delete/:id', component: DeleteContactComponent, canActivate: [authGuard]},
    {path: 'contacts/:id', component: ContactDetailsComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'login', component: LoginComponent},
];
