import { Injectable } from '@angular/core';
import { Observable, from, BehaviorSubject} from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { Storage } from '@ionic/storage';
import { DataService } from './data.service';
import { FullUser } from 'src/app/shared/interfaces/user/full-user';
import { Register } from 'src/app/shared/interfaces/user/register';
import { SignIn } from 'src/app/shared/interfaces/user/sign-in';
import { UpdatePassword } from 'src/app/shared/interfaces/user/update-password';
import { UpdateProfilePicture } from 'src/app/shared/interfaces/user/update-profile-picture';
import { User } from 'src/app/shared/interfaces/user/user';
import { UserUpdate } from 'src/app/shared/interfaces/user/user-update';
import { AuthData } from 'src/app/shared/interfaces/user/auth-data';
import { Router } from '@angular/router';

export const USER_INFO = "__user";

@Injectable({
  providedIn: 'root'
})
export class UserControllerService {
  public authApi = '/api/auth';
  public userApi = '/api/user';
  
  public authenticationState = new BehaviorSubject(null);
  private tokenStorageKey = 'token';
  private tokenExpirationDate = 'tokenExpirationDate';
  private userProfileId = 'userProfileId';

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

  public get(userProfileId: string) {
    return this._dataService.get(
      `${this.userApi}/${userProfileId}`,
      {}).pipe(
        map((response: FullUser) => {
          localStorage.setItem(USER_INFO, JSON.stringify(response));
        })
      );
  }

  public getProfilPicture(userProfileId: string) {
    return this._dataService.get(
      `${this.userApi}/GetPicture/${userProfileId}`,
      {});
  }

  public list(): Observable<Array<User>> {
    return this._dataService.get(
      `${this.userApi}`,
      {});
  }

  public update(user: UserUpdate) {
    return this._dataService.put(
      `${this.userApi}`,
      {
        ...user
      },
      {})
      .pipe(
        map((response: AuthData) => {
          // Update localStorage of USER_INFO
          this.updateFullInfo(user);
        })
      );;
  }

  public updatePicture(updatePicture: UpdateProfilePicture){
    updatePicture.picture = updatePicture.picture.replace('data:image/jpeg;base64,','');
    return this._dataService.put(
      `${this.userApi}/updatePicture`,
      {
        ...updatePicture
      },
      {});
  }

  public updatePassword(resetPasswordParam: UpdatePassword) {
    return this._dataService.put(
      `${this.userApi}/updatePassword`,
      {
        ...resetPasswordParam
      },
      {});
  }

  public register(registerParam: Register) {
    return this._dataService.post(
      `${this.authApi}/register`,
      {
        ...registerParam
      },
      {}
    ).pipe(
      map((response: AuthData) => {
        //login considered successful if the response contains a identification Token
        localStorage.setItem(this.userProfileId, response.id);
        localStorage.setItem(this.tokenStorageKey, response.auth_token)
        localStorage.setItem(this.tokenExpirationDate,response.expires_in);
      })
    );
  }

  public existEmail(emailParam: string): Observable<boolean> {
    return this._dataService.get(
      `${this.authApi}/existEmail`,
      {
        email: emailParam.toString()
      });
  }

  public signIn(signInParam: SignIn) {
    return this._dataService.post(
      `${this.authApi}/login`,
      {
        email: signInParam.email,
        password: signInParam.password
      },
      {}
    ).pipe(
      map((response: AuthData) => {
        //login considered successful if the response contains a identification Token
        localStorage.setItem(this.userProfileId, response.id);
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

  public signOut() {
    localStorage.removeItem(this.tokenStorageKey);
    localStorage.removeItem(this.userProfileId);
    localStorage.removeItem(this.tokenExpirationDate);
    localStorage.removeItem(USER_INFO);
    this.router.navigate(['welcome']);
  }

  public getFullInfoUser() : FullUser{
    const user = localStorage.getItem(USER_INFO);
    return JSON.parse(user);
  }

  public updateFullInfo(user :UserUpdate ) {

    const fullInfo : FullUser = {
      userProfileId: user.id,
      email: user.email,
      firstname: user.firstName,
      lastname: user.lastName,
      phoneNumber: user.phoneNumber,
      city: user.city,
      country: user.country,
      department: user.department,
      job: user.job
    };
    localStorage.setItem(USER_INFO, JSON.stringify(fullInfo));
  }

  public getUserProfileId(): string {
    const user = localStorage.getItem(USER_INFO);
    if (!JSON.parse(user)){
      const userProfileId = localStorage.getItem(this.userProfileId);
      return userProfileId;
      

    }
    return JSON.parse(user).userProfileId;
  }

}
