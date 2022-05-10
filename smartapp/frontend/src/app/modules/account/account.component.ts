import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { FullUser } from 'src/app/shared/interfaces/user/full-user';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
})
export class AccountComponent implements OnInit {
  public user: FullUser ; 
  public profilPicture: string;

  constructor(
    public _userController: UserControllerService,
    public router: Router) {
  }

  ngOnInit() {
    this.getUser();
    this.getProfilPicture();
  }

  ionViewWillEnter(){
    this.getUser();
    this.getProfilPicture();
  }

  public signOut() {
    this._userController.signOut();
  }

  public getUser() {
    this.user = this._userController.getFullInfoUser();
  }

  public getProfilPicture(){
    const userProfileId = this._userController.getUserProfileId();
    this._userController.getProfilPicture(userProfileId)
      .subscribe(res => {
        this.profilPicture = res.picture
      });
  }

  public toEditProfile() {
    this.router.navigate(['tabs/account/edit-profile'], { queryParams: { param: JSON.stringify(this.user) }});
  }

  public toManageGroups() {
    this.router.navigate(['tabs/account/manage-groups']);
  }

  public toSettings() {
    this.router.navigate(['tabs/account/settings']);
  }
}
