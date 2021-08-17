import { Injectable } from '@angular/core';

import { DataService } from './data.service';

import { TagRegister } from 'src/app/shared/interfaces/tag-register';

@Injectable({
  providedIn: 'root'
})
export class NotificationsControllerService {
  public _registerTagsApi = '/api/notifications/registerTags';
  public _unsubscribeApi = '/api/notifications/unsubscribe';
  public _notifyForResultApi = '/api/notifications/registerForResults';
  constructor(public _dataService: DataService){}

  /**
   * method: registerTags.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param tagRegisterParam `TagRegisterVM`.
   */
  public registerTags(
      tagRegisterParam: TagRegister
  ) {
    return this._dataService.post(
      `${this._registerTagsApi}`,
      {
          InstallationId: tagRegisterParam.installationId,
          RegistrationId: tagRegisterParam.registrationId,
          Tags: tagRegisterParam.tags,
          Platform: tagRegisterParam.platform
      },
      {}
    );
  }

  /**
   * method: unsubscribe.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param installationIdParam `string`.
   */
  public unsubscribe(installationIdParam: string) {
    let installationId = installationIdParam;
    return this._dataService.put(
      `${this._unsubscribeApi}`+ "?installationId=" + installationId,
      {
      },
      {});
  }

  /**
   * method: notifyForResult.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param userIdParam `string`.
   */
  public notifyForResult(userIdParam: string) {
    return this._dataService.get(
      `${this._notifyForResultApi}`,
      {
          userId: userIdParam
      });
  }
}
