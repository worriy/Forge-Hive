import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IdeaRoutingModule } from './idea-routing.module';
import { Camera } from '@ionic-native/camera/ngx';
import { IonicModule } from '@ionic/angular';
import { TranslateModule } from '@ngx-translate/core';
import { CreateQuestionComponent } from './create-question/create-question.component';
import { CreateSettingsComponent } from './create-settings/create-settings.component';
import { EditIdeaModalComponent } from './edit-idea-modal/edit-idea-modal.component';
import { PreviewComponent } from './preview/preview.component';
import { IdeaComponent } from './idea.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    IdeaComponent,
    CreateQuestionComponent,
    CreateSettingsComponent,
    EditIdeaModalComponent,
    PreviewComponent
  ],
  imports: [
    IdeaRoutingModule,
    CommonModule,
    IonicModule,
    TranslateModule.forChild(),
    ReactiveFormsModule
  ],
  providers: [
    Camera
  ]
})
export class IdeaModule { }
