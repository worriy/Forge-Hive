import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { DataService } from './data.service';
import { TopPost } from 'src/app/shared/interfaces/highlights/top-post';
import { Post } from 'src/app/shared/interfaces/posts/post';
import { PostDetails } from 'src/app/shared/interfaces/posts/postDetails';
import { CardTypes } from 'src/app/shared/interfaces/posts/card-types.enum';
import { EditablePost } from 'src/app/shared/interfaces/posts/editable-post';
import { PictureVM } from 'src/app/shared/interfaces/posts/pictureVM';
import { CreatePost } from 'src/app/shared/interfaces/posts/create-post';
import { EditPost } from 'src/app/shared/interfaces/posts/edit-post';


@Injectable({
  providedIn: 'root'
})
export class PostsControllerService {
  private _createPostApi = '/api/posts';
  private _updatePostApi = '/api/posts';
  private _getDefaultPictureApi = '/api/posts/getDefaultPicture';
  private _getEditablePostApi = '/api/posts/getEditableCard';
  public _getLatestPostsApi = '/api/posts/getLatestPosts';
  public _getPostDetailsApi = '/api/posts/getPostDetails';
  public _getTopPostsApi = '/api/posts/getTopPosts';
  public _deleteApi = '/api/posts';
  constructor(
    public _dataService: DataService)
  {
  }

  public create(createPostVM: CreatePost): Observable<any> {
    return this._dataService.post(
      this._createPostApi,
      createPostVM,
      {}
    );
  }

  public update(updatePostVm: EditPost): Observable<any> {
    return this._dataService.put(
      this._updatePostApi,
      updatePostVm,
      {}
    );
  }

  public getDefaultPicture(type: CardTypes): Observable<PictureVM> {
    return this._dataService.get(
      `${this._getDefaultPictureApi}`,
      {
        type: type.toString()
      }
    );
  }

  public getEditablePost(postId: string): Observable<EditablePost> {
    return this._dataService.get(
      `${this._getEditablePostApi}/${postId}`,
      {}
    );
  }

  /**
   * Retrieves Users latest posts from server
   * @param userProfileIdParam `number`.
   * @returns `PostVM>`.
   */
  public getLatestPosts(
    userProfileIdParam: string
  ): Observable<Array<Post>> {
    var result = this._dataService.get(
      `${this._getLatestPostsApi}/${userProfileIdParam}`,
      {}); 

      return result;
  }

  /**
   * Retrieves a specific post's details from server
   * @param postIdParam `number`.
   * @returns `PostDetailsVM>`.
   */
  public getPostDetails(postIdParam: string): Observable<PostDetails> {
    return this._dataService.get(
      `${this._getPostDetailsApi}/${postIdParam}`,
      {});
  }

  /**
   * Retrieves the user's top posts from server
   * @returns `TopPostVM>`.
   */
  public getTopPosts (userProfileIdParam: string
  ): Observable<Array<TopPost>> {
    return this._dataService.get(
      `${this._getTopPostsApi}/${userProfileIdParam}`,
      {});
  }

  /**
   * Deletes a post
   * @param ideaIdParam `number`.
   */
  public delete(postIdParam: string) {
    return this._dataService.delete(
      `${this._deleteApi}/${postIdParam}`,
      {}
    ).subscribe(data  =>{           
      },
      error=>
      {
        window.alert(error.error.Message);
      }
    );
  }
}
