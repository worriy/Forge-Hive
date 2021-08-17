import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-highlights',
  templateUrl: './highlights.component.html',
  styleUrls: ['./highlights.component.scss'],
})
export class HighlightsComponent implements OnInit {
  //notifier boolean to notify the best post and best contributor layouts when it has to refresh
  public refresher = new Subject<void>();
  loading: any;

  constructor(    
  ) {   
  }

  ngOnInit() {

  }

  /**
   * method: onRefresh
   * That method is a blank method.
   */
  public onRefresh(event: any) {
    this.refresher.next();
    setTimeout(() => event.target.complete(), 1500);
  }
}
