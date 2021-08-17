import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { FullUser } from 'src/app/shared/interfaces/user/full-user';
import { Storage } from '@ionic/storage';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
})
export class AccountComponent implements OnInit {
  public user: FullUser ; 

  constructor(
    public _userController: UserControllerService,
    public router: Router,
    private activatedRoute: ActivatedRoute,
    public storage: Storage
    ) {
    this.onGetUser();
  }

  ngOnInit() {

  }

  /**
   * method: onSignOut
   * That method is a blank method.
   */
  public onSignOut() {
    localStorage.removeItem('authenticatedUser');
    localStorage.removeItem('token');
    localStorage.removeItem('userProfileId');
    localStorage.removeItem('roles');
    this.router.navigate(['welcome']);
  }

  /**
   * method: onGetUser
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param userProfileId `string`.
   * @returns A `Subscription<any>`.
   */
  public onGetUser() {
    let currentUser = localStorage.getItem('authenticatedUser');
    return this._userController.getFullInfos(
      currentUser
    ).subscribe(res => {
      this.user = res;
    });
  }

  /**
   * method: onToEditProfile
   * That method is a navigation method.
   */
  public onToEditProfile() {
    this.router.navigate(['tabs/account/edit-profile'], { queryParams: { param: JSON.stringify(this.user) }});
  }

  /**
   * method: onToManageGroups
   * That method is a navigation method.
   */
  public onToManageGroups() {
    this.router.navigate(['tabs/account/manage-groups']);
  }

  /**
   * method: onToSettings
   * That method is a navigation method.
   */
  public onToSettings() {
    this.router.navigate(['tabs/account/settings']);
  }

}
