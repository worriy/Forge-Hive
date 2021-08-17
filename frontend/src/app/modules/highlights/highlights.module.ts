import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HighlightsRoutingModule } from './highlights-routing.module';
import { HighlightsComponent } from './highlights.component';
import { BestContributorComponent } from './best-contributor/best-contributor.component';
import { BestPostComponent } from './best-post/best-post.component';
import { GeneralMoodComponent } from './general-mood/general-mood.component';
import { TopPostsComponent } from './top-posts/top-posts.component';
import { TopPostsCardModalComponent } from './top-posts-card-modal/top-posts-card-modal.component';
import { TranslateModule } from '@ngx-translate/core';



@NgModule({
  declarations: [
    HighlightsComponent,
    BestContributorComponent,
    BestPostComponent,
    GeneralMoodComponent,
    TopPostsComponent,
    TopPostsCardModalComponent,
  ],
  imports: [
    HighlightsRoutingModule,
    CommonModule,
    TranslateModule.forChild()
  ]
})
export class HighlightsModule { }
