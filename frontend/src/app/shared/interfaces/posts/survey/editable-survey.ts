import { SurveyQuestion } from "./survey-question";

export interface EditableSurvey {
  id: number;

  content: string;

  publicationDate: Date;

  endDate: Date;

  targetGroupsIds: number[];

  picture: string;

  questions: SurveyQuestion[];
}
