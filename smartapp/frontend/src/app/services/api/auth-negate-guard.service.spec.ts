import { TestBed } from '@angular/core/testing';

import { AuthNegateGuardService } from './auth-negate-guard.service';

describe('AuthNegateGuardService', () => {
  let service: AuthNegateGuardService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthNegateGuardService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
