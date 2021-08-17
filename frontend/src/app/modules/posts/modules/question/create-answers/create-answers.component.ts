import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { TargetGroup } from 'src/app/shared/interfaces/groups/target-group';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { Choice } from 'src/app/shared/interfaces/posts/question/choice';
import { CreateQuestion } from 'src/app/shared/interfaces/posts/question/create-question';
import { CreateSurvey } from 'src/app/shared/interfaces/posts/survey/create-survey';

@Component({
  selector: 'question-create-answers',
  templateUrl: './create-answers.component.html',
  styleUrls: ['./create-answers.component.scss'],
})
export class CreateAnswersComponent implements OnInit {

  private isSurvey: boolean;

  private minAnswers : number = 2 ; 
  private maxAnswers : number = 5 ;
  private visibleAdd: boolean = true ; 
  public numberAnswers: Array<number>; 


  //the post's settings, 
  private settings = {
    publicationDate: new Date().toISOString(),
    endDate: new Date().toISOString(),
    targetGroupIds: []
  };

  //the user's groups
  private myGroups: TargetGroup[];
  
  // Survey
  @Input() survey: CreateSurvey;
  @Input() pictureSurvey: string;

  //the idea, coming from the previous step
  @Input() vm: CreateQuestion;
  @Output() changes = new EventEmitter<CreateQuestion>();

  public answersForm: FormGroup;

  public count_answers = 2;

  constructor(
    public _groupController: GroupControllerService,
    public _toastCtrl: ToastController,
    public _translateService: TranslateService,
    private formBuilder: FormBuilder
  ) {
    this.numberAnswers = Array(this.maxAnswers).fill(this.maxAnswers).map((x,i) => i); 
  }

  public ngOnInit() {
    if (this.survey)
      this.isSurvey = true;
    
    this.answersForm = this.formBuilder.group({
      answer1: [this.vm.choices[0], Validators.maxLength(10)],
      answer2: [this.vm.choices[1], Validators.maxLength(10)],
      answer3: [this.vm.choices[2], Validators.maxLength(10)],
      answer4: [this.vm.choices[3], Validators.maxLength(10)],
      answer5: [this.vm.choices[4], Validators.maxLength(10)]
    });

    this.count_answers = this.vm.choices.length > 2 ? this.vm.choices.length : 2;
    if (this.count_answers === 5)
      this.visibleAdd = false;

    this.answersForm.valueChanges.subscribe(data => this.emitChanges(data));
  }

  emitChanges(data: any) {
    let i = 0;
    if (!!data.answer1) {
      this.vm.choices[i] = data.answer1;
      i++;
    }
    if (!!data.answer2) {
      this.vm.choices[i] = data.answer2;
      i++;
    }
    if (!!data.answer3) {
      this.vm.choices[i] = data.answer3; 
      i++;
    }
    if (!!data.answer4) {
      this.vm.choices[i] = data.answer4;
      i++;
    }
    if (!!data.answer5) {
      this.vm.choices[i] = data.answer5;
      i++;
    }
    this.changes.emit(this.vm);
  }

  public async onToAnotherquestionSurvey(){
    // TODO: Fix for survey creation
    // Min 2 questions in A Survey
    // if(this.survey.questions.length < 2){
      
    //   // Write Min 2 answrers
    //   if(this.answers[0] != "" && this.answers[1] != "" && this.answers[0] != undefined && this.answers[1] != undefined){
    //     //this.survey.questions[this.survey.questions.length-1].choices = new Array<ChoiceModel>();
    //     for(let i = 0; i < this.maxAnswers; i++){
    //       if(this.answers[i] != "" && this.answers[i] != undefined){
    //         this.survey.questions[this.survey.questions.length-1].choices[i] = {
    //           id: null,
    //           name: this.answers[i]
    //         };
    //       }
    //     }
    //     this._nav.push('question-createQuestion', {'survey': this.survey, 'pictureSurvey': this.pictureSurvey});
    //   }else{
    //     let toastText: string ;
    //     this._translateService.get('question.minAnswers').subscribe(res => toastText = res);
    //     let toast = await this._toastCtrl.create({
    //       message: toastText,
    //       duration: 2000,
    //       position: 'middle'
    //     })
    //     toast.present();
    //   }
      
    // }else{
    //   // Write Min 2 answrers
    //   if(this.answers[0] != "" && this.answers[1] != "" && this.answers[0] != undefined && this.answers[1] != undefined){
    //     //this.survey.questions[this.survey.questions.length-1].choices = new Array<ChoiceModel>();
    //     for(let i = 0; i < this.maxAnswers; i++){
    //       if(this.answers[i] != "" && this.answers[i] != undefined){
    //         this.survey.questions[this.survey.questions.length-1].choices[i] = {
    //           id: null,
    //           name: this.answers[i]
    //         };
    //       }
    //     }
    //     this._nav.push('question-createSettings', {'survey': this.survey, 'pictureSurvey': this.pictureSurvey});
    //   }else{
    //     let toastText: string ;
    //     this._translateService.get('question.minAnswers').subscribe(res => toastText = res);
    //     let toast = await this._toastCtrl.create({
    //       message: toastText,
    //       duration: 2000,
    //       position: 'middle'
    //     })
    //     toast.present();
    //   }
    // }
  }

  async onAddNumberAnswers() {
    if (this.count_answers < 5)
      this.count_answers++;
    else {
      let toastText: string ;
      this._translateService.get('question.maxAnswers').subscribe(res => toastText = res);
      let toast = await this._toastCtrl.create({
        message: toastText,
        duration: 2000,
        position: 'middle'
      })
      toast.present();
    }

    if (this.count_answers == 5)
      this.visibleAdd = false;    
  }

}
