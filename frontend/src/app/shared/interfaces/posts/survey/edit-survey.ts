import { SurveyQuestion } from "./survey-question";

export interface EditSurvey {
  id: number;

  content: string;

  publicationDate: Date;

  endDate: Date;

  targetGroupsIds: number[];

  pictureId: number;

  questions: SurveyQuestion[];
}
