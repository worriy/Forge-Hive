import { TestBed } from '@angular/core/testing';

import { GroupManagementService } from './group-management.service';

describe('GroupManagementService', () => {
  let service: GroupManagementService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GroupManagementService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
