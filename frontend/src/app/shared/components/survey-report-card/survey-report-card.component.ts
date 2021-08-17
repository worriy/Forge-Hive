import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { SurveyControllerService } from 'src/app/services/api/survey-controller.service';
import { CardVM } from '../../interfaces/posts/cardVM';
import { Chart } from 'chart.js';

@Component({
  selector: 'app-survey-report-card',
  templateUrl: './survey-report-card.component.html',
  styleUrls: ['./survey-report-card.component.scss'],
})
export class SurveyReportCardComponent implements OnInit {

  //card to display, provided by the parent template
  @Input('card') card: CardVM;
  public isList = false;
  //Get the activeCatdId from the parent template
  @Input('activeCardId')
    set activeCardId(id: string)
    {
      if (!this.isList){
        this.onAdaptClass(id);
      }
    }

  @ViewChild('doughnutCanvas') doughnutCanvas;
  doughnutChart: any;

  //event emitted when the user click on Start Survey
  @Output()
  reports: EventEmitter<String> = new EventEmitter<String>();
  
  //CSS class to display on the card
  active: string;

  private values: {result: any, color: string}[];
  results: Array<number>;
  colors: Array<string>;
  bigValue: number;
  colorValue: any;

  constructor(
    public _surveyController: SurveyControllerService
  ) {
  }

  ngOnInit() {
    this.results = new Array<number>();
    this.colors = new Array<string>();
    this.values = [];
  }

  ngAfterViewInit() {
    if(this.card.answers > 0 ){
      this.onInitialiseChart();
      this.onDisplayChart();
    }
  }

  public onSeeReportSurvey(idSurvey : string) {
    //let idsurvey = this.card.id;
    this._surveyController.getSurveyReportsquestions(idSurvey).subscribe(res => {
      //emit the started event for the flow to reload the flow cards list.
      this.reports.emit();
    })
    
  }

  /**
   * method: onAdaptClass
   * That method is a blank method.
   */
  public onAdaptClass(id: string) {
    //when the active card changes, check if it's this one, if yes, adapt the css class to launch the animation
    if(this.card.id == id)
      this.active = "is-card-active";
    else 
      this.active = "";
  }

  public onRandomColor(numbers: number): Array<string>{
    var colors: Array<string> = new Array<string>();
    for(var i=0; i< numbers; i++){
      colors.push('#' + Math.random().toString(16).slice(2, 8).toUpperCase())
    }
    return colors;
  }

  public onInitialiseChart(){
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
  }

  public onDisplayChart() {
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
