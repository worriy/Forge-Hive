import { Injectable } from '@angular/core';
import { ToastController } from '@ionic/angular';
import { Observable } from 'rxjs';

import { DataService } from './data.service';

import { CreateSuggestion } from 'src/app/shared/interfaces/posts/suggestion/create-suggestion';
import { EditableSuggestion } from 'src/app/shared/interfaces/posts/suggestion/editable-suggestion';
import { EditSuggestion } from 'src/app/shared/interfaces/posts/suggestion/edit-suggestion';
import { PictureVM } from 'src/app/shared/interfaces/posts/pictureVM';


@Injectable({
  providedIn: 'root'
})
export class SuggestionControllerService {
  public _createApi = '/api/suggestion/create';
  public _getEditableSuggestionApi = '/api/suggestion/getEditableSuggestion';
  public _updateApi = '/api/suggestion/update';
  public _getDefaultPictureApi = '/api/suggestion/getDefaultPicture';
  
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
  public create(suggestionParam: CreateSuggestion) {
    return this._dataService.post(
      `${this._createApi}`,
      {
        ...suggestionParam
      },
      {}
    );
  }
  
  public getEditableSuggestion(suggestionIdParam: string): Observable<EditableSuggestion> {
    return this._dataService.get(
      `${this._getEditableSuggestionApi}`,
      {
        suggestionId: suggestionIdParam
      }
    );
  }
  
  public update(suggestionParam: EditSuggestion) {
    return this._dataService.put(
      `${this._updateApi}`,
      {
        id: suggestionParam.id,
        content: suggestionParam.content,
        publicationDate: suggestionParam.publicationDate,
        endDate: suggestionParam.endDate
      },
      {}
    ).subscribe(()  =>{
    },
    error =>
    { 
      window.alert("An error occured while updating the post, please try again");
    });
  }
  
  public getDefaultPicture(): Observable<PictureVM> {
    return this._dataService.get(
      `${this._getDefaultPictureApi}`,
      {}
    );
  }
}
