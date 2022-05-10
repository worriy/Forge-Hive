import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingController, ModalController, NavParams } from '@ionic/angular';
import { Chart } from 'chart.js';
import { AnswerControllerService } from 'src/app/services/api/answer-controller.service';
import { FlowControllerService } from 'src/app/services/api/flow-controller.service';
import { HighlightsControllerService } from 'src/app/services/api/highlights-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { Answer } from 'src/app/shared/interfaces/posts/answer';
import { UserViewCard } from 'src/app/shared/interfaces/user/user-view-card';

@Component({
  selector: 'highlights-top-posts-card-modal',
  templateUrl: './top-posts-card-modal.component.html',
  styleUrls: ['./top-posts-card-modal.component.scss'],
})
export class TopPostsCardModalComponent implements OnInit {
  @ViewChild('doughnutCanvas') doughnutCanvas;
  doughnutChart: any;

  private values: {result: any, color: string}[];
  results: Array<number>;
  colors: Array<string>;
  bigValue: number;
  colorValue: any;

  @Input() card: any;
  public date = new Date();
  public endIdeaCard;
  public alreadyAnswered = null;
  private userProfileId: any;

  @Output()
  answered: EventEmitter<String> = new EventEmitter<String>();

  constructor(
    public _answerController: AnswerControllerService,
    public _highlightsController: HighlightsControllerService,
    public router: Router,
    public _flowController: FlowControllerService,
    public modalController: ModalController,
    public _loadingCtrl: LoadingController,
    public _userController: UserControllerService
  ) {
  }

  ngOnInit() {
    this.onCardAnswered();
    this.results = new Array<number>();
    this.colors = new Array<string>();
    this.values = [];

    this.userProfileId = this._userController.getUserProfileId();
    this.alreadyAnswered = null;
    this.endIdeaCard = new Date(this.card.endDate);
    this.endIdeaCard.setDate(this.endIdeaCard.getDate() - 1);
  }


  /**
  * Save an answer for agreeing to the idea in the db
  * @param answer `AnswerVM`.
  * @returns A `Subscription<any>`.
  */
 public onAgree(idChoice: string) {

  //prepare the answerVM to send to the back end
  var answer: Answer = {
    idUser: this.userProfileId,
    idCard: this.card.linkedCardId,
    idChoice
  };

  return this._answerController.create(answer)
  .subscribe(data => {
    //emit the answered event
    this.answered.emit();
    this.modalController.dismiss({action: "close"});
  });
  }

  /**
   * Save an answer for disagreeing to the idea in the db
   * @param answer `AnswerVM`.
   * @returns A `Subscription<any>`.
   */
  public onDisagree(idChoice: string) {
    //prepare the answerVM to send to the back end
    var answer: Answer = {
      idUser: this.userProfileId,
      idCard: this.card.linkedCardId,
      idChoice
    };

    return this._answerController.create(
      answer
    ).subscribe(data => {
      //emit the answered event
      this.answered.emit();
      this.modalController.dismiss({action: "close"});
    });
  }

  public onAnswer(idChoice: string) {
    //prepare the answerVM to send to the back end
    var answer: Answer = {
      idUser: this.userProfileId,
      idCard: this.card.linkedCardId,
      idChoice
    };
    //emit the answered event for the flow to reload the flow cards list.
    this.answered.emit();
    return this._answerController.create(answer).subscribe(data => {
      //emit the answered event
      this.answered.emit();
      this.modalController.dismiss({action: "close"});
    });
  }

  public onApplause(idChoice: string) {

    //prepare the answerVM to send to the back end
    var answer: Answer = {
      idUser: this.userProfileId,
      idCard: this.card.linkedCardId,
      idChoice
    };
    return this._answerController.create(
      answer
    ).subscribe(data => {
      //emit the answered event
      this.answered.emit();
      this.modalController.dismiss({action: "close"});
    });
  }

  /**
   * Check if card is already answered
   */
  public async onCardAnswered() {
    let loading = await this._loadingCtrl.create({
      spinner: 'crescent',
      showBackdrop: false,
      cssClass: 'transparent',
      backdropDismiss: true
    });
    loading.present();
    
    this.onCheckForAnswer(this.card.linkedCardId, this.userProfileId).subscribe(res => {
      this.alreadyAnswered = res;
      loading.dismiss();
      if(this.alreadyAnswered){
        this.onInitialiseChart();
      }
      if (!this.alreadyAnswered && this.endIdeaCard < this.date && this.card.answers > 0){
        this.onInitialiseChart();
      }
      if(!this.alreadyAnswered){
        const userViewCard: UserViewCard = {
          cardId: this.card.linkedCardId,
          userId: this.userProfileId
        };
        this._flowController.addView(userViewCard).subscribe();
      }
    });
  }

