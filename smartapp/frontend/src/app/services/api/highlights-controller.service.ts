import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { DataService } from './data.service';

import { BestContributor } from 'src/app/shared/interfaces/highlights/best-contributor';
import { TopPost } from 'src/app/shared/interfaces/highlights/top-post';
import { Report } from 'src/app/shared/interfaces/posts/report';

@Injectable({
  providedIn: 'root'
})
export class HighlightsControllerService {
  public _getTopPostsApi = '/api/highlights/getTopPosts';
  public _getBestContributorApi = '/api/highlights/getBestContributor';
  public _getBestPostApi = '/api/highlights/getBestPost';
  public _getGeneralMood = '/api/highlights/getGeneralMood';

  constructor(public _dataService: DataService) {}

  /**
   * method: getTopPosts.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @returns `TopPostVM>`.
   */
  public getTopPosts(): Observable<TopPost[]> {
    return this._dataService.get(
      `${this._getTopPostsApi}`,
      {});
  }

  /**
   * method: getBestContributor.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @returns `BestContributorVM>`.
   */
  public getBestContributor(): Observable<BestContributor> {
    return this._dataService.get(
      `${this._getBestContributorApi}`,
      {});
  }

  /**
   * method: getBestPost.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @returns `ReportVM>`.
   */
  public getBestPost(): Observable<Report> {
    return this._dataService.get(
      `${this._getBestPostApi}`,
      {});
  }

  public getGeneralMood(): Observable<Report> {
    return this._dataService.get(
      `${this._getGeneralMood}`,
      {});
  }
}
