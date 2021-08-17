import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { DataService } from './data.service';

import { CardVM } from 'src/app/shared/interfaces/posts/cardVM';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { UserViewCard } from 'src/app/shared/interfaces/user/user-view-card';

@Injectable({
  providedIn: 'root'
})
export class FlowControllerService {
  public _listApi = '/api/flow/list';
  public _addViewApi = '/api/flow/addView';
  public _checkDeletedCardsApi = '/api/flow/checkDeletedCards';
  public _likePost = '/api/flow/like';
  public _dislike = '/api/flow/dislike';
  public _checkLikedCard = '/api/flow/checkLikedCard';
  constructor(public _dataService: DataService) {}

  /**
   * method: list.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param pagingParam `PagingVM`.
   * @param userProfileIdParam `number`.
   * @returns `CardVM>`.
   */
  public list(
      pagingParam: Paging,
      userProfileIdParam: string
  ): Observable<Array<CardVM>> {
    return this._dataService.get(
      `${this._listApi}`,
      {
          step: pagingParam.step.toString(),
          page: pagingParam.lastId.toString(),
          userProfileId: userProfileIdParam
      });
  }

  /**
   * method: addView.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param userViewCardParam `UserViewCardVM`.
   */
  public addView(
      userViewCard: UserViewCard
  ) {
    let cardId = userViewCard.cardId ; 
    let userId = userViewCard.userId;
    return this._dataService.put(
      `${this._addViewApi}`,
      {
        cardId,userId
      },
      {});
  }

  /**
   * method: checkDeletedCards.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param pagingParam `PagingVM`.
   * @param userViewCardParam `UserViewCardVM`.
   */
  public checkDeletedCards(
    pagingParam: Paging,
    userProfileIdParam: number
  ) {
    return this._dataService.get(
      `${this._checkDeletedCardsApi}`,
      {
        lastId: pagingParam.lastId.toString(),
        userProfileId: userProfileIdParam.toString()
      });
  }

  public like(userViewCard: UserViewCard) {
    let cardId = userViewCard.cardId ; 
    let userId = userViewCard.userId;
    return this._dataService.put(
      `${this._likePost}`,
      {
        cardId,userId
      },
     {});
  }

  public dislike(userViewCard: UserViewCard) {
    let cardId = userViewCard.cardId ; 
    let userId = userViewCard.userId;
    return this._dataService.put(
      `${this._dislike}`,
      {
        cardId,userId
      },
     {});
  }

  public checkLikedCard(
    cardId: string,
    userId: string
) {
    return this._dataService.get(
      `${this._checkLikedCard}`,
      {
        cardId: cardId,
        userId: userId
      });
}
}
