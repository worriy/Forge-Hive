import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ActionSheetController, AlertController, LoadingController, ModalController, NavParams } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { GroupManagementService } from 'src/app/services/group-management.service';
import { Group } from 'src/app/shared/interfaces/groups/group';
import { UpdateMembers } from 'src/app/shared/interfaces/groups/update-members';
import { CreateGroupModalComponent } from './create-group-modal/create-group-modal.component';
import { EditGroupModalComponent } from './edit-group-modal/edit-group-modal.component';

@Component({
  selector: 'app-manage-groups',
  templateUrl: './manage-groups.component.html',
  styleUrls: ['./manage-groups.component.scss'],
})
export class ManageGroupsComponent implements OnInit, OnDestroy {
  private destroy = new Subject<void>();

  private  groups : any;
  private myGroups: Array<Group>;
  
  private userProfileId : string;
  public loading : any;

  LoadingView: boolean;

  private translations: string[] = [];

  constructor(
    public _navParam: NavParams, 
    public _actionSheetCtrl: ActionSheetController,
    public alertCtrl: AlertController,
    public _groupController: GroupControllerService, 
    public router: Router, 
    public modalCtrl: ModalController,
    public _translateService: TranslateService,
    private _loadingCtrl: LoadingController,
    private activatedRoute: ActivatedRoute,
    private groupManagementService: GroupManagementService,
    public _userController: UserControllerService
  ) {
  }

  ngOnInit() {
    this.getTranslations();
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  ngAfterViewInit(){
    this.userProfileId = this._userController.getUserProfileId();
    this.loading = this._loadingCtrl.create({
      spinner: 'crescent',
      showBackdrop: true,
      backdropDismiss: false
    });

    // Subscribe to groups lists from group management service
    this.groupManagementService.userCreatedGroups
      .pipe(takeUntil(this.destroy))
      .subscribe((groups: Group[]) => this.groups = groups);  

    // Subscribe to groups lists from group management service
    this.groupManagementService.userMemberGroups
    .pipe(takeUntil(this.destroy))
    .subscribe((groups: Group[]) => this.myGroups = groups);  
  }

  private async getTranslations() {
    this._translateService.stream([
      'commons.cancel',
      'commons.leave',
      'commons.delete',
      'commons.edit',
      'commons.no',
      'commons.yes',
      'manageGroups.deleteGroup',
      'manageGroups.deleteGroupMessage'
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

  /**
   * this method is called after a click in the view, it will open an Action Sheet dialogue that lets the user to choose from a set of options(Leave or Cancel)
   * method: onPresentOptionsLeaveGroupMenu
   * That method is a blank method.
   */
  public async onPresentOptionsLeaveGroupMenu(group : Group) {
    let actionSheet = await this._actionSheetCtrl.create({
      header: 'Options',
      buttons: [
      {
        text: this.translations['commons.cancel'],
        role: 'cancel',
        handler: () => {
        }
      },
      {
        text: this.translations['commons.leave'],
        role: 'leave',
        handler: () => {
          this.onConfirmLeave(group.idGroup);
        }
      }]
    });
    actionSheet.present();
  }

  /**
   * this method is called after a click in the view, it will open an Action Sheet dialogue that lets the user to choose from a set of options(Delete, Edit or Cancel)
   * method: onPresentOptionsDeleteGroupMenu
   * That method is a blank method.
   */
  public async onPresentOptionsDeleteGroupMenu(group: Group){
    let actionSheet = await this._actionSheetCtrl.create({
      header: group.name + ' options ' ,
      buttons: [
      {
        text: this.translations['commons.delete'],
        role: 'destructive',
        handler: () => {
          this.onConfirmDelete(group);
        }
      },
      {
        text: this.translations['commons.cancel'],
        role: 'cancel',
        handler: () => {
        }
      },
      {
        text: this.translations['commons.edit'],
        role: 'Update',
        handler: () => {
          this.onShowEditGroupModal(group);
        }
      }]
    });
    actionSheet.present();
  }

  /**
   * method: onShowCreateGroupModal
   * That method is a blank method.
   */
  public async onShowCreateGroupModal() {
    //Open create Group Modal 
    let createGroupModal = await this.modalCtrl.create({
      component: CreateGroupModalComponent
    });
    createGroupModal.present();
  }

  /**
   * method: onConfirmDelete
   * That method is a blank method.
   */
  public async onConfirmDelete(group : Group) {
    let alert = await this.alertCtrl.create({
      header: this.translations['manageGroups.deleteGroup'],
      message: this.translations['manageGroups.deleteGroupMessage'],
      buttons: [
        {
          text: this.translations['commons.no'],
          role: 'cancel',
          handler: () => {
          }
        },
        {
          text: this.translations['commons.yes'],
          handler: () => {
            this.onDelete(group.idGroup);
          }
        }
      ]
    });
    alert.present();
  }

  /**
   * method: onDelete
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param groupId `number`.
   * @returns A `Subscription<any>`.
   */
  public onDelete(groupId: string) {
    //Allows the real delete of a group
    this.groupManagementService.delete(groupId).subscribe();
  }

  /**
   * method: onShowEditGroupModal
   * That method is a blank method.
   */
  public async onShowEditGroupModal(group : any) {
    //Open a modal for updating  group informations 
    let editGroupModal = await this.modalCtrl.create( {
      component: EditGroupModalComponent, 
      componentProps: {group: group}
    });
    editGroupModal.present();
  }

  /**
   * method: onConfirmLeave
   * That method is a blank method.
   */
  public async onConfirmLeave(id: string) {
    let alert = await this.alertCtrl.create({
      header: this.translations['manageGroups.leaveGroupTitle'],
      message: this.translations['manageGroups.leaveGroupMessage'],
      buttons: [
        {
          text: this.translations['commons.no'],
          role: 'cancel',
          handler: () => {
          }
        },
        {
          text: this.translations['commons.yes'],
          handler: () => {
            let member = new Array<string>();
            member.push(this.userProfileId);
            this.onLeave(member,id);
          }
        }
      ]
    });
    alert.present();
  }

  /**
   * method: onLeave
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param userProfileId `number`.
   * @param groupId `number`.
   * @returns A `Subscription<any>`.
   */
  public onLeave(userProfileId: string[],groupId: string) {
    let members: UpdateMembers = {
      userIds: userProfileId,
      groupId
    };
    return this.groupManagementService.removeMembers(
      members
    ).subscribe();
  }

  /**
   * method: onToGroupDetails
   * That method is a navigation method.
   */
  public onToGroupDetails(group : Group) {
    this.router.navigate(['tabs/account/manage-groups/details'], { queryParams: { group: JSON.stringify(group) } });
  }

  /**
   * returns to the previous page
   */
  public onReturn()
  {
    this.router.navigate(['..'], { relativeTo: this.activatedRoute });
  }
}
