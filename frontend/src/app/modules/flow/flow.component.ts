import { Component, OnDestroy, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { AnswerControllerService } from 'src/app/services/api/answer-controller.service';
import { FlowControllerService } from 'src/app/services/api/flow-controller.service';
import { SurveyControllerService } from 'src/app/services/api/survey-controller.service';
import { CardVM } from 'src/app/shared/interfaces/posts/cardVM';
import { Discard } from 'src/app/shared/interfaces/posts/discard';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { Survey } from 'src/app/shared/interfaces/posts/survey/survey';
import { UserViewCard } from 'src/app/shared/interfaces/user/user-view-card';

import { 
  Direction,
  DragEvent,
  StackConfig,
  SwingCardComponent, 
  SwingStackComponent, 
  ThrowEvent
} from 'angular2-swing';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-flow',
  templateUrl: './flow.component.html',
  styleUrls: ['./flow.component.scss'],
})
export class FlowComponent implements OnInit, OnDestroy {
  private destroy = new Subject<void>();
  private translations: string[] = [];
  private loading: HTMLIonLoadingElement = null;

  public isList = false;

  @ViewChild('myswing1') swingStack: SwingStackComponent;
  @ViewChildren('mycards1') swingCards: QueryList<SwingCardComponent>;

  @ViewChild('myswing2') swingStack2: SwingStackComponent;
  @ViewChildren('mycards2') swingCards2: QueryList<SwingCardComponent>;

  //Swing component animation stack for swipe animations
  private displayCards: {card:CardVM, index: number}[] = [];

  //list of all the flow cards
  private cards: CardVM[];

  //id of the card to display in the list of all cards
  private currentId: number; 

  //id (object id) of the displayed card
  private activeCardId: string;

  //groupName to display on the current card
  private groupNames = "";

  //connected user infos
  private appUserId: string;
  private userProfileId: any;

  //swipe animations configuration 
  public stackConfig: StackConfig;

  //Boolean to know if we have to show the loading circle when loading the cards
  private initiated: boolean;

  //Actual paging value containing the step and last known id
  private paging: Paging;

  private surveyContent: string;
  private indexQuestion: number;
  private maxQuestion: number;

  //Replay functionality
  private lenghtReplay: number;
  private indexReplay: number;
  private replayCards: {card:CardVM, index: number}[] = [];
  private isReplayed: boolean;
  private showCards: boolean;
  private stackReplayed: StackConfig;
  private swipeIndex: number = 0 ;

  constructor(
    public _flowController: FlowControllerService,
    public _answerController: AnswerControllerService,
    public _surveyController: SurveyControllerService,
    public _translateService: TranslateService,
    public router: Router,
    public _loadingCtrl: LoadingController,
    public _toastCtrl: ToastController
  ) {
    this.getTranslations();
    this.lenghtReplay = 3;
    this.indexReplay = 0;
    this.replayCards = [];
    this.isReplayed = false;
    this.showCards = false;
    //animation stack configuration
    this.stackConfig = {
      //Calcul whether or not the card should be thrown out
     throwOutConfidence: (offsetX, offsetY, element) => {
        return Math.min(Math.max(Math.abs(offsetX) / (element.offsetWidth / 1.7), Math.abs(offsetY) / (element.offsetHeight / 2)), 1);
      },
      throwOutDistance: (d) => {
        return 500;
      },
      maxRotation: 6,
      allowedDirections: [Direction.LEFT, Direction.RIGHT, Direction.UP],
    };    

    //animation stack configuration
    this.stackReplayed = {
      //Calcul whether or not the card should be thrown out
      throwOutConfidence: (offsetX, offsetY, element) => {
        return Math.min(Math.max(Math.abs(offsetX) / (element.offsetWidth / 1.7), Math.abs(offsetY) / (element.offsetHeight / 2)), 1);
      },
      throwOutDistance: (d) => {
        return 800;
      },
      maxRotation: 6,
      allowedDirections: [Direction.LEFT],
    }; 
  }

