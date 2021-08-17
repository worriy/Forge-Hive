import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ActionSheetController, AlertController, LoadingController, ModalController, ToastController } from '@ionic/angular';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { GroupManagementService } from 'src/app/services/group-management.service';
import { Group } from 'src/app/shared/interfaces/groups/group';
import { UpdateMembers } from 'src/app/shared/interfaces/groups/update-members';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { User } from 'src/app/shared/interfaces/user/user';

@Component({
  selector: 'app-edit-members-modal',
  templateUrl: './edit-members-modal.component.html',
  styleUrls: ['./edit-members-modal.component.scss'],
})
export class EditMembersModalComponent implements OnInit {
  //search to filter the user list with
  searchTerm: string = "";

  //The actual group general infos
  @Input() group: Group;

  //The list of the actual group's members
  private groupMembers: User[];

  //List of all the users associated with a boolean determining if it is a member of the group or not
  private UsersStatus: {user: User, isMember: boolean}[];

  //same list than before but once filtered depending on the search term
  private filteredItems: {user: User, isMember: boolean}[];

  //connected user Id
  private userProfileId: string;

  //List of Memebers
  members: Array<User>;

  membersAdd = new Array<string>();
  membersRemove = new Array<string>();

  checkMember: boolean;

  private paging: Paging;

  private users = new Array<User>();

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
    this.userProfileId = localStorage.getItem('userProfileId');
    this.UsersStatus = []; 
    this.paging = {
      step: 10,
      lastId: 0
    };
  }

  public ngOnInit():void
  {    
    this.onGetUsers();
    this.onClearSearch();
  }

  /**
   * method: onSearchUser
   * That method is a blank method.
   */
  public onSearchUser() {
    //filter the users list depending on the search term, if it appears in either one of the name or the email
    this.filteredItems = this.UsersStatus.filter(userStatus => 
    {
      if(userStatus.user.userProfileId != this.userProfileId){
        return userStatus.user.firstname.toLowerCase().includes(this.searchTerm.toLowerCase()) 
        || userStatus.user.lastname.toLowerCase().includes(this.searchTerm.toLowerCase()) 
        || userStatus.user.email.toLowerCase().includes(this.searchTerm.toLowerCase());
      }      
    });
  }

  public onUpdateMember(userProfileId: string,checkMember: boolean){
    if(checkMember == true){
      this.membersAdd.push(userProfileId);
      if(this.membersRemove.find(m => m.valueOf() == userProfileId) ){
        this.membersRemove.splice(this.membersRemove.indexOf(userProfileId), 1);
      }
    }
    else{
      if(this.groupMembers.find(u => u.userProfileId == userProfileId)){
        this.membersRemove.push(userProfileId);
      }
        
      if(this.membersAdd.find(m => m.valueOf() == userProfileId)){
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
    await this._userController.list(this.paging).toPromise().then(users =>
      {
        this.users = users;
        //Retrieve this group's members
        this._groupController.getMembers( this.paging,this.group.idGroup ).subscribe(groupMembers => 
        {
          //retrieve the members of the group
          this.groupMembers = groupMembers;

          //Fill the usersStatus Array depending on their presence in the group or not
          users.forEach(user => 
          {
            if(groupMembers.find(u => u.userProfileId == user.userProfileId))
              this.UsersStatus.push({user: user, isMember: true});
              
            else
              this.UsersStatus.push({user: user, isMember: false});
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
   * method: onClearSearch
   * That method is a blank method.
   */
  public onClearSearch() {
    // TO DO
    this.searchTerm = "";  
  }

  /**
   * method: onCancel
   * That method is a blank method.
   */
  public onCancel() {
    this.modalController.dismiss();
  }
}
