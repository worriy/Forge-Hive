export interface CreateQuestion {
  content?: string;

  publicationDate?: Date;

  endDate?: Date;

  targetGroupsIds?: string[];

  authorId?: string;
  
  pictureId?: string;

  choices?: String[];
}
