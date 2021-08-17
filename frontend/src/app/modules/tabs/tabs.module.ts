import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TabsRoutingModule } from './tabs-routing.module';
import { IonicModule } from '@ionic/angular';
import { TabsComponent } from './tabs.component';
import { TranslateModule } from '@ngx-translate/core';



@NgModule({
  declarations: [TabsComponent],
  imports: [
    TabsRoutingModule,
    IonicModule,
    CommonModule,
    TranslateModule.forChild()
  ]
})
export class TabsModule { }
