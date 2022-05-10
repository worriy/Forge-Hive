import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { HighlightsComponent } from "./highlights.component";

const routes: Routes = [
  {
    path: '',
    component: HighlightsComponent
  }
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class HighlightsRoutingModule { }