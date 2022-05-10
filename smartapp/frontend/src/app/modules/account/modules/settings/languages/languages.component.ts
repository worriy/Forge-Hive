import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'settings-languages',
  templateUrl: './languages.component.html',
  styleUrls: ['./languages.component.scss'],
})
export class LanguagesComponent implements OnInit {
  public lang: any 
  private langs: any;

  
  constructor(
    public router: Router,
    public _translateService: TranslateService,
    public alertCtrl: AlertController
  ) {
    this.lang = _translateService.currentLang;

    let fr; 
    this._translateService.get('commons.fr').subscribe(res => fr = res);

    let en; 
    this._translateService.get('commons.en').subscribe(res => en = res);

    this.langs = [
        {type: 'radio', label: en, value:'En', checked: false},
        {type: 'radio', label: fr, value:'Fr', checked: false},
      ]

    
  }

  ngOnInit() {
    
  }

  public onChangeLanguages(value: string) {
    // TO DO
    if (value != this.lang){
        this._translateService.use(value).subscribe(data => {
            this.lang = this._translateService.currentLang;
            localStorage.setItem('lang',this.lang);
        });   
    }   
  }

  public async onConfirlChangeLanguages(){
    // TO DO
    for (let i = 0; i< this.langs.length; i++){
      if(this.lang == this.langs[i].value){
          this.langs[i].checked = true;
      }else {
        this.langs[i].checked = false;
      }
    }
    let alertTitle; 
    this._translateService.get('settings.languages').subscribe(res => alertTitle = res);

    let apply; 
    this._translateService.get('commons.apply').subscribe(res => apply = res);

    let cancel; 
    this._translateService.get('commons.cancel').subscribe(res => cancel = res);


    let alert = await this.alertCtrl.create({
    header: alertTitle,
    inputs: this.langs,
    buttons: [
        {
        text: cancel,
        role: 'cancel',
        handler: () => {
        }
        },
        {
        text: apply,
        handler: (data) => {
            this.onChangeLanguages(data)
        }
        }
    ]
    });
    alert.present();
  }
}
