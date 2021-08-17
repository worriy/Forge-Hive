import { Injectable } from '@angular/core';
import { Network } from '@ionic-native/network/ngx';
import { Subject } from 'rxjs';

export enum ConnectionStatusEnum {
  Online,
  Offline
}

@Injectable({
  providedIn: 'root'
})
export class NetworkProviderHelper {
  previousStatus;

  public connected = new Subject<boolean>();

  constructor(public network: Network)
  {
    console.log('Hello NetworkProvider Provider');
    this.previousStatus = ConnectionStatusEnum.Online;
  }

  // TODO: Check use of EVENTS to replace with observables (if really necessary)
  public initializeNetworkEvents(): void {
    this.network.onDisconnect().subscribe(() => {
      if (this.previousStatus === ConnectionStatusEnum.Online) {
        this.connected.next(false);
      }
      this.previousStatus = ConnectionStatusEnum.Offline;
    });

    this.network.onConnect().subscribe(() => {
      if (this.previousStatus === ConnectionStatusEnum.Offline) {
        this.connected.next(true);
      }
      this.previousStatus = ConnectionStatusEnum.Online;
    });
  }
}
