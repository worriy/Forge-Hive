import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SettingsRoutingModule } from './settings-routing.module';
import { TranslateModule } from '@ngx-translate/core';
import { SettingsComponent } from './settings.component';
import { NotificationsComponent } from './notifications/notifications.component';
import { LanguagesComponent } from './languages/languages.component';



@NgModule({
  declarations: [
    SettingsComponent,
    NotificationsComponent,
    LanguagesComponent
  ],
  imports: [
    SettingsRoutingModule,
    CommonModule,
    TranslateModule.forChild()
  ]
})
export class SettingsModule { }
