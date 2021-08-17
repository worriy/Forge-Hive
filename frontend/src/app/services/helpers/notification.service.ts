import { Injectable } from '@angular/core';
import { ModalController, Platform, ToastController } from '@ionic/angular';
import { Device } from '@ionic-native/device/ngx';

import { GroupControllerService } from '../api/group-controller.service';
import { NotificationsControllerService } from '../api/notifications-controller.service';
import { UserControllerService } from '../api/user-controller.service';

import { TagRegister } from 'src/app/shared/interfaces/tag-register';
import { GroupDetailsComponent } from 'src/app/modules/account/modules/manage-groups/group-details/group-details.component';
import { ManageGroupsComponent } from 'src/app/modules/account/modules/manage-groups/manage-groups.component';
import { PostDetailsModalComponent } from 'src/app/modules/posts/components/post-details-modal/post-details-modal.component';
import { PostsComponent } from 'src/app/modules/posts/posts.component';

//declare WindowsAzure plugin object
declare var WindowsAzure: any
//declare phonegap push plugin object.
declare var PushNotification: any

//Notifications possible actions Enumeration
enum NotificationActions {
  group_details,
  group_list,
  post_details,
  post_list        
}

@Injectable({
  providedIn: 'root'
})
export class NotificationHelper {
  constructor(  
    public _userController: UserControllerService,
    public _notifCtrl: NotificationsControllerService,
    public _groupCtrl: GroupControllerService,
    public _toastCtrl: ToastController,
    private device: Device,
    private platform: Platform,
    private _modalCtrl: ModalController 
  )
  { }

  public pushRegistration: any = null;

  /**
   * Method creating the handlers for the push notifications events: registration, notification and error.
   */
  public handlePushNotifications()
  {
    // FIXME: Reactivate if needed
    // //initialize the pushNotification hub
    // this.pushRegistration = PushNotification.init
    // ({
    //   //TODO: senderId in a config file.
    //   android: { alert: 'false', sound:'false', vibrate: 'true', badge: 'false', senderID: "473022323349" },
    //   ios: { alert: 'true', badge: 'true', sound: 'true' }
      
    // });
    // //Handle the registration event. 
    // this.pushRegistration.on('registration', (data) => {this.registerForPushNotifications(data);});

    // //handle the receiving of a noification.
    // this.pushRegistration.on('notification', (data, d2) => 
    // {
    //   //if we receive the notification while the app is closed or sleeping
    //   //wait until the platform is ready and launch directly the action.
    //   if(data.additionalData.coldstart)
    //     this.platform.ready().then(() => 
    //     {
    //       this.notificationAction(data);
    //     });
    //   //if we receive the notification while the app is open.
    //   else
    //     this.notificationReceived(data, d2);
    // })

    // //when receiving an error from the push notification service.
    // this.pushRegistration.on('error', function (error) {
    // });
  }
   
  /**
   * Perform the handling on an incoming push notification 
   * @param data Push notification data
   * @param d2 Extra data (not used)
   */
  public async notificationReceived(data: any, d2: any)
  {

    //get the notification's corresponding action. 
    var action = data.additionalData.action;

    //show the notification in a Toast and create the action button
    let toast = await  this._toastCtrl.create({
      message: data.message,
      duration: 5000,
      position: 'top',
      //showCloseButton: true,
      //if the action is no_action, show button ok to close the toast, if else, show the see button.
      //closeButtonText: action == "no_action" ? "Ok" : "See",
      cssClass: "ButtonToast"
    })

    toast.present();

    //when the toast is closed, if it's closed by the user clicking on the button, launch the action, if not, nothing.
    // TODO: CHECK ACTUAL RESULT OF EVENT
    toast.onDidDismiss<string>().then((event) => 
    {
      // FIXME: This probably won't work, check event type
      console.log(event);
      if(event.data == 'close')
        this.notificationAction(data);
      else  
        console.log("Notification Toast dismissed naturally, no action");
    });
  }

