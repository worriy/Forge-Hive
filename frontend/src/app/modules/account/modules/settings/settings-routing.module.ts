import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { LanguagesComponent } from "./languages/languages.component";
import { NotificationsComponent } from "./notifications/notifications.component";
import { SettingsComponent } from "./settings.component";

const routes: Routes = [
  {
    path: '',
    component: SettingsComponent,
    children: [
      {
        path: 'languages',
        component: LanguagesComponent
      },
      {
        path: 'notifications',
        component: NotificationsComponent
      }
    ]
  }
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class SettingsRoutingModule { }