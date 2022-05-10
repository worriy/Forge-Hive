import { Location } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ModalController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { GroupManagementService } from 'src/app/services/group-management.service';
import { CreateGroup } from 'src/app/shared/interfaces/groups/create-group';

@Component({
  selector: 'app-create-group-modal',
  templateUrl: './create-group-modal.component.html',
  styleUrls: ['./create-group-modal.component.scss'],
})
export class CreateGroupModalComponent implements OnInit {
  public groupForm: FormGroup;

  private userProfileId: string;

  constructor(
    public _groupController: GroupControllerService,
    public router: Router,
    public modalController: ModalController, 
    public formBuilder: FormBuilder,
    public _translateService: TranslateService,
    private groupManagementService: GroupManagementService,
    public _userController: UserControllerService
  ) {
    this.userProfileId = this._userController.getUserProfileId();
  }

  ngOnInit() {
    this.groupForm = this.formBuilder.group(
    {
      name:  ['', Validators.compose([Validators.required, Validators.minLength(4), Validators.maxLength(30)])],
      city: ['', Validators.compose([Validators.required])],
      country: ['', Validators.compose([Validators.required])]
    });
  }

  /**
   * method: onCancel
   * That method is a blank method.
   */
  public onCancel() {
    this.modalController.dismiss();
    return false;
  }


  /**
   * method: onSave
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param group `CreateGroupVM`.
   * @returns A `Subscription<any>`.
   */
  public async onSave() {
    if (this.groupForm.invalid) {
      return;
    }

    let groupvm: CreateGroup = {
      ...this.groupForm.value,
      createdbyId: this.userProfileId
    };
    this.groupManagementService.create(groupvm).subscribe(() => this.modalController.dismiss());    
  }

}
