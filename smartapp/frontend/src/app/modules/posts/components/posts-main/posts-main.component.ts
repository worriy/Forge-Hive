import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ActionSheetController, AlertController, ModalController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PostsControllerService } from 'src/app/services/api/posts-controller.service';
import { Post } from 'src/app/shared/interfaces/posts/post';
import { PostDetailsModalComponent } from '../post-details-modal/post-details-modal.component';
import { EditEventModalComponent } from '../../modules/event/edit-event-modal/edit-event-modal.component';
import { EditIdeaModalComponent } from '../../modules/idea/edit-idea-modal/edit-idea-modal.component';
import { EditQuestionModalComponent } from '../../modules/question/edit-question-modal/edit-question-modal.component';
import { EditQuoteModalComponent } from '../../modules/quote/edit-quote-modal/edit-quote-modal.component';
import { EditSuggestionModalComponent } from '../../modules/suggestion/edit-suggestion-modal/edit-suggestion-modal.component';
import { UserControllerService } from 'src/app/services/api/user-controller.service';

@Component({
  selector: 'app-posts-main',
  templateUrl: './posts-main.component.html',
  styleUrls: ['./posts-main.component.scss'],
})
export class PostsMainComponent implements OnInit {
  private destroy = new Subject<void>();

  //user's profile Id
  private userProfileId: any;
  //Posts list
  private posts: Post[] = [];

  public postType = "All";

  private translations: string[] = [];
  eventsSubject: Subject<string> = new Subject<string>();

  constructor(
    public _postsController: PostsControllerService,
    public router: Router,
    public _modalCtrl: ModalController,
    public _actionSheetCtrl: ActionSheetController,
    public _alertCtrl: AlertController,
    private _translateService: TranslateService,
    private activatedRoute: ActivatedRoute,
    public _userController: UserControllerService
  ) {
    this.userProfileId = this._userController.getUserProfileId();

  }

