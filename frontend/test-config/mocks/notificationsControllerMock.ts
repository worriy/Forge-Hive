
import { Observable } from 'rxjs';
import { TagRegisterVM } from '../../src/viewModels/tagRegisterVM';
export class NotificationsControllerMock {
    public registerTags(
        tagRegisterParam: TagRegisterVM
    ): Observable<any> {
      return Observable.of();
    }

    public unsubscribe(
        installationIdParam: string
    ): Observable<any> {
      return Observable.of();
    }

    public notifyForResult(
        userIdParam: string
    ): Observable<any> {
      return Observable.of();
    }

}
