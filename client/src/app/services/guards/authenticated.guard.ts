import { inject } from '@angular/core';
import { CanActivateFn, RedirectCommand, Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { AuthenticationService } from '../authentication.service';

export function authenticatedGuard(): CanActivateFn {
  return () => {
    const authenticationService = inject(AuthenticationService);
    const router = inject(Router);

    return authenticationService.isAuthenticated().pipe(
      map((isAuthenticated) => {
        if (isAuthenticated) {
          return true;
        } else {
          return new RedirectCommand(router.parseUrl('/login'), { skipLocationChange: true });
        }
      }),
    );
  };
}
