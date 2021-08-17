import { Choice } from "./choice";

export interface EditableQuestion {
  id: string;
  
  content: string;
  
  publicationDate: Date;
  
  endDate: Date;
 
  targetGroupsIds: string[];
  
  picture: string;

  choices: Choice[];
}
