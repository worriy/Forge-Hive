import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { DataService } from './data.service';

import { PictureVM } from 'src/app/shared/interfaces/posts/pictureVM';

@Injectable({
  providedIn: 'root'
})
export class PictureControllerService {
  public _getApi = '/api/picture';
  public _createApi = '/api/picture';
  //public _uploadImage = '/api/image/upload';
  constructor(public _dataService: DataService) {}

  /**
   * method: get.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param pictureIdParam `number`.
   * @returns `PictureVM>`.
   */
  public get(pictureIdParam: string): Observable<PictureVM> {
    return this._dataService.get(
      `${this._getApi}/${pictureIdParam}`,
      {}
    );
  }

  /**
   * method: create.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param pictureParam `PictureVM`.
   */
  public create(pictureParam: PictureVM) {
    let picture = pictureParam.picture.replace('data:image/jpeg;base64,','');
    return this._dataService.post(
      `${this._createApi}`,
      {
        picture
      },
      {}
    );
  }
}
