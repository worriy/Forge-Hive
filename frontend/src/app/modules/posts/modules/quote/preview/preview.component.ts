import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { QuoteControllerService } from 'src/app/services/api/quote-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { CreationUtils } from 'src/app/shared/interfaces/posts/creation-utils';
import { CreateQuote } from 'src/app/shared/interfaces/posts/quote/create-quote';
import { User } from 'src/app/shared/interfaces/user/user';

@Component({
  selector: 'quote-preview',
  templateUrl: './preview.component.html',
  styleUrls: ['./preview.component.scss'],
})
export class PreviewComponent implements OnInit {
  //the picture to display on the previewed card.
  private PicToPreview: string;

  //the card to preview.
  @Input() card: CreateQuote;
  @Input() picture: string;
  @Input() utils: CreationUtils;

  //connected user Id
  private appUserId: string;

  //the user creating the card.
  user: User;

  constructor(
    public _quoteController: QuoteControllerService,
    public _userController: UserControllerService,
    private _loadingCtrl: LoadingController,
    private _translateService: TranslateService,
    private router: Router
  ) {
    this.appUserId = localStorage.getItem('authenticatedUser');
  }

  ngOnInit(){
    this.PicToPreview = this.picture;
    //get the current user informations
    this._userController.get(
      this.appUserId
    ).subscribe(res => 
      this.user = res
    );
  }

  /**
   * method: onSave
   * You should add a description of your method here.
   * that method is an Api service call method.
   */
  public async onSave() {
    let loadingText;
    this._translateService.get("commons.saving").subscribe(res => loadingText = res);

    //launch a loading circle while saving.
    let loading = await this._loadingCtrl.create({
      spinner: 'crescent',
      message: loadingText,
      showBackdrop: false,
      backdropDismiss: true
    });
    loading.present();
    this.card.pictureId = null;

    this._quoteController.create(this.card).subscribe(data => 
    {
      loading.dismiss();
      this.router.navigate(['tabs/posts']);
    },
    error => 
    {
      loading.dismiss();
      window.alert(error.error.text);
    });
  }
}
