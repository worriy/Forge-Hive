
import { Observable } from 'rxjs';
import { PagingVM } from '../../src/viewModels/pagingVM';
import { PostVM } from '../../src/viewModels/postVM';
import { PostDetailsVM } from '../../src/viewModels/postDetailsVM';
import { TopPostVM } from '../../src/viewModels/topPostVM';
export class PostsControllerMock {
    public getLatestPosts(
        pagingParam: PagingVM,
        userProfileIdParam: number
    ): Observable<PostVM> {
      return Observable.of();
    }

    public getPostDetails(
        postIdParam: number
    ): Observable<PostDetailsVM> {
      return Observable.of();
    }

    public getTopPosts(
        userProfileIdParam: number
    ): Observable<TopPostVM> {
      return Observable.of();
    }

    public get(
        postIdParam: string
    ): Observable<PostVM> {
      return Observable.of();
    }

}
