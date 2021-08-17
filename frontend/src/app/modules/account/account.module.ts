import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountRoutingModule } from './account-routing.module';
import { TranslateModule } from '@ngx-translate/core';
import { AccountComponent } from './account.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { EditProfileComponent } from './components/edit-profile/edit-profile.component';
import { IonicModule, NavParams } from '@ionic/angular';
import { Camera } from '@ionic-native/camera/ngx';
import { ReactiveFormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    AccountComponent,
    ChangePasswordComponent,
    EditProfileComponent
  ],
  imports: [
    AccountRoutingModule,
    CommonModule,
    IonicModule,
    TranslateModule.forChild(),
    ReactiveFormsModule
  ],
  providers: [
    NavParams,
    Camera
  ]
})
export class AccountModule { }
