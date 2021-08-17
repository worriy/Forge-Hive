import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuoteRoutingModule } from './quote-routing.module';
import { CreateQuestionComponent } from './create-question/create-question.component';
import { CreateSettingsComponent } from './create-settings/create-settings.component';
import { PreviewComponent } from './preview/preview.component';
import { QuoteComponent } from './quote.component';
import { EditQuoteModalComponent } from './edit-quote-modal/edit-quote-modal.component';
import { IonicModule } from '@ionic/angular';
import { TranslateModule } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Camera } from '@ionic-native/camera/ngx';

@NgModule({
  declarations: [
    QuoteComponent,
    CreateQuestionComponent,
    CreateSettingsComponent,
    EditQuoteModalComponent,
    PreviewComponent
  ],
  imports: [
    QuoteRoutingModule,
    CommonModule,
    IonicModule,
    TranslateModule.forChild(),
    ReactiveFormsModule
  ],
  providers: [
    Camera
  ]
})
export class QuoteModule { }
