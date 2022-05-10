import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { TabsComponent } from "./tabs.component";

const routes: Routes = [
  {
    path: 'tabs',
    component: TabsComponent,
    children: [
      {
        path: 'flow',
        loadChildren: () => import('../flow/flow.module').then(m => m.FlowModule)
      },
      {
        path: 'highlights',
        loadChildren: () => import('../highlights/highlights.module').then(m => m.HighlightsModule)
      },
      {
        path: 'posts',
        loadChildren: () => import('../posts/posts.module').then(m => m.PostsModule)
      },
      {
        path: 'account',
        loadChildren: () => import('../account/account.module').then(m => m.AccountModule)
      },
      {
        path: '',
        redirectTo: '/tabs/flow',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '',
    redirectTo: '/tabs/flow',
    pathMatch: 'full'
  }
];


@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class TabsRoutingModule { }