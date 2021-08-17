import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { HighlightsControllerService } from 'src/app/services/api/highlights-controller.service';

@Component({
  selector: 'highlights-general-mood',
  templateUrl: './general-mood.component.html',
  styleUrls: ['./general-mood.component.scss'],
})
export class GeneralMoodComponent implements OnInit {
  @Input() refresher: Subject<void>;

  public generalMood:any;
  private loading: any;
  private mood: string = "";

  constructor(
    public _highlightsController: HighlightsControllerService,
    public _translateService: TranslateService
  ) {
  }

  ngOnInit(){
    this.onGetGeneralMood();
    this.refresher.subscribe(() => this.onGetGeneralMood());    
  }

  /**
   * method: onGetBestContributor
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @returns A `Subscription<any>`.
   */
  public async onGetGeneralMood() {
    return this._highlightsController.getGeneralMood()
      .toPromise().then(data => {
        this.generalMood = data; 
        if(this.generalMood.moodName == "Sunglasses")
          this._translateService.get('mood.Sunglasses').subscribe(res => this.mood = res);

        if(this.generalMood.moodName == "Happy")
          this._translateService.get('mood.Happy').subscribe(res => this.mood = res);

        if(this.generalMood.moodName == "Muted")
          this._translateService.get('mood.Muted').subscribe(res => this.mood = res);

        if(this.generalMood.moodName == "Indifferent")
          this._translateService.get('mood.Indifferent').subscribe(res => this.mood = res);

        if(this.generalMood.moodName == "Sad")
          this._translateService.get('mood.Sad').subscribe(res => this.mood = res);
      });
  }
}
