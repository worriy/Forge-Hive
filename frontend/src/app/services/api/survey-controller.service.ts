import { Injectable } from '@angular/core';
import { ToastController } from '@ionic/angular';
import { Observable } from 'rxjs';

import { DataService } from './data.service';

import { CreateSurvey } from 'src/app/shared/interfaces/posts/survey/create-survey';
import { EditableSurvey } from 'src/app/shared/interfaces/posts/survey/editable-survey';
import { EditSurvey } from 'src/app/shared/interfaces/posts/survey/edit-survey';
import { PictureVM } from 'src/app/shared/interfaces/posts/pictureVM';
import { Survey } from 'src/app/shared/interfaces/posts/survey/survey';


@Injectable({
  providedIn: 'root'
})
export class SurveyControllerService {
  public _createApi = '/api/survey/create';
  public _getEditableSurveyApi = '/api/survey/getEditableSurvey';
  public _updateApi = '/api/survey/update';
  public _getDefaultPictureApi = '/api/survey/getDefaultPicture';
  public _getSurveyquestionsApi = '/api/survey/getSurveyquestions';
  public _getSurveyReportsQuestionsApi = '/api/survey/getSurveyReportsQuestions';

  constructor(
    public _dataService: DataService,
    public _toastCtrl: ToastController
  ) {}
    
  public create(surveyParam: CreateSurvey) {
    let questionsSurvey: {content: string, choices: {name: String}[], picture: {PicBase64: string}}[] = [];
    let choicesQuestion: {name: String}[] = [];
    let picturesQuestion: {PicBase64: string} = ({PicBase64: ""});
    /*for(let j=0; j<surveyParam.questions.length; j++){
      choicesQuestion = [];
      picturesQuestion = ({PicBase64: ""});
      for(let i=0; i < surveyParam.questions[j].choices.length ; i++) {
        choicesQuestion.push({name: surveyParam.questions[j].choices[i]});
      }

      if(surveyParam.questions[j].picture.PicBase64 != null){
        //picturesQuestion.push({PicBase64: surveyParam.questions[j].picture.PicBase64})
        let picture = surveyParam.questions[j].picture.PicBase64.replace('data:image/jpeg;base64,','');
        picturesQuestion.PicBase64 = picture;
      } else {
        //picturesQuestion.push({PicBase64: null});
        picturesQuestion.PicBase64 = null;
      }
      questionsSurvey.push({content: surveyParam.questions[j].content, choices: choicesQuestion, picture: picturesQuestion});
    }*/

    return this._dataService.post(
      `${this._createApi}`,
      {
        ...surveyParam,
        questions: questionsSurvey
      },
      {});
  }


  public getEditableSurvey(surveyIdParam: number): Observable<EditableSurvey> {
    return this._dataService.get(
      `${this._getEditableSurveyApi}`,
      {
        surveyId: JSON.stringify(surveyIdParam)
      }
    );
  }
  
  public update(surveyParam: EditSurvey) {
      let id = surveyParam.id;
      return this._dataService.put(
        `${this._updateApi}`,
        {
          ...surveyParam
        },
        {}
      ).subscribe(()  =>{
        let surveyWithPicture;
        
        this.getEditableSurvey(id).subscribe(res => {
          surveyWithPicture = res;
        });
      },
      error =>
      { 
        window.alert("An error occured while updating the post, please try again");
      }
    );
  }
  
  public getDefaultPicture(): Observable<PictureVM> {
    return this._dataService.get(
      `${this._getDefaultPictureApi}`,
      {});
  }

  public getSurveyquestions(surveyIdParam: string): Observable<Array<Survey>> {
    return this._dataService.get(
      `${this._getSurveyquestionsApi}`,
      {
        surveyId: surveyIdParam
      });
  }

  public getSurveyReportsquestions(surveyIdParam: string){
    return this._dataService.get(
      `${this._getSurveyReportsQuestionsApi}`,
      {
          surveyReportId: surveyIdParam
      });
  }
}
