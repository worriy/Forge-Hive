import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { TargetGroup } from 'src/app/shared/interfaces/groups/target-group';
import { CreationUtils } from 'src/app/shared/interfaces/posts/creation-utils';
import { CreateEvent } from 'src/app/shared/interfaces/posts/event/create-event';
import { Paging } from 'src/app/shared/interfaces/posts/paging';

@Component({
  selector: 'event-create-settings',
  templateUrl: './create-settings.component.html',
  styleUrls: ['./create-settings.component.scss'],
})
export class CreateSettingsComponent implements OnInit {
  minDate : string = new Date().toISOString();

  private paging: Paging; 

  //id of the public group
  private publicGroupId: string;

  //id of the current user
  private userProfileId: string;

  //the user's groups
  private myGroups: TargetGroup[];

  private settingsForm: FormGroup;
  
  @Input() vm: CreateEvent;
  @Input() utils: CreationUtils;

  @Output() changes = new EventEmitter<CreateEvent>();
  @Output() utilsChange = new EventEmitter<CreationUtils>();

  constructor(
    public _groupController: GroupControllerService,
    public _toastCtrl: ToastController,
    public _translateService: TranslateService,
    private formBuilder: FormBuilder
  ) {
    this.userProfileId = localStorage.getItem("userProfileId");
    this.paging = {
      step: 10,
      lastId: 0
    };
    
  }

  public ngOnInit(): void{
    this.settingsForm = this.formBuilder.group({
      publicationDate: [this.vm.publicationDate, Validators.required],
      endDate: [this.vm.endDate, Validators.required],
      targetGroups: [this.vm.targetGroupsIds, Validators.required]
    });
    this.onGetTargetGroups(this.userProfileId);

    this.settingsForm.valueChanges.subscribe(changes => this.emitChanges(changes));
  }

  emitChanges(changes: any) {
    if (!!changes.publicationDate)
      this.vm.publicationDate = changes.publicationDate;
    
    if (!!changes.endDate)
      this.vm.endDate = changes.endDate;

    if (!!changes.targetGroups) {
      this.vm.targetGroupsIds = changes.targetGroups;
      this.onCheckSelectedGroups();
      this.utilsChange.emit({...this.utils, targets: this.getTargetString()});
    }

    this.changes.emit(this.vm);
  }

  /**
   * method: onGetTargetGroups
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param userProfileId `string`.
   * @returns A `Subscription<any>`.
   */
  public onGetTargetGroups(userProfileId: string) {
    return this._groupController.listTargetableGroups(
      userProfileId,this.paging
    ).subscribe(data => {
      this.myGroups = data;

      //Test the groups to find the public group, retrieve its id and select it by default as target
      this.myGroups.forEach(element => 
        {
          if(element.name.toLowerCase() == "public")
            this.publicGroupId = element.id;
  
          this.vm.targetGroupsIds = [ this.publicGroupId ];
          this.settingsForm.controls.targetGroups.patchValue(this.vm.targetGroupsIds);
        });
    });
  }

  getTargetString(): string {
    var targetGroupsString: string = "";
    this.vm.targetGroupsIds.forEach(groupId => {
      targetGroupsString += this.myGroups.find(g => g.id == groupId).name + ", ";
    })
    targetGroupsString = targetGroupsString.slice(0, targetGroupsString.length - 2);
    return targetGroupsString;
  }

  /**
   * method: onCheckSettings
   * That method is a blank method.
   */
  // public onCheckSettings(): boolean {
  //   //get the actual date
  //   var now = new Date();
  //   now.setHours(0, 0, 0, 0);

  //   var publication = new Date(this.settings.publicationDate);
  //   var end = new Date(this.settings.endDate);

  //   var toastText;
    
  //   //if the publication date or the end date is before the actual date, return false and show a toast.
  //   if(publication.getTime() < now.getTime() || end.getTime() < now.getTime())
  //   {
  //     this._translateService.get('posts.cantPubBefTod').subscribe(res => toastText = res);

  //     let toast = this._toastCtrl.create({
  //       message: toastText,
  //       duration: 2500,
  //       position: 'middle'
  //     })
  //     toast.present();
  //     return false;
  //   }
  //   //if the expiration date is before the publication date, return false and show a toast
  //   if(end.getTime() < publication.getTime())
  //   {
  //     this._translateService.get('posts.cantExpBefPub').subscribe(res => toastText = res);

  //     let toast = this._toastCtrl.create({
  //       message: toastText,
  //       duration: 2500,
  //       position: 'middle'
  //     })
  //     toast.present();
  //     return false;
  //   }
  //   //if expiration date is the same than the publication date, return true but warn the user with a toast that its card won't last long.
  //   if(end.getTime() == publication.getTime())
  //   {
  //     this._translateService.get('posts.warnOneDayAvailable').subscribe(res => toastText = res);

  //     let toast = this._toastCtrl.create({
  //       message: toastText,
  //       duration: 2000,
  //       position: 'top'
  //     })
  //     toast.present();
  //     return true;
  //   }
  //   //if everything's alright, just return true.
  //   return true;
  // }

  /**
   * method: onCheckSelectedGroups
   * That method is a blank method.
   */
  public onCheckSelectedGroups() {
    //if no group is selected, select the public group.
    if(this.vm.targetGroupsIds.length == 0)
    {
      this.vm.targetGroupsIds = [
        this.publicGroupId
      ];
      return;
    }
    this.vm.targetGroupsIds.forEach(element => 
    {
      //if the public group is selected, then select only this one
      if(element == this.publicGroupId)
      {
        this.vm.targetGroupsIds = [
          this.publicGroupId
        ];
        this.changes.emit(this.vm);
        this.utilsChange.emit({...this.utils, targets: this.getTargetString()});
        this
        return;
      }
    });

  }
}
