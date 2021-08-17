import { Result } from "./result";

export interface CardVM {
  id: string;

  linkedCardId: string;

  content: string;

  targetGroups: string;

  publicationDate: Date;

  endDate: Date;

  author: string;

  type: string;

  views: number;

  likes: number;

  answers: number;

  picture: string;

  results: Result[];
}
