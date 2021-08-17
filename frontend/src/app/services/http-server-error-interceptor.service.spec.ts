import { TestBed } from '@angular/core/testing';

import { HttpServerErrorInterceptorService } from './http-server-error-interceptor.service';

describe('HttpServerErrorInterceptorService', () => {
  let service: HttpServerErrorInterceptorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HttpServerErrorInterceptorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
