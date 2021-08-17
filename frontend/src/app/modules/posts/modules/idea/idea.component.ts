import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateIdea } from 'src/app/shared/interfaces/posts/idea/create-idea';
import { CreationUtils } from 'src/app/shared/interfaces/posts/creation-utils';
import { ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';

export enum IdeaSteps {
  CREATE_QUESTION = 0,
  CREATE_SETTINGS = 1,
  PREVIEW = 2
};

@Component({
  selector: 'app-idea',
  templateUrl: './idea.component.html',
  styleUrls: ['./idea.component.scss'],
})
export class IdeaComponent implements OnInit {

  public step: IdeaSteps = IdeaSteps.CREATE_QUESTION;

  public steps = IdeaSteps;

  private userId: string;
  public createIdeaVm: CreateIdea;
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
    this.createIdeaVm = {
      authorId: this.userId
    };
    this.utils = {pictureSelected: false};
  }

  applyChanges(event: CreateIdea) {
    this.createIdeaVm = event;
  }

  setPicture(picture: any) {
    this.picture = picture;
  }

  onDiscard() {
    if (this.step === IdeaSteps.PREVIEW) {
      this.previousStep();
      return;
    }
    this.router.navigate(['../../newPost'], { relativeTo: this.activatedRoute });
  }

  nextStep() {
    switch(this.step) {
      case IdeaSteps.CREATE_QUESTION: 
        if (!this.createIdeaVm.content) {
          this.showMissingDataToast();
          break;
        }
        this.step++;
        break;
      case IdeaSteps.CREATE_SETTINGS: 
        if (!(this.createIdeaVm.publicationDate && this.createIdeaVm.endDate && this.createIdeaVm.targetGroupsIds)) {
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
    if (this.step === IdeaSteps.CREATE_QUESTION) {
      this.onDiscard();
      return;
    }
    this.step--;
  }

}
