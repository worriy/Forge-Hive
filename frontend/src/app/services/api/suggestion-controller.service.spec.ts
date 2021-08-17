import { TestBed } from '@angular/core/testing';

import { SuggestionControllerService } from './suggestion-controller.service';

describe('SuggestionControllerService', () => {
  let service: SuggestionControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SuggestionControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
