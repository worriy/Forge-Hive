import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActionSheetController, AlertController, LoadingController, ModalController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { QuoteControllerService } from 'src/app/services/api/quote-controller.service';
import { TargetGroup } from 'src/app/shared/interfaces/groups/target-group';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { Post } from 'src/app/shared/interfaces/posts/post';
import { EditQuote } from 'src/app/shared/interfaces/posts/quote/edit-quote';
import { EditableQuote } from 'src/app/shared/interfaces/posts/quote/editable-quote';

@Component({
  selector: 'app-edit-quote-modal',
  templateUrl: './edit-quote-modal.component.html',
  styleUrls: ['./edit-quote-modal.component.scss'],
})
export class EditQuoteModalComponent implements OnInit {

  private destroy = new Subject<void>();

  @Input() post: Post;
  editablePost: EditableQuote;
  minDate : string = new Date().toISOString();

  private paging: Paging;

  //the current user's id
  userProfileId: any;

  //the current user's groups
  myGroups: TargetGroup[];

  //the public group's id
  publicGroupId: string;

  //post's settings
  settings = {
    publicationDate: new Date().toISOString(),
    endDate: new Date().toISOString(),
    targetGroupIds: new Array<string>()
  };

  defaultPictureStyle: string;
  userPictureStyle: string;

  defaultPicture: string;
  postPicture: string;

  smallImg: any;
  pictureSelected: boolean;

  private translations: string[] = [];

  public quoteForm: FormGroup;

  constructor(
    public _quoteController: QuoteControllerService,
    public _groupController: GroupControllerService,
    public modalCtrl: ModalController,
    public _toastCtrl: ToastController,
    public _loadingCtrl: LoadingController,
    public _alertCtrl: AlertController,
    public _translateService: TranslateService,
    private formBuilder: FormBuilder
  ) {
    this.userProfileId = localStorage.getItem('userProfileId');
    this.paging = {
      step: 10,
      lastId: 0
    };
    
    this.pictureSelected = false;
  }

  

