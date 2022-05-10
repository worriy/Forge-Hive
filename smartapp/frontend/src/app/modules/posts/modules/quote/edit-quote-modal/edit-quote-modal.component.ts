import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertController, LoadingController, ModalController, ToastController } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { GroupControllerService } from 'src/app/services/api/group-controller.service';
import { UserControllerService } from 'src/app/services/api/user-controller.service';
import { PostsControllerService } from 'src/app/services/api/posts-controller.service';
import { TargetGroup } from 'src/app/shared/interfaces/groups/target-group';
import { EditPost } from 'src/app/shared/interfaces/posts/edit-post';
import { EditablePost } from 'src/app/shared/interfaces/posts/editable-post';
import { Post } from 'src/app/shared/interfaces/posts/post';

@Component({
  selector: 'app-edit-quote-modal',
  templateUrl: './edit-quote-modal.component.html',
  styleUrls: ['./edit-quote-modal.component.scss'],
})
export class EditQuoteModalComponent implements OnInit {

  private destroy = new Subject<void>();

  @Input() post: Post;
  editablePost: EditablePost;

  //the current user's id
  userProfileId: any;

  //the current user's groups
  myGroups: TargetGroup[];
  publicGroupId: string;

  private translations: string[] = [];

  public quoteForm: FormGroup;

  constructor(
    public _postsController: PostsControllerService,
    public _groupController: GroupControllerService,
    public modalCtrl: ModalController,
    public _toastCtrl: ToastController,
    public _loadingCtrl: LoadingController,
    public _alertCtrl: AlertController,
    public _translateService: TranslateService,
    private formBuilder: FormBuilder,
    public _userController: UserControllerService
  ) {
    this.userProfileId = this._userController.getUserProfileId();
  }

  public async ngOnInit() {
    this.getTranslations();
    this.initForm();
  }

  ngOnDestroy() {
    this.destroy.next();
    this.destroy.complete();
  }

  /**
   * Fetch needed informations and initialize form
   */
  private async initForm() {
    //launch a loading screen while loading the posts details
    let loading = await this._loadingCtrl.create({
      spinner: 'crescent',
      message: this.translations['commons.loadingPleaseWait'],
      showBackdrop: false
    });
    loading.present();

    this.getTargetableGroups().then(() => 
    {
      //get all the informations about the post
      this._postsController.getEditablePost(this.post.id).subscribe(data => 
      {
        this.editablePost = data;

        this.quoteForm = this.formBuilder.group({
          content: [this.post.content, Validators.compose([Validators.required, Validators.maxLength(140)])],
          publicationDate: [this.post.publicationDate, Validators.required],
          endDate: [this.post.endDate, Validators.required],
          targetGroups: [this.editablePost.targetGroupsIds, Validators.required]
        });

        //closing the loader when retrieving is done.
        loading.dismiss();
      },
      error => {
        loading.dismiss();
      });
    });
  }

  private async getTranslations() {
    this._translateService.stream([
      'commons.no',
      'commons.yes',
      'commons.saving',
      'commons.loadingPleaseWait',
      'posts.publishPost',
      'posts.publishPostMessage',
      'posts.cantPubBefTod',
      'posts.cantExpBefPub',
      'posts.warnOneDayAvailable',
      'idea.takePicture',
      'idea.fromGallery',
      'idea.compressing'
    ]).pipe(takeUntil(this.destroy))
    .subscribe((translations: string[]) => this.translations = translations);
  }

  /**
   * Retrieves the user's groups to choose post target(s)
   */
  public async getTargetableGroups() {
    return this._groupController.listTargetableGroups(this.userProfileId).toPromise().then(data => 
      {
        this.myGroups = data;
        
        //Search for the public group to retrieve its id
        this.myGroups.forEach(element => 
        {
          //if its the public group, memorize its id
          if(element.name.toLowerCase() == "public")
            this.publicGroupId = element.id;
        });
      });
  }

  /**
   * Check the edited Quote's consistency
   */
  public async checkInformation(){
    const publicationDate = new Date(this.quoteForm.controls.publicationDate.value);
    const endDate = new Date(this.quoteForm.controls.endDate.value);

    // Check that all the modifications are correct
    if(!this.checkDates(publicationDate, endDate))
      return;

    //check if the date changed
    if(!(this.editablePost.publicationDate == publicationDate))
      this.editablePost.publicationDate = publicationDate;
    if(!(this.editablePost.endDate == endDate))
      this.editablePost.endDate = endDate;
      
    this.saveChanges();    
  }
  
  /**
   * Save edited Idea
   */
  public async saveChanges() {
    if (this.quoteForm.invalid)
      return;
    // Save the changes if the changes are correct
    let loading = await this._loadingCtrl.create({
      spinner: 'crescent',
      message: this.translations['commons.saving'],
      showBackdrop: false
    });

    loading.present();
    const editVM: EditPost = {
      id: this.editablePost.id,
      content: this.quoteForm.value.content,
      publicationDate: this.quoteForm.value.publicationDate,
      endDate: this.quoteForm.value.endDate,
      targetGroupsIds: [this.publicGroupId]
    };
    this._postsController.update(editVM).subscribe(() => {
      loading.dismiss();
      this.modalCtrl.dismiss({
        action: 'update',
        post: editVM
      });
    });
    
  }

  /**
   * Check publication and end dates logic, show a warning toast if the post will only be available for one day
   */
  public checkDates(publication: Date, end: Date): boolean {
    var now = new Date();
    now.setHours(0, 0, 0, 0);

    //if publication or end date is before today, return false and show a toast
    if(publication.getTime() < now.getTime() || end.getTime() < now.getTime())
    {
      this.showToast(this.translations['posts.cantPubBefTod']);
      return false;
    }
    //if end date is before publication date, return false and show a toast
    if(end.getTime() < publication.getTime())
    {
      this.showToast(this.translations['posts.cantExpBefPub']);
      return false;
    }
    //if the end date is the same than the publication date, return true but show a warning toast
    if(end.getTime() == publication.getTime())
    {
      this.showToast(this.translations['posts.warnOneDayAvailable']);
      return true;
    }
    return true;
  }

  /**
   * Check that at least a group is selected, select the public one if not
   * If several groups are selected, including the public one, select only the public one
   */
  public checkSelectedGroups(){
    const selectedGroups = this.quoteForm.controls.targetGroups.value;
    if(selectedGroups.length == 0 || (selectedGroups.length > 1 && selectedGroups.find(groupId => groupId === this.publicGroupId)))
    {
      this.quoteForm.controls.targetGroups.setValue([this.publicGroupId], { emitEvent: false });
      return;
    }
  }

  /**
   * Show a toast with custom message
   * @param message 
   */
  private async showToast(message: string) {
    const toast = await this._toastCtrl.create({
      message: message,
      duration: 2000,
      position: 'top'
    });

    toast.present();
  }

  /**
   * Closes this modal
   */
  public onCloseModal() {
    this.modalCtrl.dismiss();
  }
}
