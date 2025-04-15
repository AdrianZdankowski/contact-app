import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const authToken = sessionStorage.getItem('authToken');

  if (authToken) {
    return true;
  }

  const router = inject(Router);
  router.navigate(['/login']);
  return false;
};