  ngOnInit() {
    this.getTranslations();
    this.onGetLatestPosts(this.userProfileId, false, "");
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  ngAfterViewInit(){
    this.onGetLatestPosts(this.userProfileId, false, "");  
  }

  ionSelected(){
    this.onGetLatestPosts(this.userProfileId, false, "");
  }

  ionViewWillEnter(){
    this.onGetLatestPosts(this.userProfileId, false, "");
  }

  getTranslations() {
    this._translateService.stream([
      'posts.deleteAlertTitle',
      'posts.deleteAlertMessage',
      'commons.yes',
      'commons.no',
      'commons.delete',
      'commons.cancel',
      'commons.edit',
      'posts.postOptions',
      'posts.unpublished',
      'posts.closed',
      'posts.published'
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }



  /**
   * Confirm post deletion
   */
  public async onConfirmDelete(postId: string) {
    let alert = await this._alertCtrl.create({
      header: this.translations['posts.deleteAlertTitle'],
      message: this.translations['posts.deleteAlertMessage'],
      buttons: [
        {
          text: this.translations['commons.no'],
          handler: () => {
          }
        },
        {
          text: this.translations['commons.yes'],
          handler: () => {
            this.onDelete(postId);
          }
        }
      ]
    });

    alert.present();
  }

  /**
   * Navigates to creation menu
   */
  public toChooseNewPostCategory() {
    this.router.navigate(['../newPost'], { relativeTo: this.activatedRoute });
  }

  public async onGetLatestPosts(userProfileId: string, isFirstLoad, event) {
    return await this._postsController.getLatestPosts(userProfileId).subscribe(res => {
      if (isFirstLoad) {
        event.target.complete();
        this.posts.push(...res);
        
      } else {
        this.posts = res;
      }
        
    }, error => {
      console.log(error);
    });
  }

  /**
   * Deletes a post
   */
  public onDelete(postId: string) {
    this._postsController.delete(postId);
    this.posts.splice(this.posts.findIndex(p => p.id == postId),1);
    this.eventsSubject.next(postId);
  }

  public deleteFromTopPost(postId){
    this.posts.splice(this.posts.findIndex(p => p.id == postId),1);
  }

  /**
   * Present options menu for post, if it is not yet published it can be edited
   */
  public async onPresentOptionsMenu(post: Post) {
    let buttons = [
      {
        text: this.translations['commons.delete'],
        role: 'destructive',
        handler: () => {
          this.onConfirmDelete(post.id);
        }
      },{
        text: this.translations['commons.cancel'],
        role: 'cancel',
        handler: () => {
        }
      }
    ];
    
    //if the post is not yet published, add the edit option
    var endDate = new Date(post.endDate);
    var pubDate = new Date(post.publicationDate);
    if(pubDate.getTime() > new Date().getTime() && !(endDate.getTime() < new Date().getTime())){
      var buttonEdit = {
        text: this.translations['commons.edit'],
        role: 'edit',
        handler: () => {
          this.onShowEditPostModal(post);
        }
      }
      buttons.push(buttonEdit);
    }

    let actionSheet = await this._actionSheetCtrl.create({
      header: this.translations['posts.postOptions'],
      buttons: buttons
    });
    actionSheet.present();
  }

  /**
   * Show edition modal depending on post type
   */
  public async onShowEditPostModal(post: Post) {

    let editPostModal: HTMLIonModalElement;
    let type: string;
    
    switch(post.type)
    {
      case "Idea":
        //Create the idea creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditIdeaModalComponent, 
          componentProps: {post: post}
        });
        type = 'Idea';
        break;
      case "Question":
        //Create the question creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditQuestionModalComponent, 
          componentProps: {post: post}
        });
        type = 'Question';
        break;
      case "Event":
        //Create the event creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditEventModalComponent, 
          componentProps: {post: post}
        });
        type = 'Event';
        break;
      case "Quote":
        //Create the event creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditQuoteModalComponent, 
          componentProps: {post: post}
        });
        type = 'Quote';
        break;
      case "Suggestion":
        //Create the suggestion creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditSuggestionModalComponent,
          componentProps: {post: post}
        });
        type = 'Suggestion';
      break;
      default: console.log("nothing to do");
        return;
    }

    editPostModal.present();

    const { data } = await editPostModal.onWillDismiss();
    if(data != null)
    {
      var index = this.posts.findIndex(p => p.id == data.post.id);
      if(data.action == "update"){
        let newPost: Post = {
          id: data.post.id,
          content: data.post.content,
          publicationDate: data.post.publicationDate,
          endDate: data.post.endDate,
          status: this.onGetStatus(data.post.publicationDate, data.post.endDate),
          type: type
        };
        if(index != null)
          this.posts.splice(index, 1, newPost);
      }
    }
  }

  /**
   * Show post details
  */
  public async onShowPostDetailsModal(post: Post) {
    if (post.type.toLowerCase() === "mood")
      return;

    let postDetailsModal = await this._modalCtrl.create({
      component: PostDetailsModalComponent, 
      componentProps: { 'postId': post.id }
    });
    postDetailsModal.present();

    const { data } = await postDetailsModal.onWillDismiss()
    if(data != null)
    {
      var index = this.posts.findIndex(p => p.id == data.post.id);
      if(data.action == "delete"){
        if(index != null)
          this.posts.splice(index, 1);
          this.eventsSubject.next(data.post.id);
      }
      else{
        if(data.action =="update"){
          let postUpdate: Post = {
            id: data.post.id,
            content: data.post.content,
            publicationDate: data.post.publicationDate,
            endDate: data.post.endDate,
            status: data.post.status,
            type: data.post.type
          };
          this.posts.splice(index,1,postUpdate);
        }
      }

    }
  }

  /**
   * Get Post status
   */
  public onGetStatus(publication: Date, end: Date){
    var endDate = new Date(end);
    var pubDate = new Date(publication);
    var now = new Date();
    
    //if the publication date if after now, the post is unpublished
    if(pubDate.getTime() > now.getTime() ){
      return this.translations['posts.unpublished'];
    }
    //if the end date is before now, the post is closed
    if(endDate.getTime() < now.getTime()){
      return this.translations['posts.closed'];
    }
    //if between those, the post is published
    else{
      return this.translations['posts.published'];
    }
  }
}
