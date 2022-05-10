import { Injectable } from '@angular/core';

import { DataService } from './data.service';

import { Answer } from 'src/app/shared/interfaces/posts/answer';

@Injectable({
  providedIn: 'root'
})
export class AnswerControllerService {
  public _createApi = '/api/answer';
  public _discardApi = '/api/answer/discard';
  public _answeredCardApi = '/api/answer/answeredCard';
  public _getChoiceMoodIdApi = '/api/answer/getChoiceMoodId';
  public _deleteAnswerApi = '/api/answer';

  constructor(public _dataService: DataService){

  }

  /**
   * method: create.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param answerParam `AnswerVM`.
   */
  public create(answerParam: Answer) {
    let idUser = answerParam.idUser;
    let idCard = answerParam.idCard;
    let idChoice = answerParam.idChoice
    return this._dataService.post(
      `${this._createApi}`,
      {
          idUser,idCard,idChoice
      },
      {});
  }

  /**
   * method: answeredCard.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param cardIdParam `number`.
   * @param userProfileIdParam `number`.
   */
  public answeredCard(
      cardIdParam: string,
      userProfileIdParam: string
  ) {
    return this._dataService.get(
      `${this._answeredCardApi}/${cardIdParam}/${userProfileIdParam}`,
      {}
    );
  }

  public getChoiceMoodId(nameChoice: string) {
    return this._dataService.get(
      `${this._getChoiceMoodIdApi}/${nameChoice}`,
      {}
    );
  }

  public delete(
    cardId: string,
    userProfileId: string
  ) {
    return this._dataService.delete(
      `${this._deleteAnswerApi}/${cardId}/${userProfileId}`,
      {}
    );
  }
}
