import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Camera } from '@ionic-native/camera/ngx';
import { IonicModule } from '@ionic/angular';
import { TranslateModule } from '@ngx-translate/core';
import { ChooseNewPostCategoryComponent } from './components/choose-new-post-category/choose-new-post-category.component';
import { PostDetailsModalComponent } from './components/post-details-modal/post-details-modal.component';
import { PostsMainComponent } from './components/posts-main/posts-main.component';
import { TopPostsComponent } from './components/top-posts/top-posts.component';
import { PostsRoutingModule } from './posts-routing.module';
import { PostsComponent } from './posts.component';

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
