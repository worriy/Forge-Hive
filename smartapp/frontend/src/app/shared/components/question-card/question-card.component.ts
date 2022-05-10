import { Component, EventEmitter, Input, OnInit, Output, SimpleChange } from '@angular/core';
import { AnswerControllerService } from 'src/app/services/api/answer-controller.service';
import { FlowControllerService } from 'src/app/services/api/flow-controller.service';
import { PictureControllerService } from 'src/app/services/api/picture-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { Answer } from '../../interfaces/posts/answer';
import { CardVM } from '../../interfaces/posts/cardVM';
import { UserViewCard } from '../../interfaces/user/user-view-card';

@Component({
  selector: 'app-question-card',
  templateUrl: './question-card.component.html',
  styleUrls: ['./question-card.component.scss'],
})
export class QuestionCardComponent implements OnInit {
  
  private card_liked: string  = "";
  
  //card to display, provided by the parent template
  @Input('card') card: CardVM;

  //Get the activeCatdId from the parent template
  @Input('activeCardId')
    set activeCardId(id: string){
      //if the new active card is this card, set the style to is-card-active to launch animation
      if(this.card.id == id){
        setTimeout(() => {
          this.active = "is-card-active";
        },20);
      }
      else 
        this.active = "";
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
    public _flowController: FlowControllerService,
    private _pictureController: PictureControllerService,
    public _userController: UserControllerService
  ) {
    this.userProfileId = this._userController.getUserProfileId();
  }

  ngOnInit() {
    this.getPicture(this.card.pictureId);
    if (this.userProfileId != null) {
      if(this.card.isLiked == true){
        this.card_liked = "is-active";
      }
      else
      {
        this.card_liked = "";
      }
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

  /**
   * Retrieve this card's picture
   * @param pictureId From CardVM
   */
  getPicture(pictureId: string) {
    this._pictureController.get(pictureId).subscribe(res => {
      this.card.picture = res.picture;
    });
  }
}
