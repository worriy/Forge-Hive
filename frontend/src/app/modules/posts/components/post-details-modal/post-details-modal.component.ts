import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ActionSheetController, AlertController, LoadingController, ModalController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { PostsControllerService } from 'src/app/services/api/posts-controller.service';
import { SurveyControllerService } from 'src/app/services/api/survey-controller.service';
import { PostDetails } from 'src/app/shared/interfaces/posts/postDetails';
import { Survey } from 'src/app/shared/interfaces/posts/survey/survey';
import { EditEventModalComponent } from '../../modules/event/edit-event-modal/edit-event-modal.component';
import { EditIdeaModalComponent } from '../../modules/idea/edit-idea-modal/edit-idea-modal.component';
import { EditQuestionModalComponent } from '../../modules/question/edit-question-modal/edit-question-modal.component';
import { EditQuoteModalComponent } from '../../modules/quote/edit-quote-modal/edit-quote-modal.component';
import { EditSuggestionModalComponent } from '../../modules/suggestion/edit-suggestion-modal/edit-suggestion-modal.component';
import { EditSurveyModalComponent } from '../../modules/survey/edit-survey-modal/edit-survey-modal.component';
import { Chart } from 'chart.js';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-post-details-modal',
  templateUrl: './post-details-modal.component.html',
  styleUrls: ['./post-details-modal.component.scss'],
})
export class PostDetailsModalComponent implements OnInit, OnDestroy {
  private destroy = new Subject<void>();

  @ViewChild('doughnutCanvas') doughnutCanvas;
  doughnutChart: any;
  private values: {result: any, color: string}[];
  results: Array<number>;
  colors: Array<string>;
  bigValue: number;
  colorValue: any;

  post: PostDetails;
  status: string;
  statusForDesign: string;
  init: boolean = false;
  questionsSurvey : Array<Survey>;

  @Input() postId: string;

  private translations: string[] = [];

  constructor(
    public _postController: PostsControllerService,
    public router: Router,
    private actionSheetCtrl: ActionSheetController,
    public _modalCtrl: ModalController,
    public alertCtrl: AlertController,
    public loadingCtrl: LoadingController,
    public _translateService: TranslateService,
    public _surveyController: SurveyControllerService
  ) {
    this.results = new Array<number>();
    this.colors = new Array<string>();
    this.values = [];
  }

  ngOnInit() {
    this.getTranslations();
  }

  ngAfterViewInit() {
    this.onGetPostDetails(this.postId);
    this.init = true;
  }

  ngOnDestroy() {
     this.destroy.next();
     this.destroy.complete();
  }

