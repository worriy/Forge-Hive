export interface EditIdea {
  id: string;

  content: string;

  publicationDate: Date;

  endDate: Date;

  targetGroupsIds: string[];
  
  pictureId: string;
}
