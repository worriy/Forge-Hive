import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Camera, CameraOptions } from '@ionic-native/camera/ngx';
import { ActionSheetController, AlertController, LoadingController, ModalController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { PictureControllerService } from 'src/app/services/api/picture-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { PostsControllerService } from 'src/app/services/api/posts-controller.service';
import { ImagesProcessingHelper } from 'src/app/services/helpers/images-processing.service';
import { TargetGroup } from 'src/app/shared/interfaces/groups/target-group';
import { EditPost } from 'src/app/shared/interfaces/posts/edit-post';
import { EditablePost } from 'src/app/shared/interfaces/posts/editable-post';
import { PictureVM } from 'src/app/shared/interfaces/posts/pictureVM';
import { Post } from 'src/app/shared/interfaces/posts/post';

@Component({
  selector: 'app-edit-question-modal',
  templateUrl: './edit-question-modal.component.html',
  styleUrls: ['./edit-question-modal.component.scss'],
})
export class EditQuestionModalComponent implements OnInit {
  private destroy = new Subject<void>();

  @Input() post: Post;
  editablePost: EditablePost;
  minDate : string = new Date().toISOString();

  //the current user's id
  userProfileId: any;

  //the current user's groups
  myGroups: TargetGroup[];
  publicGroupId: string;

  public pictureSelected = false;
  public postPicture: string;
  public customImg: any;

  private translations: string[] = [];

  public questionForm: FormGroup;

  public count_answers = 2;
  private maxAnswers : number = 5 ;
  public numberAnswers: Array<number>; 
  public visibleAdd = true;

  constructor(
    public _pictureController: PictureControllerService,
    public _postsController: PostsControllerService,
    public _imageHelper: ImagesProcessingHelper,
    public _groupController: GroupControllerService,
    public modalCtrl: ModalController,
    public _toastCtrl: ToastController,
    public _loadingCtrl: LoadingController,
    public _alertCtrl: AlertController,
    private camera: Camera,
    private actionSheetCtrl: ActionSheetController,
    public _translateService: TranslateService,
    private formBuilder: FormBuilder,
    public _userController: UserControllerService
  ) {
    this.userProfileId = this._userController.getUserProfileId();
    this.numberAnswers = Array(this.maxAnswers).fill(this.maxAnswers).map((x,i) => i);
  }

  public async ngOnInit() {
    this.getTranslations();
    this.initForm();
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  /**
   * Fetch needed informations and initialize form
   */
  private async initForm() {
    //launch a loading screen while loading the posts details
    let loading = await this._loadingCtrl.create({
      spinner: 'crescent',
      message: this.translations['commons.loadingPleaseWait'],
      showBackdrop: false
    });
    loading.present();

    this.getTargetableGroups().then(() => 
    {
      //get all the informations about the post
      this._postsController.getEditablePost(this.post.id).subscribe(data => 
      {
        this.editablePost = data;

        this.postPicture = "data:image/jpeg;base64," + data.picture;

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

        //closing the loader when retrieving is done.
        loading.dismiss();
      },
      error => {
        loading.dismiss();
      });
    });

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

  /**
   * Increment answers number counter, hide the add button if the max is reached, show a toast if the max is exceeded
   */
  async addNumberAnswers() {
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
   * Retrieves the user's groups to choose post target(s)
   */
  public async getTargetableGroups() {
    return this._groupController.listTargetableGroups(this.userProfileId).toPromise().then(data => 
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
   * Check the edited Question's consistency
   */
  public async checkInformation(){
    const publicationDate = new Date(this.questionForm.controls.publicationDate.value);
    const endDate = new Date(this.questionForm.controls.endDate.value);

    // Check that all the modifications are correct
    if(!this.checkDates(publicationDate, endDate))
      return;

    //check if the date changed
    if(!(this.editablePost.publicationDate == publicationDate))
      this.editablePost.publicationDate = publicationDate;
    if(!(this.editablePost.endDate == endDate))
      this.editablePost.endDate = endDate;
      
    this.saveChanges();  
  }
  
  /**
   * Save edited Idea
   */
  public async saveChanges() {
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

    let editVM: EditPost = {
      id: this.editablePost.id,
      content: this.questionForm.value.content,
      publicationDate: this.questionForm.value.publicationDate,
      endDate: this.questionForm.value.endDate,
      targetGroupsIds: this.questionForm.value.targetGroups,
      pictureId: null,
      choices: this.editablePost.choices
    };
    
    //if the user changed the picture, upload it and set the photoId of the post
    if(this.customImg)
    {
      loading.present();
      const pic: PictureVM = {
        picture: this.customImg
      };
      return this._pictureController.create(pic).subscribe((res: string) =>
      {
        editVM.pictureId = res;
        //then save the post
        this._postsController.update(editVM).subscribe(() => {
          loading.dismiss();
          this.modalCtrl.dismiss({
            action: 'update',
            post: editVM
          });
        });
      });
    }
    //if the user didn't choose an other picture, just save the post and dismiss the modal
    else 
    {
      loading.present();
      this._postsController.update(editVM).subscribe(() => {
        loading.dismiss();
        this.modalCtrl.dismiss({
          action: 'update',
          post: editVM
        });
      });
    }
  }

  /**
   * Set choices from form values, return false if not enough choices (2 minimum)
   */
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
   * Check publication and end dates logic, show a warning toast if the post will only be available for one day
   */
  public checkDates(publication: Date, end: Date): boolean {
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

  /**
   * Check that at least a group is selected, select the public one if not
   * If several groups are selected, including the public one, select only the public one
   */
  public checkSelectedGroups(){
    const selectedGroups = this.questionForm.controls.targetGroups.value;
    if(selectedGroups.length == 0 || (selectedGroups.length > 1 && selectedGroups.find(groupId => groupId === this.publicGroupId)))
    {
      this.questionForm.controls.targetGroups.setValue([this.publicGroupId], { emitEvent: false });
      return;
    } 
  }

  /**
   * Triggers display of an alert to select to upload the picture from the gallery or by taking a new one
   */
  public async selectImageSource() {

    let actionSheet = await this.actionSheetCtrl.create({
      header: 'Option',
      buttons: [
        {
          text: this.translations['idea.takePicture'],
          role: 'destructive',
          icon: 'camera-outline',
          handler: () => {
            this.captureImage(false);
          }
        },
        {
          text: this.translations['idea.fromGallery'],
          icon: 'image-outline',
          handler: () => {
            this.captureImage(true);
          }
        },
      ]
    });
    actionSheet.present();
  }

  /**
   * Triggers Camera to take a photo or opens gallery to choose from
   */
  public async captureImage(useGallery: boolean) {
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
        this.customImg = imgUrl;
        this.pictureSelected = true;
      }
      else
      {
        this.pictureSelected = false;
      }

      loading.dismiss();      
    })
    loading.dismiss();
  }

  /**
   * Show a toast with custom message
   * @param message 
   */
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
