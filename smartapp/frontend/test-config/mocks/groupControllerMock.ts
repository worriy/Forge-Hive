
import { Observable } from 'rxjs';
import { CreateGroupVM } from '../../src/viewModels/createGroupVM';
import { EditGroupVM } from '../../src/viewModels/editGroupVM';
import { GroupVM } from '../../src/viewModels/groupVM';
import { TargetGroupVM } from '../../src/viewModels/targetGroupVM';
import { UserVM } from '../../src/viewModels/userVM';
import { UpdateMembersVM } from '../../src/viewModels/updateMembersVM';
export class GroupControllerMock {
    public create(
        groupParam: CreateGroupVM
    ): Observable<any> {
      return Observable.of();
    }

    public delete(
        groupIdParam: number
    ): Observable<any> {
      return Observable.of();
    }

    public update(
        groupParam: EditGroupVM
    ): Observable<any> {
      return Observable.of();
    }

    public get(
        groupIdParam: string
    ): Observable<GroupVM> {
      return Observable.of();
    }

    public list(
        userProfileIdParam: number
    ): Observable<GroupVM> {
      return Observable.of();
    }

    public listTargetableGroups(
        userProfileIdParam: string
    ): Observable<TargetGroupVM> {
      return Observable.of();
    }

    public getMembers(
        groupIdParam: number
    ): Observable<UserVM> {
      return Observable.of();
    }

    public updateMembers(
        membersListParam: UpdateMembersVM
    ): Observable<any> {
      return Observable.of();
    }

    public removeMember(
        userProfileIdParam: number,
        groupIdParam: number
    ): Observable<any> {
      return Observable.of();
    }

}
