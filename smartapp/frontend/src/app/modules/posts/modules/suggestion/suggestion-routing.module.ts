import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CreateQuestionComponent } from "./create-question/create-question.component";
import { CreateSettingsComponent } from "./create-settings/create-settings.component";
import { PreviewComponent } from "./preview/preview.component";
import { SuggestionComponent } from "./suggestion.component";

const routes: Routes = [
  {
    path: '',
    redirectTo: 'create'
  },
  {
    path: 'create',
    component: SuggestionComponent
  }
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class SuggestionRoutingModule { }
