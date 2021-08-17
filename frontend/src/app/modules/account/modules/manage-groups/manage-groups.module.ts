import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManageGroupsRoutingModule } from './manage-groups-routing.module';
import { TranslateModule } from '@ngx-translate/core';
import { ManageGroupsComponent } from './manage-groups.component';
import { GroupDetailsComponent } from './group-details/group-details.component';
import { CreateGroupModalComponent } from './create-group-modal/create-group-modal.component';
import { EditGroupModalComponent } from './edit-group-modal/edit-group-modal.component';
import { EditMembersModalComponent } from './edit-members-modal/edit-members-modal.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';



@NgModule({
  declarations: [
    ManageGroupsComponent,
    GroupDetailsComponent,
    CreateGroupModalComponent,
    EditGroupModalComponent,
    EditMembersModalComponent
  ],
  imports: [
    ManageGroupsRoutingModule,
    CommonModule,
    TranslateModule.forChild(),
    IonicModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class ManageGroupsModule { }
