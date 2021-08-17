import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PostsRoutingModule } from './posts-routing.module';
import { IonicModule } from '@ionic/angular';
import { PostsComponent } from './posts.component';
import { ChooseNewPostCategoryComponent } from './components/choose-new-post-category/choose-new-post-category.component';
import { TranslateModule } from '@ngx-translate/core';
import { TopPostsComponent } from './components/top-posts/top-posts.component';
import { Camera } from '@ionic-native/camera/ngx';
import { PostsMainComponent } from './components/posts-main/posts-main.component';
import { PostDetailsModalComponent } from './components/post-details-modal/post-details-modal.component';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    PostsMainComponent,
    PostsComponent,
    ChooseNewPostCategoryComponent,
    TopPostsComponent,
    PostDetailsModalComponent
  ],
  imports: [
    PostsRoutingModule,
    IonicModule,
    CommonModule,
    TranslateModule.forChild(),
    ReactiveFormsModule
  ],
  providers: [
    Camera
  ]
})
export class PostsModule { }
