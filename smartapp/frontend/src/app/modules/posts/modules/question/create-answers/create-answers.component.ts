import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { TargetGroup } from 'src/app/shared/interfaces/groups/target-group';
import { CreatePost } from 'src/app/shared/interfaces/posts/create-post';

@Component({
  selector: 'question-create-answers',
  templateUrl: './create-answers.component.html',
  styleUrls: ['./create-answers.component.scss'],
})
export class CreateAnswersComponent implements OnInit {


  private minAnswers : number = 2 ; 
  private maxAnswers : number = 4 ;
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

  //the idea, coming from the previous step
  @Input() vm: CreatePost;
  @Output() changes = new EventEmitter<CreatePost>();

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
    this.answersForm = this.formBuilder.group({
      answer1: [this.vm.choices[0], Validators.maxLength(10)],
      answer2: [this.vm.choices[1], Validators.maxLength(10)],
      answer3: [this.vm.choices[2], Validators.maxLength(10)],
      answer4: [this.vm.choices[3], Validators.maxLength(10)]
    });

    this.count_answers = this.vm.choices.length > 2 ? this.vm.choices.length : 2;
    if (this.count_answers === 4)
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
    this.changes.emit(this.vm);
  }

  async addNumberAnswers() {
    if (this.count_answers < 4)
      this.count_answers++;
    else {
      let toast = await this._toastCtrl.create({
        message: this._translateService.instant('question.maxAnswers'),
        duration: 2000,
        position: 'middle'
      })
      toast.present();
    }

    if (this.count_answers == 4)
      this.visibleAdd = false;    
  }

}
