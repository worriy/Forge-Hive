import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AlertController, LoadingController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { UserControllerService } from 'src/app/services/api/user-controller.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
})
export class ForgotPasswordComponent implements OnInit, OnDestroy {
  private destroy = new Subject<void>();

  public authForm: FormGroup;

  private translations: string[] = [];
  private loading: HTMLIonLoadingElement = null;

  constructor(
    private userController: UserControllerService,
    private router: Router,
    private loadingCtrl: LoadingController,
    private translateService: TranslateService,
    private alertCtrl: AlertController,
    private formBuilder: FormBuilder
  ) {
  }

  ngOnInit() {
    this.getTranslations();
    this.createLoading();
    this.authForm = this.formBuilder.group({
      email: ['', Validators.compose([Validators.required,  Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')])],
    });
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  /**
   * Fetch needed translations for later user
   */
  async getTranslations() {
    this.translateService.stream([
      'login.sendEmailLoading',
      'commons.resetPassword',
      'commons.wrongmail',
      'login.notConfirmMail',
      'commons.error'
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

  /**
   * method: onCheckEmail
   * if the form is valid, check mail validity/existence and send reset password mail 
   * @returns A `Subscription<any>`.
   */
  public onCheckEmail() {
    if (this.authForm.invalid)
      return;

    this.loading.present();

    // Check that mail exists and is confirmed, mail is sent if yes
    return this.userController.checkEmail(
      this.authForm.value.email
    ).pipe(takeUntil(this.destroy))
    .subscribe(() => {
      this.loading.dismiss();
      this.alertCtrl.create({
        header: 'Reset Password',
        message: this.translations['commons.resetPassword'],
        buttons: [
          {
            text: 'OK',
            role: 'cancel',
            handler: () => {
              this.onToLogin();
            }
          }
        ]
      }).then(a => a.present());
    },
    error => {
      this.loading.dismiss();

      let httpError: HttpErrorResponse = error;
      let errorMessage;
      if (!!httpError && !!httpError.error) {
        if(httpError.error != null && httpError.error.wrongmail)
        errorMessage = this.translations['commons.wrongmail'];
        else {
          if(httpError.error != null && httpError.error.not_confirm_mail)
            errorMessage = this.translations['login.notConfirmMail'];
          else
            errorMessage = this.translations['commons.error'];
        }
      }

      this.alertCtrl.create({
        header: 'Error Send Mail',
        message: errorMessage,
        buttons: [
          {
            text: 'OK',
            role: 'cancel',
            handler: () => { }
          }
        ]
      }).then(a => a.present());
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
   * Preemptively create loader instance
   */
  private async createLoading() {
    this.loading = await this.loadingCtrl.create({
      message: this.translations['login.sendEmailLoading'],
      spinner: 'crescent',
      showBackdrop: false,
      backdropDismiss: true
    });
  }
}
