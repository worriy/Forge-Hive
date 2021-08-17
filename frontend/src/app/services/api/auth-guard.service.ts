import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { UserControllerService } from './user-controller.service';
import { tap } from 'rxjs/internal/operators/tap';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  constructor(
    private userControllerService: UserControllerService,
    private router: Router
  ) { }

  canActivate(): Observable<boolean> {
    return this.userControllerService.checkToken()
    .pipe(
      tap((state) => {
        console.log('State');
        console.log(state);
        if (state === false) {
          this.router.navigateByUrl('welcome');
        }
      }));
  }
}
