
import { Observable } from 'rxjs';
import { CreateIdeaVM } from '../../src/viewModels/createIdeaVM';
import { EditableIdeaVM } from '../../src/viewModels/editableIdeaVM';
import { EditIdeaVM } from '../../src/viewModels/editIdeaVM';
import { PictureVM } from '../../src/viewModels/pictureVM';
export class IdeaControllerMock {
    public create(
        ideaParam: CreateIdeaVM
    ): Observable<any> {
      return Observable.of();
    }

    public getEditableIdea(
        ideaIdParam: number
    ): Observable<EditableIdeaVM> {
      return Observable.of();
    }

    public update(
        ideaParam: EditIdeaVM
    ): Observable<any> {
      return Observable.of();
    }

    public delete(
        ideaIdParam: number
    ): Observable<any> {
      return Observable.of();
    }

    public getDefaultPicture(
    ): Observable<PictureVM> {
      return Observable.of();
    }

}
