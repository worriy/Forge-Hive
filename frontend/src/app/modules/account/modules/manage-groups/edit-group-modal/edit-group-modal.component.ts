import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ModalController, ToastController } from '@ionic/angular';
import { Store } from '@ngrx/store';
import { TranslateService } from '@ngx-translate/core';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { GroupManagementService } from 'src/app/services/group-management.service';
import { EditGroup } from 'src/app/shared/interfaces/groups/edit-group';
import { Group } from 'src/app/shared/interfaces/groups/group';

@Component({
  selector: 'app-edit-group-modal',
  templateUrl: './edit-group-modal.component.html',
  styleUrls: ['./edit-group-modal.component.scss'],
})
export class EditGroupModalComponent implements OnInit {
  @Input() group: Group;

  public groupForm: FormGroup;

  constructor(
    public _groupController: GroupControllerService,
    public router: Router,
    public modalController: ModalController,
    public formBuilder: FormBuilder,
    public _translateService: TranslateService,
    private groupManagementService: GroupManagementService
  ) 
  {
  }

  ngOnInit() {
    console.log(this.group);
    this.groupForm = this.formBuilder.group(
    {
      name: [this.group.name, Validators.compose([Validators.required, Validators.minLength(4), Validators.maxLength(30)])],
      city: [this.group.city, Validators.compose([Validators.required, Validators.pattern('[a-zA-Z]*')])],
      country: [this.group.country, Validators.compose([Validators.required, Validators.pattern('[a-zA-Z]*')])]
    });
  }

  /**
   * method: onCancel
   * That method is a blank method.
   */
  public onCancel() {
    // TO DO
    this.modalController.dismiss(false);
  }

  /**
   * method: onSaveChanges
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param group `EditGroupVM`.
   * @returns A `Subscription<any>`.
   */
  public onSaveChanges() {
    if (this.groupForm.invalid) {
      return;
    }

    const editGroup: EditGroup = {
      ...this.groupForm.value,
      createdbyId: this.group.createdbyId,
      id: this.group.idGroup
    };

    this.groupManagementService.update(editGroup).subscribe(() => {
      this.modalController.dismiss();
    });
  }
}
