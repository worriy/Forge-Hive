import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ActionSheetController, AlertController, ModalController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PostsControllerService } from 'src/app/services/api/posts-controller.service';
import { EditIdea } from 'src/app/shared/interfaces/posts/idea/edit-idea';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { Post } from 'src/app/shared/interfaces/posts/post';
import { PostDetailsModalComponent } from './components/post-details-modal/post-details-modal.component';
import { EditEventModalComponent } from './modules/event/edit-event-modal/edit-event-modal.component';
import { EditIdeaModalComponent } from './modules/idea/edit-idea-modal/edit-idea-modal.component';
import { EditQuestionModalComponent } from './modules/question/edit-question-modal/edit-question-modal.component';
import { EditQuoteModalComponent } from './modules/quote/edit-quote-modal/edit-quote-modal.component';
import { EditSuggestionModalComponent } from './modules/suggestion/edit-suggestion-modal/edit-suggestion-modal.component';
import { EditSurveyModalComponent } from './modules/survey/edit-survey-modal/edit-survey-modal.component';

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.scss'],
})
export class PostsComponent implements OnInit, OnDestroy {
  private destroy = new Subject<void>();

  // Get role of the User
  private role: string;
  //user's profile Id
  private userProfileId: any;
  private paging: any;
  //Posts list
  private posts: Post[] = [];

  private isAll: string;
  private isIdeas: string;
  private isQuestions: string;
  private isEvents: string;
  private isSurveys: string;
  private isQuotes: string;
  private isSuggestions: string;

  private translations: string[] = [];

  constructor(
    public _postsController: PostsControllerService,
    public router: Router,
    public _modalCtrl: ModalController,
    public _actionSheetCtrl: ActionSheetController,
    public _alertCtrl: AlertController,
    private _translateService: TranslateService,
    private activatedRoute: ActivatedRoute
  ) {
    this.userProfileId = localStorage.getItem('userProfileId');
    this.paging = {
      step: 10,
      page: 1
    };
    this.isAll = "is-active";
    this.isIdeas = "";
    this.isQuestions = "";
    this.isEvents = "";
    this.isSurveys = "";
    this.isQuotes = "";
    this.isSuggestions = "";

    this.role = localStorage.getItem('roles');
  }

  ngOnInit() {
    this.getTranslations();
    this.onGetLatestPosts(this.paging, this.userProfileId, false, "");
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  ngAfterViewInit(){
    this.onGetLatestPosts(this.paging, this.userProfileId, false, "");
    
  }

  ionSelected(){
    this.onGetLatestPosts(this.paging, this.userProfileId, false, "");
  }

  ionViewWillEnter(){
    this.onGetLatestPosts(this.paging, this.userProfileId, false, "");
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

  doInfinite(event)
  {
    this.paging.page ++;
    this.onGetLatestPosts(this.paging, this.userProfileId, true, event);
  }


  /**
   * method: onConfirmDelete
   * That method is a blank method.
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
   * method: onToChooseNewPostCategory
   * That method is a navigation method.
   */
  public onToChooseNewPostCategory() {
    this.router.navigate(['newPost'], { relativeTo: this.activatedRoute });
  }

  public async onGetLatestPosts(paging: any, userProfileId: string, isFirstLoad, event) {
  return await this._postsController.getLatestPosts(paging, userProfileId).subscribe(res => {
      
      
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
   * method: onDelete
   * That method is a blank method.
   */
  public onDelete(postId: string) {
    this._postsController.delete(postId);
    this.posts.splice(this.posts.findIndex(p => p.id == postId),1);
  }

  /**
   * method: onPresentOptionsMenu
   * That method is a blank method.
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
        role: 'cancel',
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
   * method: onShowEditPostModal
   * That method is a blank method.
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
      case "Survey":
        //Create the question creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditSurveyModalComponent, 
          componentProps: {post: post}
        });
        type = 'Survey';
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
        if(this.role == "Admin"){
          //Create the event creation process modal
          editPostModal = await this._modalCtrl.create({
            component: EditQuoteModalComponent, 
            componentProps: {post: post}
          });
          type = 'Quote';
        }else{
          console.log("nothing to do");
        }
        break;
      case "Suggestion":
      if(this.role == "Admin"){
        //Create the suggestion creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditSuggestionModalComponent,
          componentProps: {post: post}
        });
        type = 'Suggestion';
      }else{
        console.log("nothing to do");
      }
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
   * method: onShowPostDetailsModal
   * That method is a blank method.
  */
  public async onShowPostDetailsModal(postId: number) {
    let postDetailsModal = await this._modalCtrl.create({
      component: PostDetailsModalComponent, 
      componentProps: { 'post': postId }
    });
    postDetailsModal.present();

    const { data } = await postDetailsModal.onWillDismiss()
    if(data != null)
    {
      var index = this.posts.findIndex(p => p.id == data.post.id);
      if(data.action == "delete"){
        if(index != null)
          this.posts.splice(index, 1);
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
   * method: onShowPostDetailsModal
   * That method is a blank method.
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

  public onGoTo(type: string){

    switch(type)
    {
      case "All":
        this.isAll = "is-active";
        this.isIdeas = "";
        this.isQuestions = "";
        this.isSurveys = "";
        this.isEvents = "";
        this.isQuotes = "";
        this.isSuggestions = "";
        break;
      case "Idea":
        this.isAll = "";
        this.isIdeas = "is-active";
        this.isQuestions = "";
        this.isSurveys = "";
        this.isEvents = "";
        this.isQuotes = "";
        this.isSuggestions = "";
        break;
      case "Question":
        this.isAll = "";
        this.isIdeas = "";
        this.isQuestions = "is-active";
        this.isSurveys = "";
        this.isEvents = "";
        this.isQuotes = "";
        this.isSuggestions = "";
        break;
      case "Survey":
        this.isAll = "";
        this.isIdeas = "";
        this.isQuestions = "";
        this.isSurveys = "is-active";
        this.isEvents = "";
        this.isQuotes = "";
        this.isSuggestions = "";
        break;
      case "Event":
        this.isAll = "";
        this.isIdeas = "";
        this.isQuestions = "";
        this.isSurveys = "";
        this.isEvents = "is-active";
        this.isQuotes = "";
        this.isSuggestions = "";
        break;
      case "Quote":
        this.isAll = "";
        this.isIdeas = "";
        this.isQuestions = "";
        this.isSurveys = "";
        this.isEvents = "";
        this.isQuotes = "is-active";
        this.isSuggestions = "";
        break;
      case "Suggestion":
        this.isAll = "";
        this.isIdeas = "";
        this.isQuestions = "";
        this.isSurveys = "";
        this.isEvents = "";
        this.isQuotes = "";
        this.isSuggestions = "is-active";
        break;
      default: console.log("nothing to do");
        return;
    }
  }
}
