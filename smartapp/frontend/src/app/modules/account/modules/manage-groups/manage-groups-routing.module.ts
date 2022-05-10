import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { GroupDetailsComponent } from "./group-details/group-details.component";
import { ManageGroupsComponent } from "./manage-groups.component";

const routes: Routes = [
  {
    path: '',
    component: ManageGroupsComponent
  },
  {
    path: 'details',
    component: GroupDetailsComponent
  }
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class ManageGroupsRoutingModule { }