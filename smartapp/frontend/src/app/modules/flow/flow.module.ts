import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlowRoutingModule } from './flow-routing.module';
import { IonicModule } from '@ionic/angular';
import { FlowComponent } from './flow.component';
import { TranslateModule } from '@ngx-translate/core';
import { SwingModule } from 'angular2-swing';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    FlowComponent
  ],
  imports: [
    IonicModule,
    FlowRoutingModule,
    CommonModule,
    TranslateModule.forChild(),
    SwingModule,
    SharedModule
  ]
})
export class FlowModule { }
