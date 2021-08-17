import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationHelper } from 'src/app/services/helpers/notification.service';

@Component({
  selector: 'settings-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss'],
})
export class NotificationsComponent implements OnInit {
  public notificationsSetting: boolean;
  
  constructor(
    public _notifHelper: NotificationHelper
  ) {
    switch(localStorage.getItem('notificationsPreference')){
      case "0": this.notificationsSetting = false;
        break;
      case "1": this.notificationsSetting = true;
        break;
      default: console.log("No notifications setting, that's strange..");
    }
  }

  ngOnInit() {
    
  }

  /**
   * method: onChangeSetting
   * That method is a blank method.
   */
  public onChangeSetting() {
    // TO DO
    localStorage.setItem('notificationsPreference', this.notificationsSetting ? "1" : "0");
    if(this.notificationsSetting == true)
      this._notifHelper.handlePushNotifications();
    else
      this._notifHelper.unsubscribe();
  }
}
