import { Injectable, Injector } from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import { UserControllerService } from './api/user-controller.service';

@Injectable({
  providedIn: 'root'
})
export class AuthInterceptorService implements HttpInterceptor {

  constructor(private injector: Injector, private userControllerService :UserControllerService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    
    let requestToForward = req;

    if (this.userControllerService === undefined) {
      
      this.userControllerService = this.injector.get(UserControllerService);
    }
    if (this.userControllerService !== undefined) {
      const token = this.userControllerService.getToken();
      if(token !== null){
        const tokenValue = 'Bearer ' + token;
        requestToForward = req.clone({setHeaders: {Authorization: tokenValue}});
      } 
    } else {
      console.log('SecurityService undefined: NO auth header!');
    }

    return next.handle(requestToForward);
  }
}
