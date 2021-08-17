import { TestBed } from '@angular/core/testing';

import { EventControllerService } from './event-controller.service';

describe('EventControllerService', () => {
  let service: EventControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EventControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
