import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { map } from 'rxjs/operators';
import { AuthenticationService } from '../authentication.service';

export function adminGuard(): CanActivateFn {
  return () => inject(AuthenticationService).userInfo$.pipe(map((userInfo) => !!userInfo && userInfo.isAdmin));
}
