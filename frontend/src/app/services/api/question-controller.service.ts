import { Injectable } from '@angular/core';
import { ToastController } from '@ionic/angular';
import { Observable } from 'rxjs';

import { DataService } from './data.service';

import { CreateQuestion } from 'src/app/shared/interfaces/posts/question/create-question';
import { EditableQuestion } from 'src/app/shared/interfaces/posts/question/editable-question';
import { EditQuestion } from 'src/app/shared/interfaces/posts/question/edit-question';
import { PictureVM } from 'src/app/shared/interfaces/posts/pictureVM';


@Injectable({
  providedIn: 'root'
})
export class QuestionControllerService {
  public _createApi = '/api/question/create';
  public _getEditableQuestionApi = '/api/question/getEditableQuestion';
  public _updateApi = '/api/question/update';
  public _getDefaultPictureApi = '/api/question/getDefaultPicture';
  constructor(
    public _dataService: DataService,
    public _toastCtrl: ToastController
  ) {}
    
    public create(questionParam: CreateQuestion) {
      return this._dataService.post(
        `${this._createApi}`,
        {
          content: questionParam.content,
          publicationDate: questionParam.publicationDate,
          endDate: questionParam.endDate,
          targetGroupsIds: questionParam.targetGroupsIds,
          authorId: questionParam.authorId,
          pictureId: questionParam.pictureId,
          choices: questionParam.choices
        },
        {}
      );
    }


    public getEditableQuestion(questionIdParam: string): Observable<EditableQuestion> {
      return this._dataService.get(
        `${this._getEditableQuestionApi}`,
        {
            questionId: questionIdParam
        }
      );
    }
    
    public update(questionParam: EditQuestion) {
      console.log(questionParam);
      let id = questionParam.id;
      return this._dataService.put(
        `${this._updateApi}`,
        {
          ...questionParam
        },
        {}
      ).subscribe(data  =>{
        let questionWithPicture;
        
        this.getEditableQuestion(id).subscribe(res => {
          questionWithPicture = res;
        });
          
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
