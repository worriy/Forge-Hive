import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingController, ModalController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { PostsControllerService } from 'src/app/services/api/posts-controller.service';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { PostDetailsModalComponent } from '../post-details-modal/post-details-modal.component';

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

  paging: Paging; 

  //event emitted when the user answer this card
  @Output()
  answered: EventEmitter<String> = new EventEmitter<String>();

  public loading;

  constructor(
    public _postsController: PostsControllerService,
    public router: Router,
    public _modalCtrl: ModalController,
    private _translateService: TranslateService,
    private _loadingCtrl: LoadingController,
  ) {
    
  }

  async ngOnInit() {
    this.userProfileId = localStorage.getItem('userProfileId');

    this.loading = await this._loadingCtrl.create({
      spinner: 'crescent',
      showBackdrop: true,
      backdropDismiss: true
    });
    this.loading.present();

    this.topPosts = this.onGetTopPosts(this.userProfileId);
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
   * method: onShowPostDetailsModal
   * That method is a blank method.
   */
  public async onShowPostDetailsModal(postId: number,url: string) {
    // TO DO
    let postDetailsModal = await this._modalCtrl.create({
      component: PostDetailsModalComponent, 
      componentProps: { 'postId': postId , 'picture': url}
    });
    postDetailsModal.present();

  }


  /**
   * method: getStatus
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
