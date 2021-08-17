
import { Observable } from 'rxjs';
import { TopPostVM } from '../../src/viewModels/topPostVM';
import { BestContributorVM } from '../../src/viewModels/bestContributorVM';
import { ReportVM } from '../../src/viewModels/reportVM';
import { CardVM } from '../../src/viewModels/cardVM';
export class HighlightsControllerMock {
    public getTopPosts(
    ): Observable<TopPostVM> {
      return Observable.of();
    }

    public getBestContributor(
    ): Observable<BestContributorVM> {
      return Observable.of();
    }

    public getBestPost(
    ): Observable<ReportVM> {
      return Observable.of();
    }

    public reloadCard(
        cardIdParam: number
    ): Observable<CardVM> {
      return Observable.of();
    }

}