  /**
   * Apply the action specified by the notification
   * @param data notification data
   */
  public async notificationAction(data: any)
  {

    //retrieving the notification's corresponding action.
    var action = data.additionalData.action;
    var id = data.additionalData.id;

    //Creating the modal to open depending on the notification's action
    var notifModal;
    switch(action)
    {
      case "group_details":
        this._groupCtrl.get(id).subscribe(async res => 
        {
          notifModal = await this._modalCtrl.create({
            component: GroupDetailsComponent,
            componentProps: { group: res }
          });
        });
        return;
      case "group_list":
        notifModal = await this._modalCtrl.create({
          component: ManageGroupsComponent
        });
        break;
      case "post_details":
        notifModal = await this._modalCtrl.create({
          component: PostDetailsModalComponent,
          componentProps: { post: id }
        });
        break;
      case "post_list":
        notifModal = await this._modalCtrl.create({
          component: PostsComponent
        });
        break;
      default: console.log("nothing to do");
        return;
    }
    notifModal.present();
  }

  /**
   * Register for Push notification depending on the platform, Calls api to register User id as a tag on the subscription
   * @param data Push notification registration datas
   */
  public registerForPushNotifications(data: any)
  {

    //retrieve the platform of the device
    var platform = this.device.platform;

    //initialize the WindowsAzure client.
    //var client= new WindowsAzure.MobileServiceClient('https://meteometer.azurewebsites.net');

    var client= new WindowsAzure.MobileServiceClient('https://hiveredlab.azurewebsites.net/');

    //get the installationId to prepare the tag registration that has to be done from the back end
    var installationId = client.push.installationId;
    //keep it in the local storage for later use (unregister).
    localStorage.setItem('PushNotificationsInstallation', JSON.stringify(installationId));

    // Get the handle returned during registration (this is the subscriptionId).
    var handle = data.registrationId;

    //list of the tags to apply to this subscription (the user's id).
    var arrTags = [JSON.parse(localStorage.getItem('userProfileId')).toString()];
    // Set the device-specific message template.
    //if Android.
    if (platform === "Android" || platform === "android") 
    {
      // Register for GCM notifications.
      client.push.register('gcm', handle, 
      {
        mytemplate: { body: { data: { message: "{$(messageParam)}" , action: "{$(actionParam)}" } } }
      });
    } 
    //if IOS.
    else if (platform === 'iOS') 
    {
      // Register for notifications.
      client.push.register('apns', handle, 
      {
          mytemplate: { body: { aps: { alert: "{$(messageParam)}", action: "{$(actionParam)}", id: "{$(idParam)}" } } }
      });
    } 
    //if Windows phone (will come to disappear)
    else if (platform === 'windows') 
    {
      // Register for WNS notifications.
      client.push.register('wns', handle, 
      {
        myTemplate: {
          body: '<toast><visual><binding template="ToastText01"><text id="1">$(messageParam)</text></binding></visual></toast>',
          headers: { 'X-WNS-Type': 'wns/toast' } }
      });
    } 
    //if on a browser (not enabled yet).
    else if (this.device.platform === 'browser') 
    {
      console.log("registering for push notifications from a browser.");
    }

    //calling the API to register the tags to the subscription.
    var register: TagRegister = {
      installationId,
      registrationId: handle,
      tags: arrTags,
      platform
    }
    
    this._notifCtrl.registerTags(register).subscribe();
  }

  /**
   * Unsubscribe from the notification service
   */
  public unsubscribe()
  {
    this.pushRegistration.unregister(function(){
    }, function(){
    });
    //unsubscribe from the push notifications and remove the item from the local storage.
    this._notifCtrl.unsubscribe(JSON.parse(localStorage.getItem('PushNotificationsInstallation'))).subscribe(res => 
    {
      localStorage.removeItem('PushNotificationsInstallation');
    },
    error => 
    {
      console.log("Error " + error);
    }); 
  }

  /**
   * Call the Notification Service to notify the user of his yesterday expired posts 
   */
  public notifyForResults()
  {
    this._notifCtrl.notifyForResult(localStorage.getItem('authenticatedUser')).subscribe(() => {
      console.log("Notifying for results Helper");
    });
  }
}
