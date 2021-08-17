import { Choice } from "./choice";

export interface EditQuestion {
  id: string;
  
  content: string;
  
  publicationDate: Date;
  
  endDate: Date;
  
  targetGroupsIds: string[];

  pictureId: string;

  choices: Choice[];
}
