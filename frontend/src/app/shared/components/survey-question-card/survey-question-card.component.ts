import { Component, EventEmitter, Input, OnInit, Output, SimpleChange } from '@angular/core';
import { AnswerControllerService } from 'src/app/services/api/answer-controller.service';
import { FlowControllerService } from 'src/app/services/api/flow-controller.service';
import { Answer } from '../../interfaces/posts/answer';
import { Survey } from '../../interfaces/posts/survey/survey';
import { UserViewCard } from '../../interfaces/user/user-view-card';

@Component({
  selector: 'app-survey-question-card',
  templateUrl: './survey-question-card.component.html',
  styleUrls: ['./survey-question-card.component.scss'],
})
export class SurveyQuestionCardComponent implements OnInit {

  private card_liked: string  = "";
  public isList = false;
  //card to display, provided by the parent template
  @Input('card') card: Survey;

  //Get the activeCatdId from the parent template
  @Input('activeCardId')
    set activeCardId(id: string){
      if (!this.isList){
        //if the new active card is this card, set the style to is-card-active to launch animation
        if(this.card.id == id){
          setTimeout(() => {
            this.active = "is-card-active";
          },20);
        }
        else 
          this.active = "";
      }
    }

  //event emitted when the user answer this card
  @Output()
  answered: EventEmitter<String> = new EventEmitter<String>();

  //Css class to use by the card
  active: string;

  //user Ids
  private userProfileId: any;

  constructor(
    public _answerController: AnswerControllerService,
    public _flowController: FlowControllerService
  ) {
    this.userProfileId = localStorage.getItem('userProfileId');
  }

  ngOnInit() {
    if (this.userProfileId != null) {
      this._flowController.checkLikedCard(this.card.id, this.userProfileId).subscribe(res => {
        console.log(res);
        if(res == true){
          this.card_liked = "is-active";
        }else{
          this.card_liked = "";
        }
      });
    }
  }

  ngOnChanges(changes: {[propKey: string]: SimpleChange}){
    let log: string[] = [];
    for(let propName in changes){
      let changeProp = changes[propName];
    }
  }

  /**
   * method: onAgree
   * Method to call when the user agree to the Idea
   * @param choiceId: number, the Id of the selected choice.
   * @returns A `Subscription<any>`.
   */
  public onAnswer(idChoice: string) {
    //prepare the answerVM to send to the back end
    var answer: Answer = {
      idUser: this.userProfileId,
      idCard: this.card.id,
      idChoice
    };
    //emit the answered event for the flow to reload the flow cards list.
    this.answered.emit();
    return this._answerController.create(answer).subscribe();
  }


  /**
   * method: onAdaptClass
   * That method adapt the 'active' property depending on the needed css class for the card.
   * It checks if the card is supposed to be the active card (through its Id) and apply the class if necessary
   * @param id: number, Id of the active card
   */
  public onAdaptClass(id: string) {
    //if the new active card is this card, set the style to is-card-active to launch animation
    if(this.card.id == id)
      this.active = "is-card-active";
    else 
      this.active = "";
  }

  public onAddLike(){
    let userViewCardVM : UserViewCard = {
      cardId: this.card.id,
      userId: this.userProfileId
    };
    if(this.card_liked == ""){
      this._flowController.like(userViewCardVM).subscribe(res => {
        this.card_liked = "is-active";
      }
      )
      
    }else{
      this._flowController.dislike(userViewCardVM).subscribe(res => {
        this.card_liked = "";
      });
      
    }
  }
}
