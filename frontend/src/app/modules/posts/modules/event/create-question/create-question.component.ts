import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Camera, CameraOptions } from '@ionic-native/camera/ngx';
import { ActionSheetController, LoadingController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { EventControllerService } from 'src/app/services/api/event-controller.service';
import { ImagesProcessingHelper } from 'src/app/services/helpers/images-processing.service';
import { CreationUtils } from 'src/app/shared/interfaces/posts/creation-utils';
import { CreateEvent } from 'src/app/shared/interfaces/posts/event/create-event';

@Component({
  selector: 'event-create-question',
  templateUrl: './create-question.component.html',
  styleUrls: ['./create-question.component.scss'],
})
export class CreateQuestionComponent implements OnInit {
//Idea default picture
private defaultPicture: any;

//boolean indicationg whether or not the user selected his own picture
private pictureSelected: boolean;

//Style to affect to the square showing the user selected picture
private userPictureBoxStyle: string;

//style to affect to the square showing the default picture
private defaultPictureBoxStyle: string;

//The user selected picture after compressing
private smallImg = null;

private questionForm: FormGroup;

@Input() vm: CreateEvent;
@Input() utils: CreationUtils;

@Output() changes = new EventEmitter<CreateEvent>();
@Output() utilsChange = new EventEmitter<CreationUtils>();
@Output() picture = new EventEmitter<any>();

constructor(
  public _eventController: EventControllerService,
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
  this.defaultPictureBoxStyle = this.onGetStyle(this.pictureSelected);
}

public ngOnInit(): void{
  this.onGetDefaultPicture();
  this.onAdaptImageBoxStyle(true);

  this.questionForm = this.formBuilder.group({
    content: [this.vm.content, Validators.compose([ Validators.required, Validators.maxLength(140)])]
  });

  this.questionForm.valueChanges.subscribe(changes => this.emitChanges(changes));
}

emitChanges(changes: any) {
  this.vm.content = changes.content;
  this.changes.emit(this.vm)
}

/**
 * method: onGetDefaultPicture
 * You should add a description of your method here.
 * that method is an Api service call method.
 * @returns A `Subscription<any>`.
 */
public onGetDefaultPicture() {
  return this._eventController.getDefaultPicture().subscribe(data => {
    this.defaultPicture = "data:image/jpeg;base64," + data.picture;
    this.picture.emit(this.defaultPicture);
    this.utilsChange.emit({...this.utils, pictureSelected: false});
  });
}

/**
 * method: onSelectImageSource
 * That method is a blank method.
 */
public async onSelectImageSource() {
  let takePictureText;
  this._translateService.get('idea.takePicture').subscribe(res => takePictureText = res);
  
  let fromGalleryText;
  this._translateService.get('idea.fromGallery').subscribe(res => fromGalleryText = res);

  let actionSheet = await this._actionSheetCtrl.create({
    header: 'Option',
    buttons: [
      {
        text: takePictureText,
        role: 'destructive',
        icon: 'camera-outline',
        handler: () => {
          this.onAdaptImageBoxStyle(false);
          this.onCaptureImage(false);
        }
      },
      {
        text: fromGalleryText,
        icon: 'image-outline',
        handler: () => {
          this.onAdaptImageBoxStyle(false);
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
  const options: CameraOptions = {
    quality: 100,
    destinationType: this.camera.DestinationType.DATA_URL,
    encodingType: this.camera.EncodingType.JPEG,
    mediaType: this.camera.MediaType.PICTURE,
    ...useGallery ? {sourceType: this.camera.PictureSourceType.SAVEDPHOTOALBUM} : {correctOrientation: true}
  }

  const imageData = await this.camera.getPicture(options);
  let base64Image  = 'data:image/jpeg;base64,' + imageData;

  let compressingText;
  this._translateService.get('idea.compressing').subscribe(res => compressingText = res);

  //Launch loading before compressing image
  let loading = await this._loadingCtrl.create({
    message: compressingText,
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
  if(clickOnDefaultImage){
    this.userPictureBoxStyle = this.onGetStyle(false);
    this.defaultPictureBoxStyle = this.onGetStyle(true);
    this.pictureSelected = false;
    this.picture.emit(this.defaultPicture);
    this.utilsChange.emit({...this.utils, pictureSelected: false});
  }
  else
  {
    this.userPictureBoxStyle = this.onGetStyle(true);
    this.defaultPictureBoxStyle = this.onGetStyle(false);
    this.pictureSelected = true;
  }
}

/**
 * method: onGetStyle
 * That method is a blank method.
 */
public onGetStyle(borderDisplay: boolean) {
  if(borderDisplay)
    return '3px solid black';
  else
    return '3px solid transparent';
}
}
