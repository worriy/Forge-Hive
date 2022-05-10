
import { Observable } from 'rxjs';
import { CardVM } from '../../src/viewModels/cardVM';
import { UserViewCardVM } from '../../src/viewModels/userViewCardVM';
export class FlowControllerMock {
    public list(
        userProfileIdParam: number
    ): Observable<CardVM> {
      return Observable.of();
    }

    public addView(
        userViewCardParam: UserViewCardVM
    ): Observable<any> {
      return Observable.of();
    }

}
