import { TestBed } from '@angular/core/testing';

import { QuestionControllerService } from './question-controller.service';

describe('QuestionControllerService', () => {
  let service: QuestionControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(QuestionControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
