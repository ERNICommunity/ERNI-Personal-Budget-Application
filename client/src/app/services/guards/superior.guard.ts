import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import { map } from 'rxjs/operators';

export function superiorGuard(): CanActivateFn {
  return () => inject(AuthenticationService).userInfo$.pipe(map((userInfo) => !!userInfo && userInfo.isSuperior));
}
