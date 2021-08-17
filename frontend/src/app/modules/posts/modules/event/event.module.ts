import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventRoutingModule } from './event-routing.module';
import { EventComponent } from './event.component';
import { CreateQuestionComponent } from './create-question/create-question.component';
import { CreateSettingsComponent } from './create-settings/create-settings.component';
import { EditEventModalComponent } from './edit-event-modal/edit-event-modal.component';
import { PreviewComponent } from './preview/preview.component';
import { IonicModule } from '@ionic/angular';
import { TranslateModule } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Camera } from '@ionic-native/camera/ngx';

@NgModule({
  declarations: [
    EventComponent,
    CreateQuestionComponent,
    CreateSettingsComponent,
    EditEventModalComponent,
    PreviewComponent
  ],
  imports: [
    EventRoutingModule,
    CommonModule,
    IonicModule,
    TranslateModule.forChild(),
    ReactiveFormsModule
  ],
  providers: [
    Camera
  ]
})
export class EventModule { }
