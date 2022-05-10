import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { UserControllerService } from './api/user-controller.service';
import {Observable, throwError} from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class HttpServerErrorInterceptorService implements HttpInterceptor {

  constructor(private userControllerService :UserControllerService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        if(error.status === 401){
          // redirect to Login Page
          this.userControllerService.signOut()
        }
        return throwError(error);
      })
    );
  }
}
