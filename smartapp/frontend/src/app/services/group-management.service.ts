import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { take, takeUntil, tap } from 'rxjs/operators';
import { CreateGroup } from '../shared/interfaces/groups/create-group';
import { EditGroup } from '../shared/interfaces/groups/edit-group';
import { Group } from '../shared/interfaces/groups/group';
import { UpdateMembers } from '../shared/interfaces/groups/update-members';
import { GroupControllerService } from './api/group-controller.service';
import { UserControllerService } from './api/user-controller.service';

@Injectable({
  providedIn: 'root'
})
export class GroupManagementService implements OnDestroy {
  private destroy = new Subject<void>();

  // The groups created by the current user
  public userCreatedGroups = new BehaviorSubject<Group[]>([]);

  // The groups of which the current user is a member of
  public userMemberGroups = new BehaviorSubject<Group[]>([]);
  

  private userId: string;

  constructor(
    private groupController: GroupControllerService,
    public _userController: UserControllerService
  ) 
  {
    this.userId = _userController.getUserProfileId();
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
    this.groupController.list(this.userId)
    .pipe(take(1))
    .subscribe( (res: Group[]) => {
      let userCreatedGroups = [];
      let userMemberGroups = [];
      res.forEach(group => {
        if (group.isAuthor)
          userCreatedGroups.push(group);
        else
          userMemberGroups.push(group);
      })
      this.userCreatedGroups.next(userCreatedGroups);
      this.userMemberGroups.next(userMemberGroups);
    });
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
