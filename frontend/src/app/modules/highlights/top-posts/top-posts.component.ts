import { typeWithParameters } from '@angular/compiler/src/render3/util';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingController, ModalController } from '@ionic/angular';
import { Subject } from 'rxjs';
import { HighlightsControllerService } from 'src/app/services/api/highlights-controller.service';
import { TopPost } from 'src/app/shared/interfaces/highlights/top-post';
import { TopPostsCardModalComponent } from '../top-posts-card-modal/top-posts-card-modal.component';

@Component({
  selector: 'highlights-top-posts',
  templateUrl: './top-posts.component.html',
  styleUrls: ['./top-posts.component.scss'],
})
export class TopPostsComponent implements OnInit {
  @Input() refresher: Subject<void>;

  public topPosts: any;
  private loading: HTMLIonLoadingElement = null;
  private hideLoading = false;

  constructor(
    public _highlightsController: HighlightsControllerService,
    public router: Router,
    public modalCtrl: ModalController,
    private _loadingCtrl: LoadingController
  ) {
  }

  ngOnInit() {
    this.showLoading();
    this.onGetTopPosts();
    this.refresher.subscribe(() => this.onGetTopPosts());
  }

  private async showLoading() {
    this.hideLoading = false;
    if (this.loading == null) {
      this.loading = await this._loadingCtrl.create({
        spinner: 'crescent',
        showBackdrop: true,
        backdropDismiss: false
      });
    }

    if (!this.hideLoading) {
      this.loading.present();
    }
  }

  private hideLoader() {
    this.hideLoading = true;
    if (this.loading !== null)
      this.loading.dismiss();
  }

  /**
   * method: onShowCard
   * That method is a blank method.
   */
  public async onShowCard(card: TopPost) {

    let cardModal = await this.modalCtrl.create({
      component: TopPostsCardModalComponent,
      componentProps: { card: card },
      cssClass: 'post-details-modal'
    });

    cardModal.present();

    //reload the top posts list when closing the modal to get newest answers and views numbers 
    const { data } = await cardModal.onWillDismiss();
    if(!!data && data.action == "survey"){
      this.router.navigate(['tabs/flow']);
    }
    this.onGetTopPosts();
  }

  /**
   * method: onGetTopPosts
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @returns A `Subscription<any>`.
   */
  public async onGetTopPosts() {
    return this._highlightsController.getTopPosts(
      ).toPromise().then(data => {
        this.topPosts = data ;
        this.hideLoader(); 
      })
  }
}
