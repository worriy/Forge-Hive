import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { HighlightsControllerService } from 'src/app/services/api/highlights-controller.service';

@Component({
  selector: 'highlights-best-contributor',
  templateUrl: './best-contributor.component.html',
  styleUrls: ['./best-contributor.component.scss'],
})
export class BestContributorComponent implements OnInit {

  @Input() refresher: Subject<void>;

  public bestContributor:any;

  constructor(
    public _highlightsController: HighlightsControllerService
    
  ) {
  }

  ngOnInit(){
    this.onGetBestContributor();
    this.refresher.subscribe(() => this.onGetBestContributor());
  }

  /**
   * Retrieves the best contributor
   */
  public async onGetBestContributor() {
    return this._highlightsController.getBestContributor(
      ).toPromise().then(data => {
        this.bestContributor = data; 
      });
  }

}
