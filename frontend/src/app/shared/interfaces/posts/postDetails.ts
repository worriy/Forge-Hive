import { Result } from "./result";

export interface PostDetails {
  id: string;

  content: string;

  type: string;

  publicationDate: Date;

  endDate: Date;

  views: number;

  likes: number;

  answers: number;

  picture: string;

  targetGroups: string;

  status: string;

  results: Result[];
}
