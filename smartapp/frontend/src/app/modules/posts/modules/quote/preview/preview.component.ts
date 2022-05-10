import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { PostsControllerService } from 'src/app/services/api/posts-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { CreatePost } from 'src/app/shared/interfaces/posts/create-post';
import { CreationUtils } from 'src/app/shared/interfaces/posts/creation-utils';
import { FullUser } from 'src/app/shared/interfaces/user/full-user';
import { User } from 'src/app/shared/interfaces/user/user';

@Component({
  selector: 'quote-preview',
  templateUrl: './preview.component.html',
  styleUrls: ['./preview.component.scss'],
})
export class PreviewComponent implements OnInit {

  //the card to preview.
  @Input() card: CreatePost;
  @Input() picture: string;
  @Input() utils: CreationUtils;


  //the user creating the card.
  user: FullUser;

  constructor(
    public _postsController: PostsControllerService,
    public _userController: UserControllerService,
    private _loadingCtrl: LoadingController,
    private _translateService: TranslateService,
    private router: Router
  ) {
  }

  ngOnInit(){
    //get the current user informations
    this.user = this._userController.getFullInfoUser();
  }

  /**
   * Save the card
   */
  public async save() {
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

    this._postsController.create(this.card).subscribe(data => 
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
