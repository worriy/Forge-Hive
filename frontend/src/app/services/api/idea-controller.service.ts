import { Injectable } from '@angular/core';
import { ToastController } from '@ionic/angular';
import { Observable } from 'rxjs';

import { DataService } from './data.service';

import { CreateIdea } from 'src/app/shared/interfaces/posts/idea/create-idea';
import { EditableIdea } from 'src/app/shared/interfaces/posts/idea/editable-idea';
import { EditIdea } from 'src/app/shared/interfaces/posts/idea/edit-idea';
import { PictureVM } from 'src/app/shared/interfaces/posts/pictureVM';


@Injectable({
  providedIn: 'root'
})
export class IdeaControllerService {
  public _createApi = '/api/idea/create';
  public _getEditableIdeaApi = '/api/idea/getEditableIdea';
  public _updateApi = '/api/idea/update';
  public _deleteApi = '/api/idea/delete';
  public _getDefaultPictureApi = '/api/idea/getDefaultPicture';
  constructor(
    public _dataService: DataService,
    public _toastCtrl: ToastController
  ) {}

  /**
   * method: create.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param ideaParam `CreateIdeaVM`.
   */
  public create(ideaParam: CreateIdea) {
    return this._dataService.post(
      `${this._createApi}`,
      {
        content: ideaParam.content,
        publicationDate: ideaParam.publicationDate,
        endDate: ideaParam.endDate,
        targetGroupsIds: ideaParam.targetGroupsIds,
        authorId: ideaParam.authorId,
        pictureId: ideaParam.pictureId
      },
      {});
  }

  /**
   * method: getEditableIdea.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param ideaIdParam `number`.
   * @returns `EditableIdeaVM>`.
   */
  public getEditableIdea(ideaIdParam: string): Observable<EditableIdea> {
    return this._dataService.get(
      `${this._getEditableIdeaApi}`,
      {
          ideaId: ideaIdParam
      });
  }
  /**
   * method: update.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param ideaParam `EditIdeaVM`.
   */
  public update(ideaParam: EditIdea) {
    let id = ideaParam.id;
    let content = ideaParam.content;
    let publicationDate = ideaParam.publicationDate;
    let endDate = ideaParam.endDate;
    let targetGroupsIds = ideaParam.targetGroupsIds;
    let pictureId = ideaParam.pictureId;
    return this._dataService.put(
      `${this._updateApi}`,
      {
        id,
        content,
        publicationDate,
        endDate,
        targetGroupsIds,
        pictureId
      },
      {}
    ).subscribe(data  =>{
      let ideaWithPicture;
      
        
    },
    error =>
    { 
      window.alert("An error occured while updating the post, please try again");
    });
  }

  /**
   * method: getDefaultPicture.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @returns `PictureVM>`.
   */
  public getDefaultPicture(
  ): Observable<PictureVM> {
      return this._dataService.get(
        `${this._getDefaultPictureApi}`,
        {});
  }
}
