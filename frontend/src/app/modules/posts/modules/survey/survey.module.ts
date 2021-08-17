import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SurveyRoutingModule } from './survey-routing.module';
import { CreateQuestionComponent } from './create-question/create-question.component';
import { PreviewComponent } from './preview/preview.component';
import { SurveyComponent } from './survey.component';
import { EditSurveyModalComponent } from './edit-survey-modal/edit-survey-modal.component';
import { IonicModule } from '@ionic/angular';
import { TranslateModule } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Camera } from '@ionic-native/camera/ngx';

@NgModule({
  declarations: [
    SurveyComponent,
    CreateQuestionComponent,
    EditSurveyModalComponent,
    PreviewComponent
  ],
  imports: [
    SurveyRoutingModule,
    CommonModule,
    IonicModule,
    TranslateModule.forChild(),
    ReactiveFormsModule
  ]
  ,
  providers: [
    Camera
  ]
})
export class SurveyModule { }
