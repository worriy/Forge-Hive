import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-choose-new-post-category',
  templateUrl: './choose-new-post-category.component.html',
  styleUrls: ['./choose-new-post-category.component.scss'],
})
export class ChooseNewPostCategoryComponent implements OnInit {

  constructor(
    public router: Router,
    private activatedRoute: ActivatedRoute
  ) {
  }

  ngOnInit() {
  }

  /**
   * Navigate to the correct post creation process depending on the target type
   */
  public async onShowCreateModal(type: string) {
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
   * Navigates back
   */
  public onCancel() {
    this.router.navigate(['..'], { relativeTo: this.activatedRoute });
  }
}
