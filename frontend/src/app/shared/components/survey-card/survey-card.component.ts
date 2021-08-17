import { Component, EventEmitter, Input, OnInit, Output, SimpleChange } from '@angular/core';
import { AnswerControllerService } from 'src/app/services/api/answer-controller.service';
import { FlowControllerService } from 'src/app/services/api/flow-controller.service';
import { SurveyControllerService } from 'src/app/services/api/survey-controller.service';
import { Answer } from '../../interfaces/posts/answer';
import { CardVM } from '../../interfaces/posts/cardVM';
import { UserViewCard } from '../../interfaces/user/user-view-card';

@Component({
  selector: 'app-survey-card',
  templateUrl: './survey-card.component.html',
  styleUrls: ['./survey-card.component.scss'],
})
export class SurveyCardComponent implements OnInit {

  private card_liked: string  = "";
  public isList = false;
  //card to display, provided by the parent template
  @Input('card') card: CardVM;

  //Get the activeCatdId from the parent template
  @Input('activeCardId')
    set activeCardId(id: string){
      if(!this.isList){
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

  //Replay this card
  @Output()
  replayed: EventEmitter<String> = new EventEmitter<String>();

  //event emitted when the user answer this card
  @Output()
  answered: EventEmitter<String> = new EventEmitter<String>();

  //event emitted when the user click on Start Survey
  @Output()
  started: EventEmitter<String> = new EventEmitter<String>();

  //Css class to use by the card
  active: string;

  //user Ids
  private userProfileId: any;

  constructor(
    public _answerController: AnswerControllerService,
    public _flowController: FlowControllerService,
    public _surveyController: SurveyControllerService
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

  public onStartSurvey(idChoice: string) {
    var answer: Answer = {
      idUser: this.userProfileId,
      idCard: this.card.id,
      idChoice
    };
    //var discard = new DiscardVM(this.userProfileId, this.card.id);
    this._answerController.create(answer).subscribe(res => {
      //emit the answered event for the flow to reload the flow cards list.
      //emit the started event for the flow to reload the flow cards list.
      this.started.emit();
      //this.answered.emit();
    })
    
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

  public onReplay(){
    this._answerController.delete(this.card.id, this.userProfileId).subscribe(res => {
      this.replayed.emit();
    });
  }
}
