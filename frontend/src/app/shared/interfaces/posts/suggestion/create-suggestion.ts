export interface CreateSuggestion {
  content?: string;

  publicationDate?: Date;

  endDate?: Date;

  targetGroupsIds?: string[];

  authorId?: string;

  pictureId?: string;
}
