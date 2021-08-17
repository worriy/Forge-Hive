import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Camera, CameraOptions } from '@ionic-native/camera/ngx';
import { ActionSheetController, AlertController, LoadingController, ModalController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { PictureControllerService } from 'src/app/services/api/picture-controller.service';
import { QuestionControllerService } from 'src/app/services/api/question-controller.service';
import { ImagesProcessingHelper } from 'src/app/services/helpers/images-processing.service';
import { TargetGroup } from 'src/app/shared/interfaces/groups/target-group';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { PictureVM } from 'src/app/shared/interfaces/posts/pictureVM';
import { Post } from 'src/app/shared/interfaces/posts/post';
import { EditQuestion } from 'src/app/shared/interfaces/posts/question/edit-question';
import { EditableQuestion } from 'src/app/shared/interfaces/posts/question/editable-question';

@Component({
  selector: 'app-edit-question-modal',
  templateUrl: './edit-question-modal.component.html',
  styleUrls: ['./edit-question-modal.component.scss'],
})
export class EditQuestionModalComponent implements OnInit {
  private destroy = new Subject<void>();

  @Input() post: Post;
  editablePost: EditableQuestion;
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

  public questionForm: FormGroup;

  public count_answers = 2;
  private maxAnswers : number = 5 ;
  public numberAnswers: Array<number>; 
  public visibleAdd = true;

  constructor(
    public _pictureController: PictureControllerService,
    public _questionController: QuestionControllerService,
    public _imageHelper: ImagesProcessingHelper,
    public _groupController: GroupControllerService,
    public modalCtrl: ModalController,
    public _toastCtrl: ToastController,
    public _loadingCtrl: LoadingController,
    public _alertCtrl: AlertController,
    private camera: Camera,
    private actionSheetCtrl: ActionSheetController,
    public _translateService: TranslateService,
    private formBuilder: FormBuilder
  ) {
    this.userProfileId = localStorage.getItem('userProfileId');
    this.paging = {
      step: 10,
      lastId: 0
    };
    
    this.pictureSelected = false;
    this.numberAnswers = Array(this.maxAnswers).fill(this.maxAnswers).map((x,i) => i);
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
      this._questionController.getEditableQuestion(this.post.id).subscribe(data => 
      {
        this.editablePost = data;
        console.log(this.editablePost);
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
        this.questionForm = this.formBuilder.group({
          content: [this.post.content, Validators.compose([Validators.required, Validators.maxLength(140)])],
          publicationDate: [this.post.publicationDate, Validators.required],
          endDate: [this.post.endDate, Validators.required],
          targetGroups: [this.editablePost.targetGroupsIds, Validators.required],
          answer1: [this.editablePost.choices[0]?.name, Validators.maxLength(10)],
          answer2: [this.editablePost.choices[1]?.name, Validators.maxLength(10)],
          answer3: [this.editablePost.choices[2]?.name, Validators.maxLength(10)],
          answer4: [this.editablePost.choices[3]?.name, Validators.maxLength(10)],
          answer5: [this.editablePost.choices[4]?.name, Validators.maxLength(10)]
        });
        this.questionForm.controls.targetGroups.patchValue(this.editablePost.targetGroupsIds);
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
      'idea.compressing',
      'question.maxAnswers',
      'question.minAnswers'
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

  async onAddNumberAnswers() {
    if (this.count_answers < 5)
      this.count_answers++;
    else {
      let toast = await this._toastCtrl.create({
        message: this.translations['question.maxAnswers'],
        duration: 2500,
        position: 'middle'
      })
      toast.present();
    }

    if (this.count_answers == 5)
      this.visibleAdd = false;    
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
    if (this.questionForm.invalid)
      return;
    // Save the changes if the changes are correct
    let loading = await this._loadingCtrl.create({
      spinner: 'crescent',
      message: this.translations['commons.saving'],
      showBackdrop: false
    });

    if (!this.setChoices())
      return;
    
    //if the user changed the picture, upload it and set the photoId of the post
    if(this.smallImg)
    {
      loading.present();
      const pic: PictureVM = {
        picture: this.smallImg
      };
      return this._pictureController.create(pic).subscribe((res: string) =>
      {
        const editVM: EditQuestion = {
          id: this.editablePost.id,
          content: this.questionForm.value.content,
          publicationDate: this.questionForm.value.publicationDate,
          endDate: this.questionForm.value.endDate,
          targetGroupsIds: this.questionForm.value.targetGroups,
          pictureId: res,
          choices: this.editablePost.choices
        };
        //then save the post
        this._questionController.update(editVM);
        loading.dismiss();
        this.modalCtrl.dismiss({
          action: 'update',
          post: editVM
        });
      });
    }
    //if the user didn't choose an other picture, just save the post and dismiss the modal
    else 
    {
      loading.present();
      const editVM: EditQuestion = {
        id: this.editablePost.id,
        content: this.questionForm.value.content,
        publicationDate: this.questionForm.value.publicationDate,
        endDate: this.questionForm.value.endDate,
        targetGroupsIds: this.questionForm.value.targetGroups,
        pictureId: null,
        choices: this.editablePost.choices
      };
      this._questionController.update(editVM);
      loading.dismiss();
      this.modalCtrl.dismiss({
        action: 'update',
        post: editVM
      });
    }
  }

  private setChoices(): boolean {
    let choices = [];
    let i = 0;
    if (!!this.questionForm.value.answer1) {
      choices[i] = this.questionForm.value.answer1;
      i++;
    }
    if (!!this.questionForm.value.answer2) {
      choices[i] = this.questionForm.value.answer2;
      i++;
    }
    if (!!this.questionForm.value.answer3) {
      choices[i] = this.questionForm.value.answer3; 
      i++;
    }
    if (!!this.questionForm.value.answer4) {
      choices[i] = this.questionForm.value.answer4;
      i++;
    }
    if (!!this.questionForm.value.answer5) {
      choices[i] = this.questionForm.value.answer5;
      i++;
    }

    if (choices.length < 2) {
      this.showToast(this.translations['question.minAnswers']);
      return false;
    }

    for (i=0; i < choices.length; i++) {
      if (this.editablePost.choices[i] != undefined)
        this.editablePost.choices[i].name = choices[i];
      else
        this.editablePost.choices[i] = { id: "00000000-0000-0000-0000-000000000000", name: choices[i], rawVersion: null};
    }

    return true;
  }

  /**
   * method: onCheckDates
   * That method is a blank method.
   */
  public onCheckDates(publication: Date, end: Date): boolean {
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

  /**
   * method: onSelectImageSource
   * That method is a blank method.
   */
  public async onSelectImageSource() {

    let actionSheet = await this.actionSheetCtrl.create({
      header: 'Option',
      buttons: [
        {
          text: this.translations['idea.takePicture'],
          role: 'destructive',
          icon: 'camera-outline',
          handler: () => {
            this.onCaptureImage(false);
          }
        },
        {
          text: this.translations['idea.fromGallery'],
          icon: 'image-outline',
          handler: () => {
            this.onCaptureImage(true);
          }
        },
      ]
    });
    actionSheet.present();
  }

  /**
   * method: onCaptureImage
   * That method is a blank method.
   */
  public async onCaptureImage(useGallery: boolean) {
    // TO DO
    const options: CameraOptions = {
      quality: 100,
      destinationType: this.camera.DestinationType.DATA_URL,
      encodingType: this.camera.EncodingType.JPEG,
      mediaType: this.camera.MediaType.PICTURE,
      ...useGallery ? {sourceType: this.camera.PictureSourceType.SAVEDPHOTOALBUM} : {correctOrientation: true}
    }

    const imageData = await this.camera.getPicture(options);
    let base64Image  = 'data:image/jpeg;base64,' + imageData;

    //Launch loading before compressing image
    let loading = await this._loadingCtrl.create({
      message: this.translations['idea.compressing'],
      spinner: 'crescent',
      showBackdrop: false
    });
    loading.present();
    
    this._imageHelper.imageLoaded(base64Image, 0, (imgUrl, imgFile) => {
      if(imgUrl != "tooBig")
      {
        this.smallImg = imgUrl;
        this.pictureSelected = true;
        this.onAdaptImageBoxStyle(false);
      }
      else
      {
        this.onAdaptImageBoxStyle(true);
        this.pictureSelected = false;
      }

      loading.dismiss();      
    })
    loading.dismiss();
  }

  /**
   * method: onAdaptImageBoxStyle
   * That method is a blank method.
   */
  public onAdaptImageBoxStyle(clickOnDefaultImage: boolean) {
    // TO DO
    if(clickOnDefaultImage){
      this.userPictureStyle = this.onGetStyle(false);
      this.defaultPictureStyle = this.onGetStyle(true);
      this.pictureSelected = false;
    }
    else
    {
      this.userPictureStyle = this.onGetStyle(true);
      this.defaultPictureStyle = this.onGetStyle(false);
      this.pictureSelected = true;
    }
  }

  /**
   * method: onGetStyle
   * That method is a blank method.
   */
  public onGetStyle(borderDisplay: boolean) {
    // TO DO
    if(borderDisplay)
      return '3px solid black';
    else
      return '3px solid transparent';
  }

  private async showToast(message: string) {
    const toast = await this._toastCtrl.create({
      message: message,
      duration: 2500,
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
