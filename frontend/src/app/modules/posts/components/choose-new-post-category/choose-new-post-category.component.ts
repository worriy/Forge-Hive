import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ModalController } from '@ionic/angular';
import { CreateQuestionComponent as EventComp } from '../../modules/event/create-question/create-question.component';
import { CreateQuestionComponent as IdeaComp } from '../../modules/idea/create-question/create-question.component';
import { CreateQuestionComponent as QuestionComp } from '../../modules/question/create-question/create-question.component';
import { CreateQuestionComponent as SuggestionComp } from '../../modules/suggestion/create-question/create-question.component';
import { CreateQuestionComponent as SurveyComp } from '../../modules/survey/create-question/create-question.component';
import { CreateQuestionComponent as QuoteComp } from '../../modules/quote/create-question/create-question.component';

@Component({
  selector: 'app-choose-new-post-category',
  templateUrl: './choose-new-post-category.component.html',
  styleUrls: ['./choose-new-post-category.component.scss'],
})
export class ChooseNewPostCategoryComponent implements OnInit {

  // Get role of the User
  private role: string;

  constructor(
    public router: Router,
    public _modalCtrl: ModalController,
    private activatedRoute: ActivatedRoute
  ) {
    this.role = localStorage.getItem('roles');
  }

  ngOnInit() {
  }

  /**
   * method: onShowPostCreationModal
   * That method is a blank method.
   */
  public async onShowCreateModal(type: string) {
    let createPostModal: HTMLIonModalElement = null;
    switch(type)
    {
      case "Idea":
        this.router.navigate(['../idea'], { relativeTo: this.activatedRoute });
        break;
      case "Question":
        this.router.navigate(['../question'], { relativeTo: this.activatedRoute });
        break;
      case "Event":
        this.router.navigate(['../event'], { relativeTo: this.activatedRoute });
        break;
      case "Survey":
        this.router.navigate(['../survey'], { relativeTo: this.activatedRoute });
        break;
      case "Quote":
        this.router.navigate(['../quote'], { relativeTo: this.activatedRoute });
        break;
      case "Suggestion":
        this.router.navigate(['../suggestion'], { relativeTo: this.activatedRoute });
        break;
      default: console.log("nothing to do");
        return;
    }
  }

  


  /**
   * method: onCancel
   * That method is a blank method.
   */
  public onCancel() {
    this.router.navigate(['..'], { relativeTo: this.activatedRoute });
  }
}
