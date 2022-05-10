import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Camera, CameraOptions } from '@ionic-native/camera/ngx';
import { ActionSheetController, LoadingController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { PostsControllerService } from 'src/app/services/api/posts-controller.service';
import { ImagesProcessingHelper } from 'src/app/services/helpers/images-processing.service';
import { CreatePost } from 'src/app/shared/interfaces/posts/create-post';
import { CreationUtils } from 'src/app/shared/interfaces/posts/creation-utils';

@Component({
  selector: 'question-create-question',
  templateUrl: './create-question.component.html',
  styleUrls: ['./create-question.component.scss'],
})
export class CreateQuestionComponent implements OnInit {

  //Idea default picture
  public defaultPicture: any;

  //boolean indicationg whether or not the user selected his own picture
  public pictureSelected: boolean;

  //The user selected picture after compressing
  private smallImg = null;

  private questionForm: FormGroup;

  private translations: string[] = [];

  @Input() vm: CreatePost;
  @Input() utils: CreationUtils;

  @Output() changes = new EventEmitter<CreatePost>();
  @Output() utilsChange = new EventEmitter<CreationUtils>();
  @Output() picture = new EventEmitter<any>();

  constructor(
    public _postsController: PostsControllerService,
    public _imageHelper: ImagesProcessingHelper,
    public router: Router,
    public _toastCtrl: ToastController,
    public _loadingCtrl: LoadingController, 
    public _actionSheetCtrl: ActionSheetController,
    public _translateService: TranslateService,
    private camera: Camera,
    private formBuilder: FormBuilder
  ) {
    this.pictureSelected = false;
  }

  public ngOnInit(): void{
    this.getTranslations();
    this.onGetDefaultPicture();

    this.questionForm = this.formBuilder.group({
      content: [this.vm.content, Validators.compose([ Validators.required, Validators.maxLength(90)])]
    });

    this.questionForm.valueChanges.subscribe(changes => this.emitChanges(changes));
  }

  emitChanges(changes: any) {
    this.vm.content = changes.content;
    this.changes.emit(this.vm)
  }

  private getTranslations() {
    this._translateService.stream([
      'idea.takePicture',
      'idea.fromGallery',
      'idea.compressing'
    ]).subscribe((translations: string[]) => this.translations = translations);
  }

  /**
   * Call server to get default Question image
   */
  public onGetDefaultPicture() {
    return this._postsController.getDefaultPicture(this.vm.type).subscribe(data => {
      this.defaultPicture = "data:image/jpeg;base64," + data.picture;
      this.picture.emit(this.defaultPicture);
      this.utilsChange.emit({...this.utils, pictureSelected: false});
    });
  }

  /**
   * Triggers display of an alert to select to upload the picture from the gallery or by taking a new one
   */
  public async selectImageSource() {

    let actionSheet = await this._actionSheetCtrl.create({
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
      showBackdrop: false,
      backdropDismiss: true
    });
    loading.present();
    
    this._imageHelper.imageLoaded(base64Image, 0, compressedImage => {
      if(compressedImage != "tooBig")
      {
        this.smallImg = compressedImage;
        this.pictureSelected = true;
        this.picture.emit(this.smallImg);
        this.utilsChange.emit({...this.utils, pictureSelected: true});
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
   * Select the default picture and emit the change
   */
  public selectDefaultPicture() {
    this.pictureSelected = false;
    this.picture.emit(this.defaultPicture);
    this.utilsChange.emit({...this.utils, pictureSelected: false})
  }
}
