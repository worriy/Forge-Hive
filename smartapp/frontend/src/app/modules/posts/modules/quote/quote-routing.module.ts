import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { QuoteComponent } from "./quote.component";

const routes: Routes = [
  {
    path: '',
    redirectTo: 'create'
  },
  {
    path: 'create',
    component: QuoteComponent
  }
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class QuoteRoutingModule { }
