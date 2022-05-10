import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { LanguagesComponent } from "./languages/languages.component";
import { SettingsComponent } from "./settings.component";

const routes: Routes = [
  {
    path: '',
    component: SettingsComponent,
    children: [
      {
        path: 'languages',
        component: LanguagesComponent
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