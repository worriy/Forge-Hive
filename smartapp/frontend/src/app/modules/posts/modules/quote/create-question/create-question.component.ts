import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ActionSheetController, LoadingController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { CreatePost } from 'src/app/shared/interfaces/posts/create-post';
import { CreationUtils } from 'src/app/shared/interfaces/posts/creation-utils';

@Component({
  selector: 'quote-create-question',
  templateUrl: './create-question.component.html',
  styleUrls: ['./create-question.component.scss'],
})
export class CreateQuestionComponent implements OnInit {

  private questionForm: FormGroup;

  @Input() vm: CreatePost;
  @Input() utils: CreationUtils;

  @Output() changes = new EventEmitter<CreatePost>();
  @Output() utilsChange = new EventEmitter<CreationUtils>();

  constructor(
    public router: Router,
    public _toastCtrl: ToastController,
    public _loadingCtrl: LoadingController, 
    public _actionSheetCtrl: ActionSheetController,
    public _translateService: TranslateService,
    private formBuilder: FormBuilder
  ) {
  }

  public ngOnInit(): void{

    this.questionForm = this.formBuilder.group({
      content: [this.vm.content, Validators.compose([ Validators.required, Validators.maxLength(200)])]
    });

    this.questionForm.valueChanges.subscribe(changes => this.emitChanges(changes));
  }

  emitChanges(changes: any) {
    this.vm.content = changes.content;
    this.changes.emit(this.vm)
  }

}
