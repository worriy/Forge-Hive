import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { take, takeUntil, tap } from 'rxjs/operators';
import { CreateGroup } from '../shared/interfaces/groups/create-group';
import { EditGroup } from '../shared/interfaces/groups/edit-group';
import { Group } from '../shared/interfaces/groups/group';
import { UpdateMembers } from '../shared/interfaces/groups/update-members';
import { Paging } from '../shared/interfaces/posts/paging';
import { GroupControllerService } from './api/group-controller.service';

@Injectable({
  providedIn: 'root'
})
export class GroupManagementService implements OnDestroy {
  private destroy = new Subject<void>();

  // The groups created by the current user
  public groups = new BehaviorSubject<Group[]>([]);

  // The groups of which the current user is a member of
  public userGroups = new BehaviorSubject<Group[]>([]);
  

  private userId: string;
  private paging: Paging = {
    step: 10,
    lastId: 0
  };

  constructor(
    private groupController: GroupControllerService
  ) 
  {
    this.userId = localStorage.getItem('userProfileId');
    this.getGroups();
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  /**
   * Retrieve groups for current user and emit result to observers
   */
  private getGroups() {
    this.groupController.list(this.paging , this.userId)
    .pipe(take(1))
    .subscribe( (res: Group[]) => {
      console.log(res);
      this.groups.next(res);
    });

    this.groupController.listMyGroups(this.userId, this.paging)
    .pipe(take(1))
    .subscribe((res: Group[]) => {
      this.userGroups.next(res);
    })
  }

  /**
   * Create a new group and fetch groups list
   * @param groupvm `CreateGroup`
   */
  public create(groupvm: CreateGroup): Observable<any> {
    return this.groupController.create(groupvm)
    .pipe(tap(() => this.getGroups()));
  }

  /**
   * Delete the group and fetch groups lists
   * @param groupId `number`
   */
  public delete(groupId: string): Observable<any> {
    return this.groupController.delete(groupId)
    .pipe(tap(() => this.getGroups()));
  }

  /**
   * Update a group and fetch group list
   * @param groupvm `EditGroup`
   */
  public update(groupvm: EditGroup): Observable<any> {
    return this.groupController.update(groupvm)
    .pipe(tap(() => this.getGroups()));
  }

  /**
   * Add members to a group and triggers groups fetching
   * @param members `UpdateMembers`
   */
  public addMembers(members: UpdateMembers): Observable<any> {
    return this.groupController.addMembers(members)
    .pipe(tap(() => this.getGroups()));
  }

  /**
   * Remove members from a group and triggers groups fetching
   * @param members `UpdateMembers`
   */
  public removeMembers(members: UpdateMembers): Observable<any> {
    return this.groupController.removeMembers(members)
    .pipe(tap(() => this.getGroups()));    
  }
}
