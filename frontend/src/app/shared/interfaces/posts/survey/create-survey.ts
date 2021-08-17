import { CreateQuestion } from "../question/create-question";
import { SurveyQuestion } from "./survey-question";

export interface CreateSurvey {
  content: string;

  publicationDate: Date;

  endDate: Date;

  targetGroupsIds: number[];

  authorId: number;

  pictureId: number;

  questions: CreateQuestion[];
}
