import { Injectable } from '@angular/core';
import { Observable, from, BehaviorSubject} from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { Storage } from '@ionic/storage';
import { DataService } from './data.service';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { FullUser } from 'src/app/shared/interfaces/user/full-user';
import { Register } from 'src/app/shared/interfaces/user/register';
import { ResetPassword } from 'src/app/shared/interfaces/user/reset-password';
import { SignIn } from 'src/app/shared/interfaces/user/sign-in';
import { UpdatePassword } from 'src/app/shared/interfaces/user/update-password';
import { UpdateProfilePicture } from 'src/app/shared/interfaces/user/update-profile-picture';
import { User } from 'src/app/shared/interfaces/user/user';
import { UserUpdate } from 'src/app/shared/interfaces/user/user-update';
import { AuthData } from 'src/app/shared/interfaces/user/auth-data';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class UserControllerService {
  public _getFullInfosApi = '/api/user/getFullInfos';
  public _listApi = '/api/user/list';
  public _updateApi = '/api/Auth/updateUser';
  public _signInApi = '/api/Auth/login';
  public _refreshToken = '/api/Auth/refreshToken';
  public _loginFacebookApi = '/api/Auth/loginFacebook';
  public _logingoogleApi = '/api/Auth/loginGoogle';
  public _checkEmailApi = '/api/Auth/checkEmail';
  public _registerApi = '/api/Auth/register';
  public _resetPasswordApi = '/api/Auth/resetPassword';
  public _getApi = '/api/user/get';
  public _updatePasswordApi = '/api/Auth/updatePassword';
  public _updatePicture = '/api/user/updatePicture';
  public _existEmailApi = '/api/Auth/ExistEmail';
  public _getRolesApi = '/api/Auth/getRoles';
  public _confirmEmailApi = '/api/Auth/confirmEmail';

  public authenticationState = new BehaviorSubject(null);
  private tokenStorageKey = 'token';
  private tokenExpirationDate = 'tokenExpirationDate';
  private authenticatedUser = 'authenticatedUser'

  constructor(public _dataService: DataService, public storage: Storage, public router: Router) { }


  get tokenValidate$() {
    return from(this.getValidateToken());
  }
  checkToken(): Observable<boolean> {
    //return this.token$
    return this.tokenValidate$
      .pipe(tap((isToken : boolean) => {
        
        this.authenticationState.next(isToken);
      }));
  }
  /**
   * method: getFullInfos.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param userProfileIdParam `string`.
   * @returns `FullUserVM>`.
   */
  public getFullInfos(userProfileIdParam: string): Observable<FullUser> {
    return this._dataService.get(
      `${this._getFullInfosApi}`,
      {
        userProfileId: userProfileIdParam
      });
  }

  /**
   * method: list.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param pagingParam `PagingVM`.
   * @returns `UserVM>`.
   */
  public list(pagingParam: Paging): Observable<Array<User>> {
    let step = JSON.stringify(pagingParam.step);
    let lastId = JSON.stringify(pagingParam.lastId);
    return this._dataService.get(
      `${this._listApi}`,
      {
        step,
        lastId
      });
  }

  /**
   * method: update.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param userParam `UserVM`.
   */
  public update(user: UserUpdate) {
    return this._dataService.put(
      `${this._updateApi}`,
      {
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        phoneNumber: user.phoneNumber,
        country: user.country,
        city: user.city,
        department: user.department,
        job: user.job
      },
      {});
  }

  /**
   * method: signIn.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param signInParam `SignInVM`.
   */
  public signIn(signInParam: SignIn) {
    return this._dataService.post(
      `${this._signInApi}`,
      {
        email: signInParam.email,
        password: signInParam.password
      },
      {}
    ).pipe(
      map((response: AuthData) => {
        //login considered successful if the response contains a identification Token
        localStorage.setItem(this.authenticatedUser, response.id);
        localStorage.setItem(this.tokenStorageKey, response.auth_token)
        localStorage.setItem(this.tokenExpirationDate,response.expires_in);
      })
    );
  }

  public getToken(): string {
      return localStorage.getItem(this.tokenStorageKey);
  }

  private getValidateToken(): Promise<boolean> {
    return new Promise<boolean>(((resolve) => {
      const token = this.getToken();
      if (token != null){
        resolve(true);
      } else {
        resolve(false);
      }
    }));
  }

  /**
   * method: checkEmail.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param emailParam `string`.
   */
  public checkEmail(emailParam: string) {
    return this._dataService.post(
      `${this._checkEmailApi}`,
      {
        email: emailParam
      },
      {});
  }

  public existEmail(emailParam: string): Observable<boolean> {
    return this._dataService.get(
      `${this._existEmailApi}`,
      {
        email: emailParam.toString()
      });
  }

  public confirmEmail(
    userId: string,
    code: string
  ) {
    return this._dataService.post(
      `${this._confirmEmailApi}`,
      {
        userId : userId,
        code: code 
      },
      {});
  }

  /**
   * method: resetPassword.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param resetPasswordParam `ResetPasswordVM`.
   */
  public resetPassword(resetPasswordParam: ResetPassword) {
    return this._dataService.post(
      `${this._resetPasswordApi}`,
      {
        ...resetPasswordParam
      },
      {});
  }

  public updatePassword(resetPasswordParam: UpdatePassword) {
    return this._dataService.put(
      `${this._updatePasswordApi}`,
      {
        ...resetPasswordParam
      },
      {});
  }

  public register(registerParam: Register) {
    return this._dataService.post(
      `${this._registerApi}`,
      {
        ...registerParam
      },
      {}
    ).pipe(
      map((response: AuthData) => {
        //login considered successful if the response contains a identification Token
        localStorage.setItem(this.authenticatedUser, response.id);
        localStorage.setItem(this.tokenStorageKey, response.auth_token)
        localStorage.setItem(this.tokenExpirationDate,response.expires_in);
      })
    );
  }

  /**
   * method: get.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param applicationUserIdParam `string`.
   * @returns `UserVM>`.
   */
  public get(applicationUserIdParam: string): Observable<User> {
    return this._dataService.get(
      `${this._getApi}`,
      {
        applicationUserId: applicationUserIdParam.toString()
      });
  }

  public updatePicture(updatePicture: UpdateProfilePicture){
    let picture = updatePicture.picture.replace('data:image/jpeg;base64,','');
    let idUser = updatePicture.idUser;
    return this._dataService.put(
      `${this._updatePicture}`,
      {
        picture,
        idUser
      },
      {});
  }

  public refreshToken(idUser: string) {
    return this._dataService.post(
      `${this._refreshToken}`,
      {
        id: idUser 
      },
      {}
    ).pipe(
      map((response: AuthData) => {
        //refresh Token
        localStorage.setItem(this.authenticatedUser, response.id);
        localStorage.setItem(this.tokenStorageKey, response.auth_token)
        localStorage.setItem(this.tokenExpirationDate, response.expires_in);
      })
    );
  }

  public getRoles(applicationUserIdParam: string){
    return this._dataService.get(
      `${this._getRolesApi}`,
      {
        userId: applicationUserIdParam.toString()
      }
    ).pipe(
      map((response: Response) => {
        localStorage.setItem('roles', response[0]);
      },
      (error: any) => {
        console.log("Error Roles");
        console.log(error)
      })
    );
  }

  public signOut() {
    localStorage.removeItem('authenticatedUser');
    localStorage.removeItem('token');
    localStorage.removeItem('userProfileId');
    localStorage.removeItem('roles');
    this.router.navigate(['welcome']);
  }
}
