import { TestBed } from '@angular/core/testing';

import { IdeaControllerService } from './idea-controller.service';

describe('IdeaControllerService', () => {
  let service: IdeaControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(IdeaControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
