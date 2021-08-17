import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ActionSheetController, AlertController, ModalController } from '@ionic/angular';
import { Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { GroupManagementService } from 'src/app/services/group-management.service';
import { Group } from 'src/app/shared/interfaces/groups/group';
import { UpdateMembers } from 'src/app/shared/interfaces/groups/update-members';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { User } from 'src/app/shared/interfaces/user/user';
import { EditMembersModalComponent } from '../edit-members-modal/edit-members-modal.component';

@Component({
  selector: 'app-group-details',
  templateUrl: './group-details.component.html',
  styleUrls: ['./group-details.component.scss'],
})
export class GroupDetailsComponent implements OnInit, OnDestroy {
  private destroy = new Subject<void>();

  public group : Group ;
  
  private userProfileId : string;
  public  Members : User[]; 
  private paging: Paging;

  private translations: string[] = [];

  constructor(
    public _actionSheetCtrl: ActionSheetController,
    public alertCtrl: AlertController, 
    public _groupController: GroupControllerService, 
    public router: Router,
    public modalCtrl: ModalController,
    public activatedRoute: ActivatedRoute,
    public _translateService: TranslateService,
    private groupManagementService: GroupManagementService
  ) {
    this.activatedRoute.queryParams.subscribe(params => {
      this.group = JSON.parse(params.group);
      console.log(params);
      console.log(this.group);
    });;
    this.userProfileId = localStorage.getItem('userProfileId');
    this.paging = {
      step: 10,
      lastId: 0
    };
    this.Members = new Array<User>();
  }

  ngOnInit() {
    this.getTranslations();
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  getTranslations() {
    this._translateService.stream([
      'commons.leave',
      'commons.remove',
      'commons.cancel',
      'commons.confirm',
      'manageGroups.removeUser',
      'manageGroups.removeGroupTitle',
      'manageGroups.removeGroupContent'
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

  public ionViewWillEnter():void{
    //call to retrieve members of the specified group 
    this.onGetMembers();
  }
  /**
   * method: onPresentOptionsMenu
   * That method is a blank method.
   */
  public async onPresentOptionsMenu(id: string, idUser: string) {
    //ActionSheet to remove a member (idUser) from group (id)
    let actionSheet = await this._actionSheetCtrl.create({
      header: this.translations['manageGroups.removeUser'],
      buttons: [
      {
        text: idUser == this.userProfileId ? 
          this.translations['commons.leave'] : 
          this.translations['commons.remove'],
        role: 'destructive',
        handler: () => {
          this.onConfirmRemove(id, idUser);
        }
      },
      {
        text: this.translations['commons.cancel'],
        role: 'cancel',
        handler: () => {
        }
      }]
    });

    actionSheet.present();
  }

  /**
   * method: onGetMembers
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param paging `PagingVM`.
   * @param groupId `number`.
   * @returns A `Subscription<any>`.
   */
  public onGetMembers() {
    //returns the list of groups for a user 
    this._groupController.getMembers( this.paging,this.group.idGroup ).subscribe(res => 
    {
      this.Members = res; 
    });
  }

  /**
   * method: onRemoveMember
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param userProfileId `number`.
   * @param groupId `number`.
   * @returns A `Subscription<any>`.
   */
  public onRemoveMember(groupId: string, userProfileId: string) {
    //Real remove of a member from a group
    let users: string[] = [];
    users.push(userProfileId);

    //remove the member from the group and reload the members list
    let member: UpdateMembers = {
      userIds: users,
      groupId
    };
    this.groupManagementService.removeMembers( member).subscribe(() =>
    {
      this.onGetMembers();
      if(this.userProfileId == userProfileId){
        //this.store.dispatch(new MyGroupsActions.DeleteMyGroup({idGroup: groupId}));
        this.router.navigate(['..'], { relativeTo: this.activatedRoute });
      }
        
    },
    error =>
    {
        window.alert(error);
    })
  }

  public onReturn(){
    let groupVM: Group = {
      ...this.group
    }; // ???
    
    //this.store.select(state => state.groups).dispatch(new GroupsActions.UpdateGroup(groupVM));
    this.router.navigate(['..'], { relativeTo: this.activatedRoute });
  }  

  /**
   * method: onConfirmRemove
   * That method is a blank method.
   */
  public async onConfirmRemove(id: string, idUser: string) {
    // TO DO
    //get the name of the user that needs to be removed
    //var removeUserName = this.Members.find(u => u.userProfileId == idUser).firstname + ' ' + this.Members.find(u => u.userProfileId == idUser).lastname;

    //show an alert depending on whether or not the user to remove is the current user or another
    let alert = await this.alertCtrl.create({
      header: this.translations['manageGroups.removeGroupTitle'],
      message: this.translations['manageGroups.removeGroupContent'],
      buttons: [
        {
          text: this.translations['commons.cancel'],
          handler: () => {
          }
        },
        {
          text: this.translations['commons.confirm'],
          handler: () => {
            this.onRemoveMember(id, idUser);
          }
        }]
    });

    alert.present();
  }

  /**
   * method: onShowAddMemberModal
   * That method is a blank method.
   */
  public async onShowAddMemberModal() {
    // TO DO
    // Open addMember Modal
    let addMemberModal = await this.modalCtrl.create({
      component: EditMembersModalComponent,
      componentProps: {group: this.group}
    });
    addMemberModal.present();

    //reload the member list when coming back
    addMemberModal.onWillDismiss().finally(() => 
    {
      this.onGetMembers();
    });
  }
}
