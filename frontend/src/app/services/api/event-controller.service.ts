import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ToastController } from '@ionic/angular';

import { DataService } from './data.service';

import { CreateEvent } from 'src/app/shared/interfaces/posts/event/create-event';
import { EditableEvent } from 'src/app/shared/interfaces/posts/event/editable-event';
import { EditEvent } from 'src/app/shared/interfaces/posts/event/edit-event';
import { PictureVM } from 'src/app/shared/interfaces/posts/pictureVM';


@Injectable({
  providedIn: 'root'
})
export class EventControllerService {
  public _createApi = '/api/event/create';
  public _getEditableEventApi = '/api/event/getEditableEvent';
  public _updateApi = '/api/event/update';
  public _deleteApi = '/api/event/delete';
  public _getDefaultPictureApi = '/api/event/getDefaultPicture';
  constructor(
    public _dataService: DataService,
    public _toastCtrl: ToastController
  ){}
  
  /**
   * method: create.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param ideaParam `CreateIdeaVM`.
   */
  public create(eventParam: CreateEvent) {
    return this._dataService.post(
      `${this._createApi}`,
      {
        content: eventParam.content,
        publicationDate: eventParam.publicationDate,
        endDate: eventParam.endDate,
        targetGroupsIds: eventParam.targetGroupsIds,
        authorId: eventParam.authorId,
        pictureId: eventParam.pictureId
      },
      {});
  }
  
  public getEditableEvent(eventIdParam: string): Observable<EditableEvent> {
    return this._dataService.get(
      `${this._getEditableEventApi}`,
      {
        eventId: eventIdParam
      });
  }
  
  public update(eventParam: EditEvent) {
    let id = eventParam.id;
    let content = eventParam.content;
    let publicationDate = eventParam.publicationDate;
    let endDate = eventParam.endDate;
    let targetGroupsIds = eventParam.targetGroupsIds;
    let pictureId = eventParam.pictureId;
    return this._dataService.put(
      `${this._updateApi}`,
      {
          id,content,publicationDate,endDate,targetGroupsIds,pictureId
      },
      {}).subscribe(data  =>{
    },error =>
    { 
      window.alert("An error occured while updating the post, please try again");
    });
  }
  
  public getDefaultPicture(): Observable<PictureVM> {
    return this._dataService.get(
      `${this._getDefaultPictureApi}`,
      {});
  }
}
