import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ServerConfigService } from '../server-config.service';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  constructor(public http: HttpClient, public serverConfig: ServerConfigService) { 
    this.serverUrl = serverConfig.getServerUrl();
  }

  private serverUrl: string;

  get apiEndpoint() {
    return this.serverUrl;
  }
  /**
   * method: get.
   * A generic HttpClient get method.
   * @param uri An URI for get method, add URL param
   *            directly from here or use `parameters`.
   * @param parameters URL parameters.
   * @returns Returns an `Observable<any>`.
   */
  get(uri: string, parameters: { [param: string]: string | string[] }): Observable<any> {
    return this.http.get(this.serverUrl + uri, {
      //headers: this.jwt(),
      params: parameters
    }).pipe(
      catchError(error => {
        return throwError(Error || 'Server error');
      })
    )
  }

  /**
   * method: post.
   * A generic HttpClient post method.
   * @param uri An URI for post method, add URL param
   *            directly from here or use `parameters`.
   * @param body A JSON body is requested here.
   * @param parameters URL parameters.
   * @returns Returns an `Observable<any>`.
   */
  post(uri: string, body, parameters: { [param: string]: string | string[] }) {
    return this.http.post(this.serverUrl + uri, body, {
      //headers: this.jwt(),
      params: parameters
    }).pipe(
      catchError(error => {
        return throwError(Error || 'Server error');
      })
    )
  }

  /**
   * method: put.
   * A generic HttpClient put method.
   * @param uri An URI for put method, add URL param
   *            directly from here or use `parameters`.
   * @param body A JSON body is requested here.
   * @param parameters URL parameters.
   * @returns Returns an `Observable<any>`.
   */
  put(uri: string, body, parameters: { [param: string]: string | string[] }) {
    return this.http.put(this.serverUrl + uri, body, {
      //headers: this.jwt(),
      params: parameters
    }).pipe(
      catchError(error => {
        return throwError(Error || 'Server error');
      })
    )
  }

  /**
   * method: delete.
   * A generic HttpClient delete method.
   * @param uri An URI for delete method, add URL param
   *            directly from here or use `parameters`.
   * @param parameters URL parameters.
   * @returns Returns an `Observable<any>`.
   */
  delete(uri: string, parameters: { [param: string]: string | string[] }) {
    return this.http.delete(`${this.serverUrl}` + uri, {
      //headers: this.jwt(),
      params: parameters
    }).pipe(
      catchError(error => {
        return throwError(Error || 'Server error');
      })
    )
  }

}
