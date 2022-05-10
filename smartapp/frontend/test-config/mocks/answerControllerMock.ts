
import { Observable } from 'rxjs';
import { AnswerVM } from '../../src/viewModels/answerVM';
import { DiscardVM } from '../../src/viewModels/discardVM';
export class AnswerControllerMock {
    public create(
        answerParam: AnswerVM
    ): Observable<any> {
      return Observable.of();
    }

    public discard(
        discardParam: DiscardVM
    ): Observable<any> {
      return Observable.of();
    }

    public answeredCard(
        cardIdParam: number,
        userProfileIdParam: number
    ): Observable<any> {
      return Observable.of();
    }

}
