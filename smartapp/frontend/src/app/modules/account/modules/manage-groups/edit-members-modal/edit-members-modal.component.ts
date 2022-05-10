import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ActionSheetController, AlertController, LoadingController, ModalController, ToastController } from '@ionic/angular';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { GroupManagementService } from 'src/app/services/group-management.service';
import { Group } from 'src/app/shared/interfaces/groups/group';
import { UpdateMembers } from 'src/app/shared/interfaces/groups/update-members';
import { User } from 'src/app/shared/interfaces/user/user';

@Component({
  selector: 'app-edit-members-modal',
  templateUrl: './edit-members-modal.component.html',
  styleUrls: ['./edit-members-modal.component.scss'],
})
export class EditMembersModalComponent implements OnInit {
  //The actual group general infos
  @Input() group: Group;

  //The list of the actual group's members
  private groupMembers: User[];

  //connected user Id
  private userProfileId: string;

  //List of Memebers
  members: Array<User>;

  membersAdd = new Array<string>();
  membersRemove = new Array<string>();

  checkMember: boolean;


  private users = new Array<User>();
  private selectedUser: {[id: string]: boolean} = {};
  public filteredUsers = new Array<User>();
  private _searchTerm = "";
  public set searchTerm(term: string) {
    this._searchTerm = term;
    this.onSearchUser();
  };

  constructor(
    public _actionSheetCtrl: ActionSheetController,
    public _groupController: GroupControllerService,
    public _userController: UserControllerService, 
    public router: Router, 
    public _loadingCtrl: LoadingController,
    public modalController: ModalController,
    public alertCtrl: AlertController,
    private groupManagementService: GroupManagementService
  ) {
    this.members = new Array<User>();
    this.userProfileId = this._userController.getUserProfileId();
  }

  public ngOnInit():void
  {    
    this.onGetUsers();
  }

  /**
   * method: onSearchUser
   * That method is a blank method.
   */
  public onSearchUser() {
    //filter the users list depending on the search term, if it appears in either one of the name or the email
    this.filteredUsers = this.users.filter(user => 
    {
      if(user.userProfileId != this.userProfileId){
        return user.firstname.toLowerCase().includes(this._searchTerm.toLowerCase()) 
        || user.lastname.toLowerCase().includes(this._searchTerm.toLowerCase()) 
        || user.email.toLowerCase().includes(this._searchTerm.toLowerCase());
      }      
    })
  }

  public onUpdateMember(event: any, userProfileId: string){
    this.selectedUser[userProfileId] = event.detail.checked;

    if (event.detail.checked) {
      if(this.membersRemove.find(m => m.valueOf() == userProfileId) ){
        this.membersRemove.splice(this.membersRemove.indexOf(userProfileId), 1);
      } else {
        this.membersAdd.push(userProfileId);
      }
    } else {
      if(this.groupMembers.find(u => u.userProfileId == userProfileId)){
        this.membersRemove.push(userProfileId);
      } 
      else if(this.membersAdd.find(m => m.valueOf() == userProfileId)) {
        this.membersAdd.splice(this.membersAdd.indexOf(userProfileId), 1);
      }
    }
  }

  /**
   * method: onSaveChanges
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param membersList `UpdateMembersVM`.
   * @returns A `Subscription<any>`.
   */
  public onSaveChanges() {
    const addMembersList: UpdateMembers = {
      userIds: this.membersAdd,
      groupId: this.group.idGroup
    };
    const removeMembersList: UpdateMembers = {
      userIds: this.membersRemove,
      groupId: this.group.idGroup
    };
    
    if(this.membersAdd.length >= 1 && this.membersRemove.length >= 1) {
      this.groupManagementService.addMembers(addMembersList).subscribe(() => {
        this.groupManagementService.removeMembers(removeMembersList).subscribe(() => {
          this.modalController.dismiss();
        });
      });   
    }else{
      if(this.membersAdd.length >= 1){
        this.groupManagementService.addMembers(addMembersList).subscribe(() => {
            this.modalController.dismiss();
          });
      }else{
        if(this.membersRemove.length >= 1 ){
          this.groupManagementService.removeMembers(removeMembersList).subscribe(res => {
            this.modalController.dismiss();
          });
        }
      }
    }
  }

  /**
   * method: onGetMembers
   * Calls the user API to retrieve all the users
   */
  public async onGetUsers() {
    //return AllUsers from backend  
    await this._userController.list().toPromise().then(users =>
      {
        this.users = users;
        this.filteredUsers = users;

        //Retrieve this group's members
        this._groupController.getMembers(this.group.idGroup ).subscribe(groupMembers => 
        {
          //retrieve the members of the group
          this.groupMembers = groupMembers;

          this.groupMembers.forEach(member => {
            this.selectedUser[member.userProfileId] = true;
          });          
        },
        error =>
        {  
          window.alert(error);
        });
      },
      error =>
      {  
        window.alert(error);
      });
    
  }

  /**
   * method: onCancel
   * That method is a blank method.
   */
  public onCancel() {
    this.modalController.dismiss();
  }
}
