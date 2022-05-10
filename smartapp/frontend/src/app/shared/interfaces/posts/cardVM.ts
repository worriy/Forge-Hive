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

  isLiked: boolean;

  likes: number;

  answers: number;

  pictureId: string;

  picture: string;

  results: Result[];
}
