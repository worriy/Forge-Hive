import { TestBed } from '@angular/core/testing';

import { SurveyControllerService } from './survey-controller.service';

describe('SurveyControllerService', () => {
  let service: SurveyControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SurveyControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
