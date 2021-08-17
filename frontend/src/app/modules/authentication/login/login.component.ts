import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoadingController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { NotificationHelper } from 'src/app/services/helpers/notification.service';
import { SignIn } from 'src/app/shared/interfaces/user/sign-in';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit, OnDestroy {
  private destroy = new Subject<void>();

  public authForm: FormGroup;

  private translations: string[] = [];
  private loading: HTMLIonLoadingElement;

  constructor(
    private userController: UserControllerService,
    private loadingCtrl: LoadingController,
    private toastCtrl: ToastController,
    private translateService: TranslateService,
    private formBuilder: FormBuilder,
    private router: Router,
    private notifHelper: NotificationHelper
  ) {
  }

  ngOnInit() {
    this.getTranslations();
    this.createLoading();
    this.authForm = this.formBuilder.group({
      email: ['', Validators.compose([Validators.required,  Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')])],
      password: ['', Validators.required]
    });
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  /**
   * Fetch needed translations for later user
   */
  private async getTranslations() {
    this.translateService.stream([
      'commons.loadingPleaseWait',
      'commons.error',
      'login.loginFailure',
      'login.notConfirmMail'
    ]).pipe(takeUntil(this.destroy))
      .subscribe((translations: string[]) => this.translations = translations);
  }

  /**
   * method: onLogin
   * This method calls the Auth controller to perform the login, if the login succeeds, it fetch the userProfile Id
   */
  public onLogin() {
    if (this.authForm.invalid) {
      return;
    }

    this.loading.present();

    const signIn: SignIn = {
      email: this.authForm.value.email,
      password: this.authForm.value.password
    };

    return this.userController.signIn(signIn).pipe(takeUntil(this.destroy))
    .subscribe(() => {
      // FIXME: User should be returned here or just below
      //Successful Login
      console.log('Authenticated User');
      console.log(localStorage.getItem('authenticatedUser'));
      this.userController.getRoles(localStorage.getItem('authenticatedUser')).pipe(takeUntil(this.destroy))
      .subscribe(() => {
        let user;
        if (user = localStorage.getItem('authenticatedUser') ) {
          this.onGetIdUser(user).then(() => {
            console.log('LOGIN');
            //Trigger the results notifications sending
            //this.notifHelper.notifyForResults();
            this.loading.dismiss();
            this.router.navigate(['/']);
          });
        }
        else {
          this.loading.dismiss()
  
          //if error while signIn, show an error toast
          this.showToast(this.translations['commons.error'])
        }
      })
      
      
    },
      error => {
        this.loading.dismiss();

        var httpError: HttpErrorResponse = error;
        console.log(httpError);

        let errorMessage = this.translations['commons.error'];

        //differentiate error types
        if (!!httpError && !!httpError.error) {
          if(httpError.error.loginfailure)
            errorMessage = this.translations['login.loginFailure'];
          else {
            if(httpError.error.not_confirm_mail)
              errorMessage = this.translations['login.notConfirmMail'];
          }
        }
          
        //if error while signIn, show an error toast
        this.showToast(errorMessage);
      })
  }

  /**
   * Preemptively create the loader instance to be able to present/dismiss it whenever
   */
  private async createLoading() {
    this.loading = await this.loadingCtrl.create({
      message: this.translations['commons.loadingPleaseWait'],
      spinner: 'crescent',
      showBackdrop: false,
      backdropDismiss: false
    });
  }

  /**
   * show a toast message
   * @param message Toast msg: string
   */
  private showToast(message: string) {
    this.toastCtrl.create({
      message: message,
      duration: 2500,
      position: 'top'
    }).then(t => t.present());
  }

  /**
   * method: onGetIdUser
   * Used to fetch the UserProfile Id (int)
   * @param applicationUserId `string`.
   * @returns A `Subscription<any>`.
   */
  public async onGetIdUser(applicationUserId: string) {
    return this.userController.get(applicationUserId).pipe(takeUntil(this.destroy))
    .toPromise().then(response => {
      // FIXME: We retrieve the complete user just for an id
      console.log(response);
      localStorage.setItem('userProfileId', response.userProfileId);
      //handle the push notifications.
      //this.notifHelper.handlePushNotifications();
    });
  }

  /**
   * method: onToForgotPassword
   * Navigate to forgot password page
   */
  public onToForgotPassword() {
    this.router.navigate(['welcome/forgot-password']);
  }

  /**
   * Navigate to register page
   */
  public onToRegister(){
    this.router.navigate(['welcome/register']);
  }

}
