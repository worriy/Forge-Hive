import { Result } from "./result";

export interface Report {
  id: number;

  content: string;

  author: string;

  views: number;

  likes: number;

  answers: number;

  results: Result[];
}
