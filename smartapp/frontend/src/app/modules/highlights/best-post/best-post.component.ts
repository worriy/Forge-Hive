import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { HighlightsControllerService } from 'src/app/services/api/highlights-controller.service';
import { Report } from 'src/app/shared/interfaces/posts/report';
import { Chart } from 'chart.js';
import { Subject } from 'rxjs';

@Component({
  selector: 'highlights-best-post',
  templateUrl: './best-post.component.html',
  styleUrls: ['./best-post.component.scss'],
})
export class BestPostComponent implements OnInit {

  @Input() refresher: Subject<void>;

  @ViewChild('doughnutCanvas') doughnutCanvas;
  doughnutChart: any;
  private values: {result: any, color: string}[];
  results: Array<number>;
  colors: Array<string>;
  bigValue: number;
  colorValue: any;

  public bestPost: Report;

  constructor(
    public _highlightsController: HighlightsControllerService,
    public router: Router
  ) {
    this.results = new Array<number>();
    this.colors = new Array<string>();
    this.values = [];
  }

  ngOnInit(){
    this.onGetBestPost();
    this.refresher.subscribe(() => this.onGetBestPost());
  }

  ngAfterViewInit() {
    this.onInitialiseChart();
      
  }

  /**
   * Navigates to posts tab
   */
  public onToPosts() {
    this.router.navigate(['tabs/posts/newPost']);
  }

  /**
   * Retrieves the best post
   */
  public async onGetBestPost() {
    return this._highlightsController.getBestPost(
    ).toPromise().then(data => this.bestPost = data);
  }

  public onRandomColor(numbers: number): Array<string>{
    var colors: Array<string> = new Array<string>();
    for(var i=0; i< numbers; i++){
      colors.push('#' + Math.random().toString(16).slice(2, 8).toUpperCase())
    }
    return colors;
  }

  public onInitialiseChart(){

    var card: Report;
    this.bigValue = 0;

    this._highlightsController.getBestPost().subscribe(data => {
      card = data;
      if(card != null || card != undefined){
        //if Event Card 
        if(card.results.length == 1){
          this.colors.push("#715FFF", "#E0E0E0");
          this.results.push(card.results[0].value);
          this.results.push(100 - card.results[0].value);

          this.bigValue = card.results[0].value;
          this.colorValue = this.colors[0];
          
          this.values.push({result: card.results[0], color: this.colors[0]});
        }else{
          // if it is a report idea take two colors 
          if (card.results.length == 2 ){
            this.colors.push("#715FFF", "#E0E0E0");
          }else{
              this.colors.push("#715FFF", "#E0E0E0");
              let randomColors = this.onRandomColor(card.results.length-2);
              randomColors.forEach(color => {
                this.colors.push(color);
              });
          }
          // Recuperate the Results of the card
          for(var i=0; i<card.results.length; i++){
            this.results.push(card.results[i].value);
      
            if(card.results[i].value > this.bigValue){
              this.bigValue = card.results[i].value;
              this.colorValue = this.colors[i];
            }

              this.values.push({result: card.results[i], color: this.colors[i]});
              
          }
        }
        this.onDisplayChart();
      }
    });
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
