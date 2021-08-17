import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MoodCardComponent } from './components/mood-card/mood-card.component';
import { QuoteCardComponent } from './components/quote-card/quote-card.component';
import { TranslateModule } from '@ngx-translate/core';
import { IdeaCardComponent } from './components/idea-card/idea-card.component';
import { QuestionCardComponent } from './components/question-card/question-card.component';
import { ReportingCardComponent } from './components/reporting-card/reporting-card.component';
import { SurveyCardComponent } from './components/survey-card/survey-card.component';
import { SuggestionCardComponent } from './components/suggestion-card/suggestion-card.component';
import { EventCardComponent } from './components/event-card/event-card.component';
import { SurveyQuestionCardComponent } from './components/survey-question-card/survey-question-card.component';
import { SurveyQuestionReportCardComponent } from './components/survey-question-report-card/survey-question-report-card.component';
import { SurveyReportCardComponent } from './components/survey-report-card/survey-report-card.component';



@NgModule({
  declarations: [
    IdeaCardComponent,
    QuestionCardComponent,
    ReportingCardComponent,
    SurveyCardComponent,
    SuggestionCardComponent,
    EventCardComponent,
    SurveyQuestionCardComponent,
    SurveyQuestionReportCardComponent,
    SurveyReportCardComponent,
    MoodCardComponent,
    QuoteCardComponent
  ],
  imports: [
    CommonModule,
    TranslateModule.forChild()
  ],
  exports: [
    IdeaCardComponent,
    QuestionCardComponent,
    ReportingCardComponent,
    SurveyCardComponent,
    SuggestionCardComponent,
    EventCardComponent,
    SurveyQuestionCardComponent,
    SurveyQuestionReportCardComponent,
    SurveyReportCardComponent,
    MoodCardComponent,
    QuoteCardComponent
  ]
})
export class SharedModule { }
