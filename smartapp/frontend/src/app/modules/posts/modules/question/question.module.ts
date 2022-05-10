import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuestionRoutingModule } from './question-routing.module';
import { CreateQuestionComponent } from './create-question/create-question.component';
import { CreateSettingsComponent } from './create-settings/create-settings.component';
import { PreviewComponent } from './preview/preview.component';
import { QuestionComponent } from './question.component';
import { CreateAnswersComponent } from './create-answers/create-answers.component';
import { EditQuestionModalComponent } from './edit-question-modal/edit-question-modal.component';
import { IonicModule } from '@ionic/angular';
import { TranslateModule } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Camera } from '@ionic-native/camera/ngx';

@NgModule({
  declarations: [
    QuestionComponent,
    CreateQuestionComponent,
    CreateSettingsComponent,
    CreateAnswersComponent,
    EditQuestionModalComponent,
    PreviewComponent
  ],
  imports: [
    QuestionRoutingModule,
    CommonModule,
    IonicModule,
    TranslateModule.forChild(),
    ReactiveFormsModule
  ],
  providers: [
    Camera
  ]
})
export class QuestionModule { }
