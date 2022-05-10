import { TestBed } from '@angular/core/testing';

import { NetworkProviderService } from './network-provider.service';

describe('NetworkProviderService', () => {
  let service: NetworkProviderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NetworkProviderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