  ngOnInit(): void {
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //this.activeCardId = 0;
    this.userProfileId = localStorage.getItem('userProfileId');
    this.paging = {
      step: 10,
      lastId: 1,
    };
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  /**
   * Method called once the View initialisation ends, here used to configure the animation stack
   */
  ngAfterViewInit() {
    //initialize the cards animation stack
    if(this.cards != null){
      this.swingStack.throwin.subscribe((event: DragEvent) => {
        event.target.style.background = '#ffffff';
      });
    }

    //initialize the cards animation stack
    if(this.cards != null){
      this.swingStack2.throwin.subscribe((event: DragEvent) => {
        event.target.style.background = '#ffffff';
      });
    }
  }

  /**
   * This method is called when the app will enter the view.
   */
  async ionViewWillEnter(){
    this.getTranslations();
    //if we are loading the flow cards for the first time we want to show a loading circle
    if(!this.initiated)
    {
      this.initiated = true;
      //launch loading
      this.loading = await this._loadingCtrl.create({
        message: this.translations['commons.loadingPleaseWait'],
        spinner: 'crescent',
        showBackdrop: false,
        backdropDismiss: true
      }) 
      this.loading.present()

      //start the card retrieving and dismiss the loading circle when done.
      this.onGetCards(this.paging, this.userProfileId).then(res => {
        this.loading.dismiss();
        this.onAdaptCardsStack();
      }, error =>{
        this.loading.dismiss();
      });

      
    }
    //if it's not the first time, we just want to reload the list without stopping the user from interacting.
    else {
      this.cards = [];
      this.paging = {
        step: 10,
        lastId: 1,
      }
      this.onGetCards(this.paging, this.userProfileId).then(_ => this.onAdaptCardsStack(), error =>{
        this.loading.dismiss();
      });
    }
      
  }

  private async getTranslations() {
    this._translateService.stream([
      'commons.loadingPleaseWait',
      'flow.warning',
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

  /**
   * method: onNextCard
   * Calculates the index of the next card in the cards list
   */
  public onNextCard() {


    //this.replayCards = [];
    // Save the previous on in the Replay array
    
    //this.replayCards.push({card: this.cards[this.currentId], index: this.indexReplay});
    //this.indexReplay++;
    //this.replayCards.push({card: this.cards[this.currentId - 1], index: 1});

    //if the currentId is the last index in the cards list, the next card will be the first item of the list (cards[0])
    //else, it's the next one
    this.currentId = this.currentId >= this.cards.length - 1 || this.currentId < 0 ? 0 : this.currentId + 1;
    
    //if we are far enough in the list we trigger the Get cards method to get new cards
    if(this.currentId >= this.cards.length - 3){
      this.onGetCards(this.paging, this.userProfileId);
    }
    this.onAdaptCardsStack();
  }

  /**
   * method: onToPostsMain
   * On click on the button suggesting to create a post
   */
  public onToPostsMain() {
    this.router.navigate(['tabs/posts']);
  }

  /**
   * method: onGetCards
   * Retrieves all the cards to display in the Flow
   * @param paging `PagingVM`.
   * @param userProfileId `number`.
   * @returns A `Subscription<any>`.
   */
  public async onGetCards(paging: Paging, userProfileId: string){
    
    return await this._flowController.list(paging,userProfileId).toPromise().then(data => 
      {
        //If the cards list is empty when calling the method, we just initialize it.
        if(!this.cards || this.cards.length == 0)
        {
          this.cards = data;
          //Setting 'currentId' to zero, initialize animation stack and add view to the first card
          this.currentId = 0;
          //if there is at least one card in the list, we set up the activeCardId
          if(this.cards.length > 0)
          {
            //this.paging.lastId = this.cards[this.cards.length - 1].id;
            //this.paging.lastId = this.cards.indexOf(this.cards[this.cards.length - 1]);
            this.paging.lastId++;
            this.activeCardId = this.cards[this.currentId].id;
            var userViewCard: UserViewCard = {
              cardId: this.activeCardId,
              userId: this.userProfileId
            };
            this.onAddView(userViewCard);
          }
        }
        //If the cards list is NOT empty when calling the method.
        else 
        {
          //Testing data and add unknown cards in the flow
          data.forEach(element => 
          {
            this.cards.push(element);
          }); 
          //this.paging.lastId = this.cards[this.cards.length - 1].id;
          //this.paging.lastId = this.cards.indexOf(this.cards[this.cards.length - 1]);
          this.paging.lastId++;
          //Check for not anymore existing cards and destroy them
          this.onDestroyOldCards();
        }
      },
      error => 
      {
        //if error from the back end, initialize an empty list and show a warning Toast
        this.cards = [];
        this._toastCtrl.create({
          message: this.translations['flow.warning'],
          duration: 3000,
          position: "top"
        }).then(t => t.present());
      });
  }

  /**
   * method: onDiscard
   * Discard a card and display the next one in the list
   * that method is an Api service call method.
   * @param discard `DiscardVM`.
   * @returns A `Subscription<any>`.
   */
  public onDiscard(discard: Discard) {

    //this.replayCards = [];
    this.isReplayed = true;
    this.replayCards.push({card: this.cards[this.currentId], index: this.indexReplay});
    this.indexReplay++;

    var idCard = discard.cardId;
    var index = this.cards.findIndex(c => c.id == idCard);

    //if the discarded card is a Reporting we simply go to the next card
    if(this.cards[this.currentId].type == "Reporting")
      this.onNextCard();
    //if not, we remove the card from the list, adapt the currentId and display the next card
    else 
    {
      this.cards.splice(index, 1);
      this.currentId--;
      if(this.cards.length > 0){
        //this.paging.lastId = this.cards[this.cards.length - 1].id;
        //this.paging.lastId = this.cards.indexOf(this.cards[this.cards.length - 1]);
        this.paging.lastId++;
      }
      else
        //this.paging.lastId = 0;
        this.paging.lastId = 1;
      this.onNextCard();

      //creating answerVM to send to the backend
      var discard: Discard = {
        userProfileId: this.userProfileId,
        cardId: idCard
      };
      this._answerController.discard(
        discard
      ).subscribe();
    }
    
  }

  /**
   * method: onPreviousCard
   * Calculates the index of the previous card in the cards list
   */
  public onPreviousCard() {
    //this.replayCards = [];
    // Save the previous on in the Replay array
    //this.replayCards.push({card: this.cards[this.currentId], index: this.indexReplay});
    //this.indexReplay++;
    //this.replayCards.push({card: this.cards[this.currentId + 1], index: 1});


    //if the old currentId is the first index in the cards list, the next card will be the last item of the list
    //else, it's the previous one
    this.currentId = this.currentId == 0 ? this.cards.length - 1 : this.currentId - 1;
    //if we are far enough in the list we trigger the fetching of the next cards
    if(this.currentId >= this.cards.length - 3)
      this.onGetCards(this.paging, this.userProfileId);
    this.onAdaptCardsStack();
  }

  /**
   * method: onAddView
   * Add a view count on the card for this user
   * that method is an Api service call method.
   * @param userViewCard `UserViewCardVM`.
   * @returns A `Subscription<any>`.
   */
  public onAddView(userViewCard: UserViewCard) {
    //if the current card is not a reporting, we count the view
    if(!(this.cards[this.currentId].type == "Reporting") && !(this.cards[this.currentId].type == "Mood")
     && !(this.cards[this.currentId].type == "QuestionSurvey") && !(this.cards[this.currentId].type == "SurveyReport")
     && !(this.cards[this.currentId].type == "QuestionSurveyReport"))
    {
      //Creating the UserViewCardVM to send to the back end      
      this._flowController.addView(userViewCard).subscribe();
    }
  }

  /**
   * method: onSwipeCard
   * When a card in thrown out of the stack, decides the action to perform depending on the direction of the throw
   */
  public onSwipeCard(event: ThrowEvent) {
    if(this.cards.length == 1){
      this.swingStack.stack.getCard(event.target).throwIn(50,50);
    }
    else{
      switch(event.throwDirection)
      {
        //if swipe left then next card
        case Direction.LEFT: 
          //Next card if the list has more cards, reload the list if not
          if(this.cards.length > 1){
            this.onNextCard();
          }
          else
          {  
            this.onGetCards(this.paging,this.userProfileId).then(_ => this.onAdaptCardsStack());
          }
          break;
        //if swipe right then previous card
        case Direction.RIGHT:
          //Previous card if the list has more cards, reload the list if not
          if(this.cards.length > 1){
            this.onPreviousCard();
          }
          else
          {
            this.onGetCards(this.paging,this.userProfileId).then(_ => this.onAdaptCardsStack());
          }
          break;
        //if swipe up then discard
        case Direction.UP:
          var discard: Discard = {
            userProfileId: this.userProfileId,
            cardId: this.activeCardId
          };
          this.onDiscard(discard);
          break;
        default: console.log("invalid throwOut");
      }
    }
  }

  /**
   * method: onAdaptCardsStack
   * Adapt the display array to properly show current and next card
   */
  public onAdaptCardsStack() {
    //Calculate the index of the next card depending on the value of the current one
    var nextCardId = this.currentId == this.cards.length - 1 ? 0 : this.currentId + 1;
    var tmpCards = [];

    //if the list of cards is empty, we need to empty the display stack too
    if(this.cards.length == 0)
      this.displayCards = [];
    else {   
      //Setting up 'activeCardId' to trigger the card animation in child templates
      this.activeCardId = this.cards[this.currentId].id;

      //Adding view to the concerned card
      var userViewCard: UserViewCard = {
        cardId: this.activeCardId,
        userId: this.userProfileId
      };
      this.onAddView(userViewCard);

      //if the list has a length of 1, we need only one card in the display stack
      if(this.cards.length == 1)
      {
        //pushing current card in the animation stack
        tmpCards.push({card: this.cards[this.currentId], index: 1});
        this.displayCards = tmpCards;
      }
      //if the list is bigger than 1
      else
      {
        tmpCards.push({card: this.cards[nextCardId], index: -1});
        tmpCards.push({card: this.cards[this.currentId], index: 1});

        this.displayCards = tmpCards;
      }
    } 
  }

  /**
   * method: onDestroyOldCards
   * Check incoming data against existing cards list to destroy cards in the cards list that are not present in the incoming data
   */
  public async onDestroyOldCards() 
  {
    /*if(!(this.cards[this.currentId].type == "Mood"))
    {*/
    const paging: Paging = {
      step: 0,
      lastId: this.paging.lastId
    };

    //returns the ids of the cards we are supposed to have to check and destroy deleted cards
    return await this._flowController.checkDeletedCards(paging, this.userProfileId).subscribe(res => {
      for(var i = 0; i < this.cards.length; i++ ){
        //if we find an id in our cards list that is not in the res, we delete the card
        if(!res.find(_ => _ == this.cards[i].id)){
          //if the card to delete is the currently active one, we display the next one
          if(this.activeCardId == this.cards[i].id)
            this.onNextCard();
          this.cards.splice(i, 1);

          //if there still are cards in the list we ensure the lastId is still right
          if(this.cards.length > 0){
            //this.paging.lastId = this.cards[this.cards.length - 1].id;
            //this.paging.lastId = this.cards.indexOf(this.cards[this.cards.length - 1]);
            this.paging.lastId++;
          }
          else
            //this.paging.lastId = 0;
            this.paging.lastId = 1;
          
          //if i == 0 we stay at 0, else, we decrement before continuing
          i = i == 0 ? 0 : i - 1;
        }
      }
    });
  //}
  }

  /**
   * method: onCardAnswered
   * Triggered when a card is answered, destroy the card and go to the next one
   */
  public onCardListAnswered(cardId, id){
    if (this.isList){
      var index: number;
      let i: number;
      for(i = 0; i < this.cards.length; i++){
        if(this.cards[i].id == cardId){
          this.cards.splice(i, 1);
          return;
        }
      }
    }
  }
  public onCardAnswered(id: number) {
    if (!this.isList){
        //this.replayCards = [];
        if(this.cards[this.currentId].type != "QuestionSurvey"){
          this.isReplayed = true;
          this.replayCards.push({card: this.cards[this.currentId], index: this.indexReplay});
          this.indexReplay++;
        }

        //remove the card from the list of flow cards
        this.cards.splice(this.currentId, 1);

        this.currentId--;

        if(this.cards.length > 0){
          //this.paging.lastId = this.cards[this.cards.length - 1].id;
          //this.paging.lastId = this.cards.indexOf(this.cards[this.cards.length - 1]);
          this.paging.lastId++;
          //go to the next card
          this.onNextCard();
        }
        else
          //this.paging.lastId = 0;
          this.paging.lastId = 1;

        this.onAdaptCardsStack();
    }
    
  }

  public onStartSurvey(id: string) {
    this._surveyController.getSurveyquestions(id).subscribe((res : Survey[])=> {
      
      this.isReplayed = true;
      this.replayCards.push({card: this.cards[this.currentId], index: this.indexReplay});
      this.indexReplay++;

      this.cards.splice(this.currentId,1,...res);
      this.currentId--;
      
      if(this.cards.length > 0){
        //this.paging.lastId = this.cards[this.cards.length - 1].id;
        //this.paging.lastId = this.cards.indexOf(this.cards[this.cards.length - 1]);
        this.paging.lastId++;
        //go to the next card
        this.onNextCard();
      }
      else
        //this.paging.lastId = 0;
        this.paging.lastId = 1;

      this.onAdaptCardsStack();
    });
  }

  public onSeeReportSurvey(id: string) {
    this._surveyController.getSurveyReportsquestions(id).subscribe((res : Survey[])=> {
      this.cards.splice(this.currentId,1,...res);
      this.currentId--;
    
      if(this.cards.length > 0){
        //this.paging.lastId = this.cards[this.cards.length - 1].id;
        //this.paging.lastId = this.cards.indexOf(this.cards[this.cards.length - 1]);
        this.paging.lastId++;
        //go to the next card
        this.onNextCard();
      }
      else
        //this.paging.lastId = 0;
        this.paging.lastId = 1;

      
      this.onAdaptCardsStack();
    
    })
  }

  public onReplay(){
    this.showCards = true;
    this.isReplayed = false;
    this.swipeIndex = this.replayCards.length - 1 ; 
  }

  public onCloseReplay(){
    this.showCards = false;
    this.isReplayed = true;
  }

  public onChooseCard(card: any){
    this.showCards = false;
    this.isReplayed = true;
    //this.replayCards.splice(index, 1);
    this.replayCards.splice(this.replayCards.indexOf(card), 1)
    this.indexReplay--;
    this.cards = [];
    const paging: Paging = {
      step: 10,
      lastId: 1
    };
    this.onGetCards(paging, this.userProfileId).then(_ => this.onAdaptCardsStack());
  }

  /**
   * method: onSwipeCard
   * When a card in thrown out of the stack, decides the action to perform depending on the direction of the throw
   */
  public onSwipeCardReplayed(event: ThrowEvent) {
    if(this.replayCards.length == 1){
      this.swingStack2.stack.getCard(event.target).throwIn(50,50);
    }
    else{
      switch(event.throwDirection)
      {
        //if swipe left then next card
        case Direction.LEFT: 
          //Next card if the list has more cards, reload the list if not
            this.onNextCardReplayed(this.swipeIndex);
          break;
        default: console.log("invalid throwOut");
      }
    }
  }

  public onNextCardReplayed(index: number){
    
    this.replayCards.push(this.replayCards[index]);
    this.replayCards.splice(index,1);
    //this.replayCards.push({card: this.cards[this.currentId], index: this.indexReplay});
    //this.replayCards.splice(index, 1);
    //this.replayCards.push(this.replayCards[index]);

  }

  public onRefresh(refresher) {
    this.onGetCards(this.paging, this.userProfileId).then(_ => refresher.complete());
  }
}
