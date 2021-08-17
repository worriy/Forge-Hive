import { Observable } from 'rxjs';

export class DataServiceMock {
  get(uri: string, parameters: { [param: string]: string | string[] }) {
    return Observable.of();
  }

  post(uri: string, body, parameters: { [param: string]: string | string[] }) {
    return Observable.of();
  }

  put(uri: string, body, parameters: { [param: string]: string | string[] }) {
    return Observable.of();
  }

  delete(uri: string, parameters: { [param: string]: string | string[] }) {
    return Observable.of();
  }
}