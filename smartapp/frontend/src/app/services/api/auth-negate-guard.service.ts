import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { UserControllerService } from './user-controller.service';
import { tap } from 'rxjs/internal/operators/tap';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthNegateGuardService implements CanActivate {

  constructor(
    private userControllerService: UserControllerService,
    private router: Router
  ) { }

  canActivate(): Observable<boolean> {
    return this.userControllerService.checkToken()
    .pipe(
      map(state => !state),
      tap((inverseState) => {
        if (inverseState === false) {
          this.router.navigateByUrl('tabs/flow');
        }
      }));
  }
}
