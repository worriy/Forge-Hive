import { TestBed } from '@angular/core/testing';

import { AnswerControllerService } from './answer-controller.service';

describe('AnswerControllerService', () => {
  let service: AnswerControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AnswerControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
