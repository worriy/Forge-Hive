import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CreateQuestionComponent } from "./create-question/create-question.component";
import { CreateSettingsComponent } from "./create-settings/create-settings.component";
import { IdeaComponent } from "./idea.component";
import { PreviewComponent } from "./preview/preview.component";

const routes: Routes = [
  {
    path: '',
    redirectTo: 'create'
  },
  {
    path: 'create',
    component: IdeaComponent
  }
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class IdeaRoutingModule { }
