import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AlertController, LoadingController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { Register } from 'src/app/shared/interfaces/user/register';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit, OnDestroy {
  private destroy = new Subject<void>();

  public authForm: FormGroup;

  private translations: string[] = [];
  private passwordPattern = '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[!@#$&*]).{6,12}$';

  private loading: HTMLIonLoadingElement = null;

  constructor(
    private router: Router,
    private userController: UserControllerService,
    private loadingCtrl: LoadingController,
    private translateService: TranslateService,
    private toastCtrl: ToastController,
    private formBuilder: FormBuilder,
    private alertCtrl: AlertController,
  ) {
  }

  ngOnInit() {
    this.getTranslations();
    this.createLoading();
    this.authForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', Validators.compose([
        Validators.required,
        Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')
      ])],
      password: ['', Validators.compose([
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(12),
        Validators.pattern(this.passwordPattern)
      ])],
      confirmPassword: ['', Validators.compose([
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(12),
        Validators.pattern(this.passwordPattern)
      ])],
    }, {
      validator: this.matchingPasswords('password', 'confirmPassword')
    });
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  /**
   * Retrieve needed translations for later use
   */
  private async getTranslations() {
    this.translateService.stream([
      'commons.loadingPleaseWait',
      'commons.confirmEmail',
      'account.errorPassword',
      'account.emailExist'
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

  /**
   * Check that password and confirm password values are equal
   * @param passwordKey: key in the form group
   * @param confirmPasswordKey: key in the form group 
   */
  public matchingPasswords(passwordKey: string, confirmPasswordKey: string){
    return (group: FormGroup): {[key: string]: any} => {
      let password = group.controls[passwordKey];
      let confirmPassword = group.controls[confirmPasswordKey];

      if(password.value !== confirmPassword.value) {
        return {
          mismatchedPasswords: true
        };
      }
    }
  }

  /**
   * If the form is valid, Check email existence and if not create account
   * @param email `string`.
   */
  public onCreateAccount() {
    if (this.authForm.invalid) {
      return;
    }

    this.loading.present();
    this.userController.existEmail(this.authForm.value.email).subscribe(res => {
      // Email does not exist
      if(!res){

        const register: Register = {
          firstName: this.authForm.value.firstName,
          lastName: this.authForm.value.lastName,
          email: this.authForm.value.email,
          password: this.authForm.value.password,
          confirmPassword: this.authForm.value.confirmPassword
        };

        this.userController.register(register).subscribe(() => {
          this.loading.dismiss();
          this.userController.getRoles(localStorage.getItem('authenticatedUser')).pipe(takeUntil(this.destroy))
          .subscribe(() => {
            let user;
            if (user = localStorage.getItem('authenticatedUser') ) {
              this.onGetIdUser(user).then(() => {
                console.log('REGISTER');
                this.router.navigate(['/']);
              });
            }
            else {
              //if error while signIn, show an error toast
              this.showToast(this.translations['commons.error'])
            }
          })
          

          // FIXME: Seems like the first behavior were to login directly, probably the best solution (and make email confirmation optional)
          //this.nav.push('login-confirmEmail');
            /*this.userController.getRoles(JSON.parse(localStorage.getItem('authenticatedUser')).id).subscribe(() => {
              //Successful Login
              if (localStorage.getItem('authenticatedUser')) {
                this.onGetIdUser(JSON.parse(localStorage.getItem('authenticatedUser')).id).then(() => {
                loading.dismiss();
                this.nav.setRoot('tabs-main');
                });
              }
              else {
                loading.dismiss()
                let errorMessage;
                this.translateService.get('commons.error').subscribe(res => errorMessage = res);
                //if error while signIn, show an error toast
                let toast = this.toastCtrl.create({
                  message: errorMessage,
                  duration: 2500,
                  position: 'middle'
                });
                toast.present();
              }
            });*/
          },
          error => {
            //if error 
            this.loading.dismiss();
            this.showToast(this.translations['account.errorPassword']);
          }
        );
      }
      else{
        //if error while signIn, show an error toast
        this.loading.dismiss();
        this.showToast(this.translations['account.emailExist']);
      }
    },
    error => {
      // Error on check email
      this.loading.dismiss;
      this.showToast(this.translations['account.errorPassword']);
    });
  }

  /**
   * method: onToLogin
   * Navigate to login page
   */
  public onToLogin() {
    this.router.navigate(['welcome/login']);
  }

  /**
   * Retrieve profile id from application user id
   * @param applicationUserId 
   */
  public async onGetIdUser(applicationUserId: string) {
    // FIXME: We retrieve a complete user just for an id
    return this.userController.get(applicationUserId).toPromise().then(response => {
      localStorage.setItem('userProfileId', response.userProfileId.toString());
    });
  }

  /**
   * Preemptively create loader instance 
   */
  private async createLoading() {
    this.loading = await this.loadingCtrl.create({
      message: this.translations['commons.loadingPleaseWait'],
        spinner: 'crescent',
        showBackdrop: false,
        backdropDismiss: true
    });
  }

  /**
   * Show a toast message
   * @param message toast msg: string
   */
  private async showToast(message: string) {
    this.toastCtrl.create({
      message,
      duration: 2500,
      position: 'middle'
    }).then(t => t.present());
  }

}
