import { Component } from '@angular/core';
import { Platform } from '@ionic/angular';
import { StatusBar } from '@ionic-native/status-bar/ngx';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';

import { NetworkProviderHelper } from './services/helpers/network-provider.service';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

// TODO: 
/*
  -done 1. Plug network connect/disconnect to observable ('connected') of networkService instead of using Ionic 3 Events module
  2. Navigate using angular router instead of Ionic NavController
*/

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
})
export class AppComponent {
  constructor(
    private platform: Platform,
    private statusBar: StatusBar,
    private splashscreen: SplashScreen,
    private router: Router,
    private translate: TranslateService,
    private networkHelper: NetworkProviderHelper
  ) {
    this.platform.ready().then(() => {
      this.translate.setDefaultLang('Fr');
      this.translate.setDefaultLang('En');
      this.translate.use('Fr');
      this.translate.use('En');
      this.statusBar.backgroundColorByHexString('#4324B0');
      this.splashscreen.hide();
      // Initialize network status handler
      this.networkHelper.initializeNetworkEvents();
      this.handleNetworkEvents();
      //this.router.navigateByUrl('/welcome');
    })
  }

  /**
   * Act on network status change
   */
  handleNetworkEvents() {
    this.networkHelper.connected.subscribe(connected => {
      if (connected) {
        alert('network connected!');
      } else {
        alert('network was disconnected :-(');
      }
    })
  }
}
