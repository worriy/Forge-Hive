import { Observable } from 'rxjs';

let translatedWord = 'test';

export class TranslateServiceMock {
  public setDefaultLang(lang: string): void {
    return;
  }

  public use(lang: string): Observable<any> {
    return Observable.of(lang);
  }

  public get(word: string | string[], interpolateParams?: Object): Observable<any> {
    return Observable.of(translatedWord);
  }
}