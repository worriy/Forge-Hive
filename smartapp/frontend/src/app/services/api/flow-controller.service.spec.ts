import { TestBed } from '@angular/core/testing';

import { FlowControllerService } from './flow-controller.service';

describe('FlowControllerService', () => {
  let service: FlowControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FlowControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
