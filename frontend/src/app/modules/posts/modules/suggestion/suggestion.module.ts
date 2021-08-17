import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SuggestionRoutingModule } from './suggestion-routing.module';
import { CreateQuestionComponent } from './create-question/create-question.component';
import { CreateSettingsComponent } from './create-settings/create-settings.component';
import { PreviewComponent } from './preview/preview.component';
import { SuggestionComponent } from './suggestion.component';
import { EditSuggestionModalComponent } from './edit-suggestion-modal/edit-suggestion-modal.component';
import { IonicModule } from '@ionic/angular';
import { TranslateModule } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Camera } from '@ionic-native/camera/ngx';

@NgModule({
  declarations: [
    SuggestionComponent,
    CreateQuestionComponent,
    CreateSettingsComponent,
    EditSuggestionModalComponent,
    PreviewComponent
  ],
  imports: [
    SuggestionRoutingModule,
    CommonModule,
    IonicModule,
    TranslateModule.forChild(),
    ReactiveFormsModule
  ],
  providers: [
    Camera
  ]
})
export class SuggestionModule { }