  public async ngOnInit() {
    this.getTranslations();
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //Add 'implements OnInit' to the class.

    //launch a loading screen while loading the posts details
    let loading = await this._loadingCtrl.create({
      spinner: 'crescent',
      message: this.translations['commons.loadingPleaseWait'],
      showBackdrop: false
    });
    loading.present();

    /*this._postController.getDefaultPicture().subscribe(picture => {
      this.defaultPicture = "data:image/jpeg;base64," + picture.url;
    })*/

    this.onGetTargetableGroups().then(() => 
    {
      //get all the informations about the post
      this._quoteController.getEditableQuote(this.post.id).subscribe(data => 
      {
        this.editablePost = data;
        this.postPicture = "data:image/jpeg;base64," + data.picture;
        if (!this.editablePost.targetGroupsIds)
          this.editablePost.targetGroupsIds = [ this.publicGroupId ];
        // Fix to display directly group name in select (weird string lowercasing happening somewhere)
        const groups = this.editablePost.targetGroupsIds.map(t => t.toUpperCase());
        this.editablePost.targetGroupsIds = [ ...groups ];
        //filling the targetGroups setting with informations from the post
        this.settings.targetGroupIds = new Array<string>();
        this.editablePost.targetGroupsIds.forEach(element => 
        {
          this.settings.targetGroupIds.push(element);
        });

        //settings the publication and end date in the date picker
        this.settings.publicationDate = new Date(this.editablePost.publicationDate).toISOString();  
        this.settings.endDate = new Date(this.editablePost.endDate).toISOString();
        this.quoteForm = this.formBuilder.group({
          content: [this.post.content, Validators.compose([Validators.required, Validators.maxLength(140)])],
          publicationDate: [this.post.publicationDate, Validators.required],
          endDate: [this.post.endDate, Validators.required],
          targetGroups: [this.editablePost.targetGroupsIds, Validators.required]
        });
        this.quoteForm.controls.targetGroups.patchValue(this.editablePost.targetGroupsIds);
        //closing the loader when retrieving is done.
        loading.dismiss();
      },
      error => {
        loading.dismiss();
      });
    });

    

    
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  private async getTranslations() {
    this._translateService.stream([
      'commons.no',
      'commons.yes',
      'commons.saving',
      'commons.loadingPleaseWait',
      'posts.publishPost',
      'posts.publishPostMessage',
      'posts.cantPubBefTod',
      'posts.cantExpBefPub',
      'posts.warnOneDayAvailable',
      'idea.takePicture',
      'idea.fromGallery',
      'idea.compressing'
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

  /**
   * method: onGetTargetableGroups
   * That method is a blank method.
   */
  public async onGetTargetableGroups() {
    // TO DO
    return this._groupController.listTargetableGroups(this.userProfileId, this.paging).toPromise().then(data => 
      {
        this.myGroups = data;
        
        //Search for the public group to retrieve its id
        this.myGroups.forEach(element => 
        {
          //if its the public group, memorize its id
          if(element.name.toLowerCase() == "public")
            this.publicGroupId = element.id;
        });
      });
  }

  /**
   * method: onSaveChanges
   * That method is a blank method.
   */

  public async onCheckInformation(){
    // Check that all the modifications are correct
    if(!this.onCheckDates(new Date(this.settings.publicationDate), new Date(this.settings.endDate)))
      return;

    //check if the date changed (toISOString sets date in UTC format)
    if(!(new Date(this.editablePost.publicationDate).toISOString() == this.settings.publicationDate))
      this.editablePost.publicationDate = new Date(this.settings.publicationDate);
    if(!(new Date(this.editablePost.endDate).toISOString() == this.settings.endDate))
    this.editablePost.endDate = new Date(this.settings.endDate);

    //if the edited publication date is today, ask the user confirmation before publishing the post
    if(new Date(this.editablePost.publicationDate) == new Date()){
  
      let alert = await this._alertCtrl.create({
        header: this.translations['posts.publishPost'],
        message: this.translations['posts.publishPostMessage'],
        buttons: [
          {
            text: this.translations['commons.no'],
            handler: () => {
              return;
            }
          },
          {
            text: this.translations['commons.yes'],
            handler: () => {
              this.onSaveChanges();
            }
          }
        ]
      });
      alert.present();
    }
    else{
      this.onSaveChanges()
    }
    
  }
  
  public async onSaveChanges() {
    if (this.quoteForm.invalid)
      return;
    // Save the changes if the changes are correct
    let loading = await this._loadingCtrl.create({
      spinner: 'crescent',
      message: this.translations['commons.saving'],
      showBackdrop: false
    });

    loading.present();
    const editVM: EditQuote = {
      id: this.editablePost.id,
      content: this.quoteForm.value.content,
      publicationDate: this.quoteForm.value.publicationDate,
      endDate: this.quoteForm.value.endDate
    };
    this._quoteController.update(editVM);
    loading.dismiss();
    this.modalCtrl.dismiss({
      action: 'update',
      post: editVM
    });
  }

  /**
   * method: onCheckDates
   * That method is a blank method.
   */
  public onCheckDates(publication: Date, end: Date): boolean {
    // TO DO
    var now = new Date();
    now.setHours(0, 0, 0, 0);

    //if publication or end date is before today, return false and show a toast
    if(publication.getTime() < now.getTime() || end.getTime() < now.getTime())
    {
      this.showToast(this.translations['posts.cantPubBefTod']);
      return false;
    }
    //if end date is before publication date, return false and show a toast
    if(end.getTime() < publication.getTime())
    {
      this.showToast(this.translations['posts.cantExpBefPub']);
      return false;
    }
    //if the end date is the same than the publication date, return true but show a warning toast
    if(end.getTime() == publication.getTime())
    {
      this.showToast(this.translations['posts.warnOneDayAvailable']);
      return true;
    }
    return true;
  }

  public onCheckSelectedGroups(){

    //when changing the targetted groups, if the public group is selected, then only select this one
    this.settings.targetGroupIds.forEach(element => {
      if(element == this.publicGroupId){
        this.settings.targetGroupIds = new Array<string>()        
        this.settings.targetGroupIds.push(this.publicGroupId);      
        return;
      }  
    });  
  }

  private async showToast(message: string) {
    const toast = await this._toastCtrl.create({
      message: message,
      duration: 2000,
      position: 'top'
    });

    toast.present();
  }

  /**
   * method: onCloseModal
   * That method is a blank method.
   */
  public onCloseModal() {
    // TO DO
    this.modalCtrl.dismiss();
  }
}