  getTranslations() {
    this._translateService.stream([
      'commons.no',
      'commons.yes',
      'commons.loadingPleaseWait',
      'commons.delete',
      'commons.cancel',
      'commons.edit',
      'posts.unpublished',
      'posts.deleteAlertTitle',
      'posts.deleteAlertMessage',
      'posts.closed',
      'posts.published'
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

  /**
   * method: onShowEditPostModal
   * That method is a blank method.
   */
  public async onShowEditPostModal(type: string) {
    // TO DO
    let editPostModal: HTMLIonModalElement;
    
    switch(type)
    {
      case "Idea":
        //Create the idea creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditIdeaModalComponent, 
          componentProps: {post: this.post}
        });
        break;
      case "Question":
        //Create the question creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditQuestionModalComponent,
          componentProps: {post: this.post}
        });
        break;
      case "Survey":
        //Create the question creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditSurveyModalComponent, 
          componentProps: {post: this.post}
        });
        break;
      case "Event":
        //Create the event creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditEventModalComponent, 
          componentProps: {post: this.post}
        });
        break;
      case "Quote":
        //Create the event creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditQuoteModalComponent, 
          componentProps: {post: this.post}
        });
        break;
      case "Suggestion":
        //Create the event creation process modal
        editPostModal = await this._modalCtrl.create({
          component: EditSuggestionModalComponent, 
          componentProps: {post: this.post}
        });
        break;
      default: console.log("nothing to do");
        return;
    }

    editPostModal.present();
    editPostModal.onDidDismiss().then(() => {
      //this.onGetPostDetails(this.post.id);
      this._postController.getPostDetails(
        this.post.id
      ).subscribe(data => {
        this.post = data;
        if(this.post.type == "Survey"){
          this._surveyController.getSurveyquestions(this.post.id).subscribe(res => {
            
            this.questionsSurvey = res;
          })
        }
        this.status = this.onCheckPostStatus(this.post.publicationDate, this.post.endDate);
        
      });
    } );
  }

  /**
   * method: onGetPostDetails
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param postId `number`.
   * @returns A `Subscription<any>`.
   */
  public async onGetPostDetails(postId: string) {
    //launch loader
    let loading = await this.loadingCtrl.create({
      message: this.translations['commons.loadingPleaseWait'],
      spinner: 'crescent',
      showBackdrop: false,
      backdropDismiss: true
    });
    loading.present();

    //retrieve the post's details, stop the loader when it's done
    return this._postController.getPostDetails(postId)
    .subscribe(data => {
      this.post = data;
      //if the Post is a Survey recuperate his Questions
      if(this.post.type == "Survey"){
        this._surveyController.getSurveyquestions(postId).subscribe(res => {
          
          this.questionsSurvey = res;
        })
      }
      this.status = this.onCheckPostStatus(this.post.publicationDate, this.post.endDate);
      this.onInitialiseChart();
      if(this.post.answers > 0 ){
        
        this.onDisplayChart();
      }
      loading.dismiss();
    });
  }

  /**
   * method: onDelete
   * You should add a description of your method here.
   * that method is an Api service call method.
   * @param ideaId `number`.
   * @returns A `Subscription<any>`.
   */
  public onDelete(ideaId: string) {
     let deleted = this._postController.delete(ideaId)
      if(deleted){
        this._modalCtrl.dismiss({action: "delete", post: this.post});
      }
  }

  /**
   * method: onPresentOptionsMenu
   * That method is a blank method.
   */
  public async onPresentOptionsMenu() {
    // TO DO
    //create the default options (delete and cancel)
    let buttons = [
      {
        text: this.translations['commons.delete'],
        role: 'destructive',
        handler: () => {
          this.onConfirmDelete();
        }
      },{
        text: this.translations['commons.cancel'],
        role: 'cancel',
        handler: () => {
        }
      }
    ];    
    //if the post is "unpublished", add the edit option
    // FIXME: Checking a translated data feels weird
    if(this.status.toLowerCase() == this.translations['posts.unpublished'].toLowerCase()){
      
      const buttonEdit = {
        text: this.translations['commons.edit'],
        role: 'edit',
        handler: () => {
          this.onShowEditPostModal(this.post.type);
        }
      }
      buttons.push(buttonEdit);
    }

    let actionSheet = await this.actionSheetCtrl.create({
      header: 'Options',
      buttons: buttons
    });
    actionSheet.present();
  }

  /**
   * method: onConfirmDelete
   * That method is a blank method.
   */
  public async onConfirmDelete() {
    let alert = await this.alertCtrl.create({
      header: this.translations['posts.deleteAlertTitle'],
      message: this.translations['posts.deleteAlertMessage'],
      buttons: [
        {
          text: this.translations['commons.no'],
          handler: () => {
          }
        },
        {
          text: this.translations['commons.yes'],
          handler: () => {
            this.onDelete(this.post.id);
          }
        }
      ]
    });

    alert.present();
  }

  /**
   * method: onCloseModal
   * That method is a blank method.
   */
  public onCloseModal() {
    // TO DO
    //this._nav.pop();
    if(this.status == "Closed"){
      this._modalCtrl.dismiss({action: "nothing", post: this.post});
    }else{
      this._modalCtrl.dismiss({action: "update", post: this.post});
    }
      
    
    
  }


  /**
   * method: onCheckPostStatus
   * fix the post status depending on its publication and end dates
   */
  public onCheckPostStatus(publication: Date, end: Date) {
    var endDate = new Date(end);
    var pubDate = new Date(publication);
    var now = new Date();
    
    var status
    //if the publication date if after now, the post is unpublished
    if(pubDate.getTime() > now.getTime() ){
      this.statusForDesign = "Unpublished";
      this._translateService.get("posts.unpublished").subscribe(res => status = res);
      return status;
    }
    //if the end date is before now, the post is closed
    if(endDate.getTime() < now.getTime()){
      this.statusForDesign = "Closed";
      this._translateService.get("posts.closed").subscribe(res => status = res);
      return status;
    }
    //if between those, the post is published
    else{
      this.statusForDesign = "Published";
      this._translateService.get("posts.published").subscribe(res => status = res);
      return status;
    }
  }

  public onGetType(post: PostDetails){
    let type = this._translateService.instant("posts."+post.type.toLowerCase());
    return type;
  }

  public onGetResultName(resultName: string){
    let name = this._translateService.instant("posts."+resultName.toLowerCase());
    return name;
  }

  public onRandomColor(numbers: number): Array<string>{
    var colors: Array<string> = new Array<string>();
    for(var i=0; i< numbers; i++){
      colors.push('#' + Math.random().toString(16).slice(2, 8).toUpperCase())
    }
    return colors;
  }

  public onInitialiseChart(){
    this.bigValue = 0;

    //if Event Card 
    if(this.post.results.length == 1){
      this.colors.push("#715FFF", "#E0E0E0");
      this.results.push(this.post.results[0].value);
      this.results.push(100 - this.post.results[0].value);

      this.bigValue = this.post.results[0].value;
      this.colorValue = this.colors[0];
      
      this.values.push({result: this.post.results[0], color: this.colors[0]});
    }else{
      // if it is a report idea take two colors 
      if (this.post.results.length == 2 ){
        this.colors.push("#715FFF", "#E0E0E0");
      }else{
          this.colors.push("#715FFF", "#E0E0E0");
          let randomColors = this.onRandomColor(this.post.results.length-2);
          randomColors.forEach(color => {
            this.colors.push(color);
          });
      }

      // Recuperate the Results of the card
      for(var i=0; i<this.post.results.length; i++){
        this.results.push(this.post.results[i].value);

        if(this.post.results[i].value > this.bigValue){
          this.bigValue = this.post.results[i].value;
          this.colorValue = this.colors[i];
        }
        this.values.push({result: this.post.results[i], color: this.colors[i]});
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
