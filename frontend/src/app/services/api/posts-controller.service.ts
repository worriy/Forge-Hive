import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { DataService } from './data.service';
import { HttpClient } from '@angular/common/http';
import { TopPost } from 'src/app/shared/interfaces/highlights/top-post';
import { Post } from 'src/app/shared/interfaces/posts/post';
import { Paging } from 'src/app/shared/interfaces/posts/paging';
import { PostDetails } from 'src/app/shared/interfaces/posts/postDetails';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class PostsControllerService {
  posts: Array<Post>;
  topPosts: Array<TopPost>;
  private _endpoint: string = null;

  public _getLatestPostsApi = '/api/posts/getLatestPosts';
  public _getPostDetailsApi = '/api/posts/getPostDetails';
  public _getTopPostsApi = '/api/posts/getTopPosts';
  public _deleteApi = '/api/posts/delete';
  constructor(
    public _dataService: DataService)
  {
  }

  /**
   * method: getLatestPosts.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param pagingParam `PagingVM`.
   * @param userProfileIdParam `number`.
   * @returns `PostVM>`.
   */
  public getLatestPosts(
    pagingParam: any,
    userProfileIdParam: string
  ): Observable<Array<Post>> {
    var result = this._dataService.get(
      `${this._getLatestPostsApi}`,
      {
        step: pagingParam.step.toString(),
        page: pagingParam.page.toString(),
        userProfileId: userProfileIdParam.toString()
      }); 

      return result;
  }

  /**
   * method: getPostDetails.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param postIdParam `number`.
   * @returns `PostDetailsVM>`.
   */
  public getPostDetails(postIdParam: string): Observable<PostDetails> {
    return this._dataService.get(
      `${this._getPostDetailsApi}`,
      {
        postId: postIdParam
      });
  }

  /**
   * method: getTopPosts.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @returns `TopPostVM>`.
   */
  public getTopPosts (userProfileIdParam: string
  ): Observable<Array<TopPost>> {
    return this._dataService.get(
      `${this._getTopPostsApi}`,
      {
        userProfileId: userProfileIdParam
      });
  }

  /**
   * method: delete.
   * You should add a description of your method here.
   * That method should be used to connect with generated
   * backend API. You should add business logic inside.
   * @param ideaIdParam `number`.
   */
  public delete(postIdParam: string) {
    return this._dataService.delete(
      `${this._deleteApi}`,
      {
          postId: postIdParam
      }
    ).subscribe(data  =>{           
      },
      error=>
      {
        window.alert(error.error.Message);
      }
    );
  }
}
