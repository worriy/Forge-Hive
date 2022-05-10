import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ServerConfigService {
  private serverUrl: string;

  constructor() {
    this.serverUrl = 'http://localhost:8080';
  }

  public getServerUrl(): string {
    return this.serverUrl;
  }
}
