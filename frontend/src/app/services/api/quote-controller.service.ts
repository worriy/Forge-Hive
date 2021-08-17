import { Injectable } from '@angular/core';
import { ToastController } from '@ionic/angular';
import { Observable } from 'rxjs';

import { DataService } from './data.service';

import { CreateQuote } from 'src/app/shared/interfaces/posts/quote/create-quote';
import { EditableQuote } from 'src/app/shared/interfaces/posts/quote/editable-quote';
import { EditQuote } from 'src/app/shared/interfaces/posts/quote/edit-quote';
import { PictureVM } from 'src/app/shared/interfaces/posts/pictureVM';


@Injectable({
  providedIn: 'root'
})
export class QuoteControllerService {
  public _createApi = '/api/quote/create';
  public _getEditableQuoteApi = '/api/quote/getEditableQuote';
  public _updateApi = '/api/quote/update';
  public _getDefaultPictureApi = '/api/quote/getDefaultPicture';
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
  public create(quoteParam: CreateQuote) {
    return this._dataService.post(
      `${this._createApi}`,
      {
        ...quoteParam
      },
      {}
    );
  }
  
  public getEditableQuote(quoteIdParam: string): Observable<EditableQuote> {
    return this._dataService.get(
      `${this._getEditableQuoteApi}`,
      {
        quoteId: quoteIdParam
      }
    );
  }
  
  public update(quoteParam: EditQuote) {
    return this._dataService.put(
      `${this._updateApi}`,
      {
        ...quoteParam
      },
      {}
    ).subscribe(()  => {
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
