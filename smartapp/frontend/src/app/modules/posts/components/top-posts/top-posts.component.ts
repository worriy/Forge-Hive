import { Component, EventEmitter, OnInit, Output, Input } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingController, ModalController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { PostsControllerService } from 'src/app/services/api/posts-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { PostDetailsModalComponent } from '../post-details-modal/post-details-modal.component';
import {Observable} from 'rxjs';

@Component({
  selector: 'posts-top-posts',
  templateUrl: './top-posts.component.html',
  styleUrls: ['./top-posts.component.scss'],
})
export class TopPostsComponent implements OnInit {
  //the current user's Id
  userProfileId: any;

  //the list of top posts
  topPosts: any;

  @Output()
  deleted: EventEmitter<String> = new EventEmitter<String>();

  @Input() events: Observable<string>;

  public loading;

  constructor(
    public _postsController: PostsControllerService,
    public router: Router,
    public _modalCtrl: ModalController,
    private _translateService: TranslateService,
    private _loadingCtrl: LoadingController,
    public _userController: UserControllerService
  ) {
    
  }

  async ngOnInit() {
    this.userProfileId = this._userController.getUserProfileId();

    this.loading = await this._loadingCtrl.create({
      spinner: 'crescent',
      showBackdrop: true,
      backdropDismiss: true
    });
    this.loading.present();

    this.topPosts = this.onGetTopPosts(this.userProfileId);

    if (this.events){
      this.events.subscribe((postId) => {
        this.topPosts.splice(this.topPosts.findIndex(p => p.id == postId),1);
      });
    }
    
  }

  public onGetTopPosts(userProfileId: string) {
    return this._postsController.getTopPosts(userProfileId).subscribe(res =>{
      this.topPosts = res;
      this.loading.dismiss();
    }, error => {
      console.log(error);
      this.loading.dismiss();
    }
    )
      
  }

  /**
   * Show details page for a post
   */
  public async onShowPostDetailsModal(postId: number,url: string) {
    let postDetailsModal = await this._modalCtrl.create({
      component: PostDetailsModalComponent, 
      componentProps: { 'postId': postId , 'picture': url}
    });
    postDetailsModal.present();

    const { data } = await postDetailsModal.onWillDismiss();
    if(data != null)
    {
      var index = this.topPosts.findIndex(p => p.id == data.post.id);
        if(index != null && data.action == "delete") {
          this.topPosts.splice(index, 1);
          this.deleted.emit(data.post.id);
        }
    }
  }


  /**
   * * @param publication: Date
   * * @param end : Date
   * Return the card's status depending on its publication and end dates
   */
  public onGetStatus(publication: Date, end: Date)
  {
    var endDate = new Date(end);
    var pubDate = new Date(publication);
    var now = new Date();
    
    var status
    //if the publication date if after now, the post is unpublished
    if(pubDate.getTime() > now.getTime()){
      this._translateService.get("posts.unpublished").subscribe(res => status = res);
      return status;
    }
    //if the end date is before now, the post is closed
    if(endDate.getTime() < now.getTime()){
      this._translateService.get("posts.closed").subscribe(res => status = res);
      return status;
    }
    //if between those, the post is published
    else{
      this._translateService.get("posts.published").subscribe(res => status = res);
      return status;
    }
  }
}
