import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';

import { DataService } from './data.service';

import { Group } from 'src/app/shared/interfaces/groups/group';
import { EditGroup } from 'src/app/shared/interfaces/groups/edit-group';
import { TargetGroup } from 'src/app/shared/interfaces/groups/target-group';
import { User } from 'src/app/shared/interfaces/user/user';
import { UpdateMembers } from 'src/app/shared/interfaces/groups/update-members';
import { CreateGroup } from 'src/app/shared/interfaces/groups/create-group';
import { Paging } from 'src/app/shared/interfaces/posts/paging';

@Injectable({
  providedIn: 'root'
})
export class GroupControllerService {
  public _createApi = '/api/group/create';
  public _deleteApi = '/api/group/delete';
  public _updateApi = '/api/group/update';
  public _getApi = '/api/group/get';
  public _listApi = '/api/group/list';
  public _listTargetableGroupsApi = '/api/group/listTargetableGroups';
  public _listMyGroups = '/api/group/listMyGroups';
  public _getMembersApi = '/api/group/getMembers';
  public _addMembersApi = '/api/group/addMembers';
  public _removeMemberApi = '/api/group/removeMembers';

  groups: Array<Group>;
  myGroups: Array<Group>;

  constructor(
    public _dataService: DataService
  ){ }

  /**
   * method: create.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param groupParam `CreateGroupVM`.
   */
  public create(groupParam: CreateGroup) {
    return this._dataService.post( 
      `${this._createApi}`,
      {  
        createdById: groupParam.createdbyId,
        name: groupParam.name,
        city: groupParam.city,
        country: groupParam.country
      }, 
      {});
  }

  /**
   * method: delete.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param groupIdParam `number`.
   */
  public delete(groupIdParam: string) {
    return this._dataService.delete(
      `${this._deleteApi}`,
      {
          groupId: groupIdParam
      }
    )
  }

  /**
   * method: update.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param groupParam `EditGroupVM`.
   */
  public update(groupParam: EditGroup) {
    let name = groupParam.name;
    let city = groupParam.city;
    let country = groupParam.country;
    let id = groupParam.id;
    return this._dataService.put(
      `${this._updateApi}`,
      {
        name,city,country,id
      },
      {}
    );
  }

  /**
   * method: get.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param groupIdParam `string`.
   * @returns `GroupVM>`.
   */
  public get(groupIdParam: string): Observable<Group> {
    return this._dataService.get(
      `${this._getApi}`,
      {
          groupId: JSON.stringify(groupIdParam)
      }
    );
  }

  /**
   * method: list.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param pagingParam `PagingVM`.
   * @param userProfileIdParam `number`.
   * @returns `GroupVM>`.
   */
  public list(
      pagingParam: Paging,
      userProfileIdParam: string
  ): Observable<Group[]> {
    const userProfileId = userProfileIdParam;
    const step = JSON.stringify(pagingParam.step);
    const lastId = JSON.stringify(pagingParam.lastId);
    return this._dataService.get(
      `${this._listApi}`,
      {
        step,
        lastId,
        userProfileId
      }
    );
  }

  /**
   * method: listTargetableGroups.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param userProfileIdParam `string`.
   * @returns `TargetGroupVM>`.
   */
  public listTargetableGroups(
      userProfileIdParam: string,
      pagingParam: Paging
  ): Observable<TargetGroup[]> {
    return this._dataService.get(
      `${this._listTargetableGroupsApi}`,
      {
          step: pagingParam.step.toString(),
          lastId: pagingParam.lastId.toString(),
          userProfileId: userProfileIdParam
      });
  }

  /**
   * method: listTargetableGroups.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param userProfileIdParam `string`.
   * @returns `TargetGroupVM>`.
   */
  public listMyGroups(
      userProfileIdParam: string,
      pagingParam: Paging
  ): Observable<Group[]> {
    return this._dataService.get(
      `${this._listMyGroups}`,
      {
        step: pagingParam.step.toString(),
        lastId: pagingParam.lastId.toString(),
        userProfileId: userProfileIdParam
      });
  }

  /**
   * method: getMembers.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param pagingParam `PagingVM`.
   * @param groupIdParam `number`.
   * @returns `UserVM>`.
   */
  public getMembers(
      pagingParam: Paging,
      groupIdParam: string
  ): Observable<Array<User>> {
    let step = JSON.stringify(pagingParam.step);
    let lastId = JSON.stringify(pagingParam.lastId);
    return this._dataService.get(
      `${this._getMembersApi}`,
      {
          step,
          lastId,
          groupId: groupIdParam
      });
  }
  /**
   * method: updateMembers.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param membersListParam `UpdateMembersVM`.
   */
  public addMembers(
      membersListParam: UpdateMembers
  ) {
    let userIds = membersListParam.userIds;
    let groupId = membersListParam.groupId;
    return this._dataService.put(
      `${this._addMembersApi}`,
      {
          userIds,groupId
      },
      {});
  }
  /**
   * method: removeMember.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param userProfileIdParam `number`.
   * @param groupIdParam `number`.
   */
  public removeMembers(
      membersListParam: UpdateMembers
  ) {
    let userIds = membersListParam.userIds;
    let groupId = membersListParam.groupId;
    return this._dataService.put(
      `${this._removeMemberApi}`,
      {
        userIds,
        groupId
      },
      {});
  }
}
