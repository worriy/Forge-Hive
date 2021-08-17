import { TestBed } from '@angular/core/testing';

import { PictureControllerService } from './picture-controller.service';

describe('PictureControllerService', () => {
  let service: PictureControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PictureControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
