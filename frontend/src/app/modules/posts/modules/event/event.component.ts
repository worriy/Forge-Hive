import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { CreationUtils } from 'src/app/shared/interfaces/posts/creation-utils';
import { CreateEvent } from 'src/app/shared/interfaces/posts/event/create-event';

export enum EventSteps {
  CREATE_QUESTION = 0,
  CREATE_SETTINGS = 1,
  PREVIEW = 2
};
@Component({
  selector: 'app-event',
  templateUrl: './event.component.html',
  styleUrls: ['./event.component.scss'],
})
export class EventComponent implements OnInit {
  public step: EventSteps = EventSteps.CREATE_QUESTION;

  public steps = EventSteps;

  private userId: string;
  public createEventVm: CreateEvent;
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
    this.createEventVm = {
      authorId: this.userId
    };
    this.utils = {pictureSelected: false};
  }

  applyChanges(event: CreateEvent) {
    this.createEventVm = event;
  }

  setPicture(picture: any) {
    this.picture = picture;
  }

  onDiscard() {
    if (this.step === EventSteps.PREVIEW) {
      this.previousStep();
      return;
    }
    this.router.navigate(['../../newPost'], { relativeTo: this.activatedRoute });
  }

  nextStep() {
    switch(this.step) {
      case EventSteps.CREATE_QUESTION: 
        if (!this.createEventVm.content) {
          this.showMissingDataToast();
          break;
        }
        this.step++;
        break;
      case EventSteps.CREATE_SETTINGS: 
        if (!(this.createEventVm.publicationDate && this.createEventVm.endDate && this.createEventVm.targetGroupsIds)) {
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
    if (this.step === EventSteps.CREATE_QUESTION) {
      this.onDiscard();
      return;
    }
    this.step--;
  }
}
