import { CardModel } from "./cardModel";
import { Result } from "./result";

export interface Reporting extends CardModel {
  views: number;

  answers: number;

  cardId: number;

  results: Result[];
}
