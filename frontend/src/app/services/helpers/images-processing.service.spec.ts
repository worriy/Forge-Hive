import { TestBed } from '@angular/core/testing';

import { ImagesProcessingService } from './images-processing.service';

describe('ImagesProcessingService', () => {
  let service: ImagesProcessingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ImagesProcessingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
