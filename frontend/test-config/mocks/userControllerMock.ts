
import { Observable } from 'rxjs';
import { FullUserVM } from '../../src/viewModels/fullUserVM';
import { PagingVM } from '../../src/viewModels/pagingVM';
import { UserVM } from '../../src/viewModels/userVM';
import { SignInVM } from '../../src/viewModels/signInVM';
import { ResetPasswordVM } from '../../src/viewModels/resetPasswordVM';
export class UserControllerMock {
    public getFullInfos(
        userProfileIdParam: string
    ): Observable<FullUserVM> {
      return Observable.of();
    }

    public list(
        pagingParam: PagingVM
    ): Observable<UserVM> {
      return Observable.of();
    }

    public update(
        userParam: UserVM
    ): Observable<any> {
      return Observable.of();
    }

    public signIn(
        signInParam: SignInVM
    ): Observable<any> {
      return Observable.of();
    }

    public checkEmail(
        emailParam: string
    ): Observable<any> {
      return Observable.of();
    }

    public resetPassword(
        resetPasswordParam: ResetPasswordVM
    ): Observable<any> {
      return Observable.of();
    }

    public get(
        applicationUserIdParam: string
    ): Observable<UserVM> {
      return Observable.of();
    }

    public searchUser(
        searchTermParam: string
    ): Observable<UserVM> {
      return Observable.of();
    }

}
