import { TestBed } from '@angular/core/testing';

import { QuoteControllerService } from './quote-controller.service';

describe('QuoteControllerService', () => {
  let service: QuoteControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(QuoteControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
