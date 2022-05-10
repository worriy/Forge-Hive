import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ChooseNewPostCategoryComponent } from "./components/choose-new-post-category/choose-new-post-category.component";
import { PostsMainComponent } from "./components/posts-main/posts-main.component";
import { PostsComponent } from "./posts.component";


const routes: Routes = [
  {
    path: '',
    component: PostsComponent,
    children: [
      {
        path: '',
        redirectTo: 'main'
      },
      {
        path: 'main',
        component: PostsMainComponent
      },
      {
        path: 'newPost',
        component: ChooseNewPostCategoryComponent,
      },
      {
        path: 'event',
        loadChildren: () => import('./modules/event/event.module').then(m => m.EventModule)
      },
      {
        path: 'idea',
        loadChildren: () => import('./modules/idea/idea.module').then(m => m.IdeaModule)
      },
      {
        path: 'question',
        loadChildren: () => import('./modules/question/question.module').then(m => m.QuestionModule)
      },
      {
        path: 'quote',
        loadChildren: () => import('./modules/quote/quote.module').then(m => m.QuoteModule)
      },
      {
        path: 'suggestion',
        loadChildren: () => import('./modules/suggestion/suggestion.module').then(m => m.SuggestionModule)
      }
      
    ]
  },
  {
    path: '',
    redirectTo: 'main'
  }
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class PostsRoutingModule { }
