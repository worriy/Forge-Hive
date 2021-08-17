import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AccountComponent } from "./account.component";
import { ChangePasswordComponent } from "./components/change-password/change-password.component";
import { EditProfileComponent } from "./components/edit-profile/edit-profile.component";

const routes: Routes = [
  {
    path: 'main',
    component: AccountComponent,
  },
  {
    path: 'edit-profile',
    component: EditProfileComponent
  },
  {
    path: 'change-password',
    component: ChangePasswordComponent
  },
  {
    path: 'manage-groups',
    loadChildren: () => import('./modules/manage-groups/manage-groups.module').then(m => m.ManageGroupsModule)
  },
  {
    path: 'settings',
    loadChildren: () => import('./modules/settings/settings.module').then(m => m.SettingsModule)
  },
  {
    path: '',
    redirectTo: 'main'
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class AccountRoutingModule { }