import { Choice } from "./choice";
import { Reporting } from "./reporting";

export interface Result {
  id: number;

  value: number;

  choiceId: number;

  reportingId: number;

  choice: Choice;

  reporting: Reporting;
}
