import { TestBed } from '@angular/core/testing';

import { NotificationsControllerService } from './notifications-controller.service';

describe('NotificationsControllerService', () => {
  let service: NotificationsControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NotificationsControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
