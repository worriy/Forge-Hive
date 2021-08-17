import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { CreationUtils } from 'src/app/shared/interfaces/posts/creation-utils';
import { CreateQuote } from 'src/app/shared/interfaces/posts/quote/create-quote';

export enum QuoteSteps {
  CREATE_QUESTION = 0,
  CREATE_SETTINGS = 1,
  PREVIEW = 2
};

@Component({
  selector: 'app-quote',
  templateUrl: './quote.component.html',
  styleUrls: ['./quote.component.scss'],
})
export class QuoteComponent implements OnInit {
  public step: QuoteSteps = QuoteSteps.CREATE_QUESTION;

  public steps = QuoteSteps;

  private userId: string;
  public createQuoteVm: CreateQuote;
  public picture: any = null;

  public utils: CreationUtils;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private toastCtrl: ToastController,
    private translateService: TranslateService
  ) { }

  ngOnInit() {
    this.userId = localStorage.getItem('userProfileId');
    this.createQuoteVm = {
      authorId: this.userId
    };
    this.utils = {pictureSelected: false};
  }

  applyChanges(event: CreateQuote) {
    this.createQuoteVm = event;
  }

  onDiscard() {
    if (this.step === QuoteSteps.PREVIEW) {
      this.previousStep();
      return;
    }
    this.router.navigate(['../../newPost'], { relativeTo: this.activatedRoute });
  }

  nextStep() {
    switch(this.step) {
      case QuoteSteps.CREATE_QUESTION: 
        if (!this.createQuoteVm.content) {
          this.showMissingDataToast();
          break;
        }
        this.step++;
        break;
      case QuoteSteps.CREATE_SETTINGS: 
        if (!(this.createQuoteVm.publicationDate && this.createQuoteVm.endDate && this.createQuoteVm.targetGroupsIds)) {
          this.showMissingDataToast();
          break;
        }
        this.step++;
        break;
      default: 
        console.warn("we should not see this...");
    }
  }

  private async showMissingDataToast() {
    const toast = await this.toastCtrl.create({
      message: this.translateService.instant('commons.missingData'),
      position: 'top',
      duration: 2000,
      color: 'danger'
    });
    toast.present();
  }

  previousStep() {
    if (this.step === QuoteSteps.CREATE_QUESTION) {
      this.onDiscard();
      return;
    }
    this.step--;
  }
}
