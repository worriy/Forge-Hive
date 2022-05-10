import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertController, LoadingController, ModalController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { UpdatePassword } from 'src/app/shared/interfaces/user/update-password';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
})
export class ChangePasswordComponent implements OnInit, OnDestroy {
  private destroy = new Subject<void>();

  authForm: FormGroup;
  private email: string;
  private translations: string[] = [];

  constructor(
    public _userController: UserControllerService,
    public _translateService: TranslateService,
    public router: Router,
    public activatedRoute: ActivatedRoute,
    public _loadingCtrl: LoadingController,
    private _toastCtrl: ToastController,
    public _alertCtrl: AlertController,
    public formBuilder: FormBuilder,
    private modalCtrl: ModalController
  ) {
    this.activatedRoute.queryParams.subscribe(params => {
      this.email = params.email
    }); 
    this.authForm = formBuilder.group({
      current_password: ['', Validators.required],
      password: ['', Validators.compose([Validators.required, Validators.minLength(6), Validators.maxLength(12), Validators.pattern('^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[!@#$&*]).{6,12}$')])],
      confirm_password: ['', Validators.compose([Validators.required, Validators.minLength(6), Validators.maxLength(12), Validators.pattern('^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[!@#$&*]).{6,12}$')])],
    }, {
      validator: this.matchingPasswords('password', 'confirm_password')
    });
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
      'account.errorPassword',
      'account.notSaving'
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

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

  public updatePassword() {
    let resetPassword: UpdatePassword = {
      email: this.email,
      currentPassword: this.authForm.controls.current_password.value,
      password: this.authForm.controls.password.value,
      confirmPassword: this.authForm.controls.confirm_password.value 
    };
    return this._userController.updatePassword(resetPassword).subscribe(() => {
      this.modalCtrl.dismiss();
    },
    async error => {
      let toast = await this._toastCtrl.create({
        message: this.translations['account.errorPassword'],
        duration: 2500,
        position: 'middle'
      });

      toast.present();
    }); 
  }

  public async return(){
    const currentPassword = this.authForm.controls.current_password.value;
    const password = this.authForm.controls.password.value;
    const confirmPassword = this.authForm.controls.confirm_password.value;
    if(currentPassword || password || confirmPassword){
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
              this.modalCtrl.dismiss();
            }
          }
        ]
      });
      alert.present();
    } else {
      this.modalCtrl.dismiss();
    }
  }
}
