import { TestBed } from '@angular/core/testing';

import { PostsControllerService } from './posts-controller.service';

describe('PostsControllerService', () => {
  let service: PostsControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PostsControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
