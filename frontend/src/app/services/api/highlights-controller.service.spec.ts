import { TestBed } from '@angular/core/testing';

import { HighlightsControllerService } from './highlights-controller.service';

describe('HighlightsControllerService', () => {
  let service: HighlightsControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HighlightsControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
