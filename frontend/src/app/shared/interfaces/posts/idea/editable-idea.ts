export interface EditableIdea {
  id: string;

  content: string;

  publicationDate: Date;

  endDate: Date;

  targetGroupsIds: string[];
  
  picture: string;
}