  /**
   * Calls api to know if card is already answered
   * @param cardId `number`.
   * @param userProfileId `number`.
   * @returns A `Subscription<any>`.
   */
  public onCheckForAnswer(cardId: string, userProfileId: string) {
    return this._answerController.answeredCard(cardId,userProfileId);
  }

  /**
  * dismiss the Modal
  */
  onCloseModal() {
    this.modalController.dismiss({action: "close"});
  }

  public onRandomColor(numbers: number): Array<string>{
    var colors: Array<string> = new Array<string>();
    for(var i=0; i< numbers; i++){
      colors.push('#' + Math.random().toString(16).slice(2, 8).toUpperCase())
    }
    return colors;
  }

  onInitialiseChart(){
    this.bigValue = 0

    //if Event Card 
    if(this.card.results.length == 1){
      this.colors.push("#715FFF", "#E0E0E0");
      this.results.push(this.card.results[0].value);
      this.results.push(100 - this.card.results[0].value);

      this.bigValue = this.card.results[0].value;
      this.colorValue = this.colors[0];
      
      this.values.push({result: this.card.results[0], color: this.colors[0]});
    }else{
      // if it is a report idea take two colors 
      if (this.card.results.length == 2 ){
        this.colors.push("#715FFF", "#E0E0E0");
      }else{
          this.colors.push("#715FFF", "#E0E0E0");
          let randomColors = this.onRandomColor(this.card.results.length-2);
          randomColors.forEach(color => {
            this.colors.push(color);
          });
      }
      // Recuperate the Results of the card
      for(var i=0; i<this.card.results.length; i++){
        this.results.push(this.card.results[i].value);

        if(this.card.results[i].value > this.bigValue){
          this.bigValue = this.card.results[i].value;
          this.colorValue = this.colors[i];
        }

        this.values.push({result: this.card.results[i], color: this.colors[i]});
      }
    }
    this.onDisplayChart();
  }

  onDisplayChart() {
    this.doughnutChart = new Chart(this.doughnutCanvas.nativeElement, {
      type: 'doughnut',
      data: {
        datasets: [{
          data: this.results,
          backgroundColor: this.colors,
        }]
      },
      options: {
        elements: {
          center: {
          text: this.bigValue.toString() + "%",
          color: this.colorValue, //Default black
          fontStyle: 'Arial', //Default Arial
          sidePadding: 60 //Default 20 (as a percentage)
        }
      },
        cutoutPercentage: 80,
        responsive: true,
        //maintainAspectRatio: false,
        legend: {
          display: false,
        },
        tooltips: {
          enabled: false
        },
      },
 
    });
    

    Chart.pluginService.register({
      beforeDraw: function (chart) {
        if (chart.config.options.elements.center) {
          //Get ctx from string
          var ctx = chart.chart.ctx;
    
          //Get options from the center object in options
          var centerConfig = chart.config.options.elements.center;
          var fontStyle = centerConfig.fontStyle || 'Arial';
          var txt = centerConfig.text;
          var color = centerConfig.color || '#000';
          var sidePadding = centerConfig.sidePadding || 20;
          var sidePaddingCalculated = (sidePadding/100) * (chart.innerRadius * 2)
          //Start with a base font of 30px
          ctx.font = "30px " + fontStyle;
    
          //Get the width of the string and also the width of the element minus 10 to give it 5px side padding
          var stringWidth = ctx.measureText(txt).width;
          var elementWidth = (chart.innerRadius * 2) - sidePaddingCalculated;
    
          // Find out how much the font can grow in width.
          var widthRatio = elementWidth / stringWidth;
          var newFontSize = Math.floor(30 * widthRatio);
          var elementHeight = (chart.innerRadius * 2);
    
          // Pick a new font size so it will not be larger than the height of label.
          var fontSizeToUse = Math.min(newFontSize, elementHeight);
    
          //Set font settings to draw it correctly.
          ctx.textAlign = 'center';
          ctx.textBaseline = 'middle';
          var centerX = ((chart.chartArea.left + chart.chartArea.right) / 2);
          var centerY = ((chart.chartArea.top + chart.chartArea.bottom) / 2);
          ctx.font = fontSizeToUse+"px " + fontStyle;
          ctx.fontWeight = 'normal'
          ctx.fillStyle = color;
    
          //Draw text in center
          ctx.fillText(txt, centerX, centerY);
        }
      }
    });
  }
}
