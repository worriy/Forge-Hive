import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {
  AlertController,
  LoadingController,
  ToastController,
} from '@ionic/angular';
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
  private loading: HTMLIonLoadingElement = null;

  constructor(
    private router: Router,
    private userController: UserControllerService,
    private loadingCtrl: LoadingController,
    private translateService: TranslateService,
    private toastCtrl: ToastController,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit() {
    this.getTranslations();
    this.createLoading();
    this.authForm = this.formBuilder.group(
      {
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        email: [
          '',
          Validators.compose([
            Validators.required,
            Validators.pattern(
              '^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$'
            ),
          ]),
        ],
        password: [
          '',
          Validators.compose([Validators.required, Validators.minLength(8)]),
        ],
        confirmPassword: [
          '',
          Validators.compose([Validators.required, Validators.minLength(8)]),
        ],
      },
      {
        validator: this.matchingPasswords('password', 'confirmPassword'),
      }
    );
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  /**
   * Retrieve needed translations for later use
   */
  private async getTranslations() {
    this.translateService
      .stream([
        'commons.loadingPleaseWait',
        'commons.confirmEmail',
        'account.errorPassword',
        'account.emailExist',
      ])
      .pipe(takeUntil(this.destroy))
      .subscribe(
        (translations: string[]) => (this.translations = translations)
      );
  }

  /**
   * Check that password and confirm password values are equal
   * @param passwordKey: key in the form group
   * @param confirmPasswordKey: key in the form group
   */
  public matchingPasswords(passwordKey: string, confirmPasswordKey: string) {
    return (group: FormGroup): { [key: string]: any } => {
      let password = group.controls[passwordKey];
      let confirmPassword = group.controls[confirmPasswordKey];

      if (password.value !== confirmPassword.value) {
        return {
          mismatchedPasswords: true,
        };
      }
    };
  }

  /**
   * If the form is valid, Check email existence and if not create account
   * @param email `string`.
   */
  public createAccount() {
    if (this.authForm.invalid) {
      return;
    }

    this.loading.present();
    this.userController.existEmail(this.authForm.value.email).subscribe(
      (res) => {
        // Email does not exist
        if (!res) {
          const register: Register = {
            firstName: this.authForm.value.firstName,
            lastName: this.authForm.value.lastName,
            email: this.authForm.value.email,
            password: this.authForm.value.password,
            confirmPassword: this.authForm.value.confirmPassword,
          };

          this.userController.register(register).subscribe(
            () => {
              this.loading.dismiss();
              const userProfileId = this.userController.getUserProfileId();
              if (userProfileId) {
                this.userController.get(userProfileId).subscribe(() => {
                  this.router.navigate(['/']);
                });
              } else {
                //if error while signIn, show an error toast
                this.showToast(this.translations['commons.error']);
              }
            },
            (error) => {
              //if error
              this.loading.dismiss();
              this.showToast(this.translations['account.errorPassword']);
            }
          );
        } else {
          //if error while signIn, show an error toast
          this.loading.dismiss();
          this.showToast(this.translations['account.emailExist']);
        }
      },
      (error) => {
        // Error on check email
        this.loading.dismiss;
        this.showToast(this.translations['account.errorPassword']);
      }
    );
  }

  /**
   * method: toLogin
   * Navigate to login page
   */
  public toLogin() {
    this.router.navigate(['welcome/login']);
  }

  /**
   * Preemptively create loader instance
   */
  private async createLoading() {
    this.loading = await this.loadingCtrl.create({
      message: this.translations['commons.loadingPleaseWait'],
      spinner: 'crescent',
      showBackdrop: false,
      backdropDismiss: true,
    });
  }

  /**
   * Show a toast message
   * @param message toast msg: string
   */
  private async showToast(message: string) {
    this.toastCtrl
      .create({
        message,
        duration: 2500,
        position: 'middle',
      })
      .then((t) => t.present());
  }
}
