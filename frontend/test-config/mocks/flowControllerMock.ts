
import { Observable } from 'rxjs';
import { PagingVM } from '../../src/viewModels/pagingVM';
import { CardVM } from '../../src/viewModels/cardVM';
import { UserViewCardVM } from '../../src/viewModels/userViewCardVM';
export class FlowControllerMock {
    public list(
        pagingParam: PagingVM,
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
