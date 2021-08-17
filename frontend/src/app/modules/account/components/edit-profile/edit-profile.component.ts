import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { ActionSheetController, AlertController, LoadingController, NavParams, Platform, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { PictureControllerService } from 'src/app/services/api/picture-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { ImagesProcessingHelper } from 'src/app/services/helpers/images-processing.service';
import { FullUser } from 'src/app/shared/interfaces/user/full-user';
import { UpdateProfilePicture } from 'src/app/shared/interfaces/user/update-profile-picture';
import { UserUpdate } from 'src/app/shared/interfaces/user/user-update';
import { Camera, CameraOptions } from '@ionic-native/camera/ngx';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GroupManagementService } from 'src/app/services/group-management.service';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.scss'],
})
export class EditProfileComponent implements OnInit, OnDestroy {
  private destroy = new Subject<void>();

  private phoneNumber: string;
  private email: string;

  //picture url when the user select a new picture
  private url: any ; 

  //boolean indicating if the user selected a new picture 
  private change: boolean;
  
  public user: FullUser;
  private currentUser: any;

  private translations: string[] = [];

  public userForm: FormGroup;
  
  constructor(
    public _userController: UserControllerService,
    public _pictureController: PictureControllerService,
    public _imageHelper: ImagesProcessingHelper,
    public router: Router,
    public _navParam: NavParams,
    public _loadingCtrl: LoadingController,
    private camera: Camera,
    private actionSheetCtrl: ActionSheetController,
    public _translateService: TranslateService,
    public _alertCtrl: AlertController,
    private activatedRoute: ActivatedRoute,
    private formBuilder: FormBuilder
  ) {
    this.change = false ;
    this.activatedRoute.queryParams.subscribe(res => {
      this.user = JSON.parse(res.param);
      this.initForm();
    });
    this.user = _navParam.get('param');
    //this.email = _navParam.get('param').email ; 

    this.currentUser = localStorage.getItem('authenticatedUser');
  }

  ngOnInit() {
    this.getTranslations();
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  getTranslations() {
    this._translateService.stream([
      'commons.cancel',
      'commons.no',
      'commons.yes',
      'idea.takePicture',
      'idea.fromGallery',
      'idea.compressing',
      'account.notSaving',
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

  private initForm() {
    this.userForm = this.formBuilder.group({
      firstName: [this.user.firstname, Validators.required],
      lastName: [ this.user.lastname, Validators.required],
      email: [ this.user.email, Validators.compose([
        Validators.required,
        Validators.email
      ])],
      phoneNumber: [ this.user.phoneNumber ],
      country: [ this.user.country ],
      city: [ this.user.city ],
      department: [ this.user.department ],
      job: [ this.user.job ]
    });
  }

  /**
   * method: onUpdateUser
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param user `UserVM`.
   * @returns A `Subscription<any>`.
   */

  public onCheckChanges(): boolean{
    if (this.userForm.invalid) {
      return;
    }

    const userUpdate: UserUpdate = {
      ...this.userForm.value
    };
    if(this.user.firstname != userUpdate.firstName || this.user.lastname != userUpdate.lastName || this.user.email != userUpdate.email || this.user.phoneNumber != userUpdate.phoneNumber || this.user.country != userUpdate.country || this.user.city != userUpdate.city || this.user.department != userUpdate.department || this.user.job != userUpdate.job )
      return true;
    else
      return false;
  }

  public async onUpdateUser() {
    if (this.userForm.invalid) {
      return;
    }

    let loading;
    var userUpdate: UserUpdate = {
      ...this.userForm.value
    };
    console.log(userUpdate);
    //if there is a file to upload
    loading = await  this._loadingCtrl.create({
      spinner: 'crescent',
      showBackdrop: false,
      backdropDismiss: true
    });
    loading.present();
    if (this.url) {
      //We upload it
      let updateProfilePictureVM: UpdateProfilePicture = {
        idUser: this.currentUser,
        picture: this.url
      };
      this._userController.updatePicture(updateProfilePictureVM).subscribe(() => {
        //if the phone number has been changed
        
        if(this.onCheckChanges())
        {     
          //we update the user
          this._userController.update(userUpdate).subscribe(() => {
            this.router.navigate(['tabs/account']);
            loading.dismiss();
          },
          error => {
            console.warn(error);
            loading.dismiss();
          });
        }
        else 
        {
          this.router.navigate(['tabs/account']);
          loading.dismiss();
        }  
      });
      
    }
    else 
    {
      //if the phone number has been changed
      if(this.onCheckChanges())
      {
        //We update the user
        this._userController.update(userUpdate).subscribe(res => {
          this.router.navigate(['tabs/account']);
          loading.dismiss();
        })
      }
      else
      {
        this.router.navigate(['tabs/account']);
        loading.dismiss();
      }
    }
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
  // FIXME: Find better handling for loading and alerts
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
      showBackdrop: false,
      backdropDismiss: true
    });
    loading.present();
    
    this._imageHelper.imageLoaded(base64Image, 0, (imgUrl, imgFile) => {
      if(imgUrl != "tooBig")
      {
        this.url = imgUrl;
        this.change = true;
      }

      loading.dismiss();      
    })
    loading.dismiss();
  }

  onToChangePassword(){
    this.router.navigate(['tabs/account/change-password'], { queryParams: { 'email': this.user.email }});
  }

  public async onReturn(){
    if (this.url){  
      let alert = await this._alertCtrl.create({
        header: this.translations['commons.cancel'],
        message: this.translations['account.notSaving'],
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
              this.router.navigate(['..'], { relativeTo: this.activatedRoute });
            }
          }
        ]
      });
      alert.present();
    } else{
      this.router.navigate(['..'], { relativeTo: this.activatedRoute });
    }
  }
}
