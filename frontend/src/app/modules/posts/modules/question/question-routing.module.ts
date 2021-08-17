import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CreateAnswersComponent } from "./create-answers/create-answers.component";
import { CreateQuestionComponent } from "./create-question/create-question.component";
import { CreateSettingsComponent } from "./create-settings/create-settings.component";
import { PreviewComponent } from "./preview/preview.component";
import { QuestionComponent } from "./question.component";

const routes: Routes = [
  {
    path: '',
    redirectTo: 'create'
  },
  {
    path: 'create',
    component: QuestionComponent
  }
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class QuestionRoutingModule { }
