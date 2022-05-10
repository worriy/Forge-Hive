
import { Observable } from 'rxjs';
import { PictureVM } from '../../src/viewModels/pictureVM';
export class PictureControllerMock {
    public get(
        pictureIdParam: number
    ): Observable<PictureVM> {
      return Observable.of();
    }

    public create(
        pictureParam: PictureVM
    ): Observable<any> {
      return Observable.of();
    }

}
