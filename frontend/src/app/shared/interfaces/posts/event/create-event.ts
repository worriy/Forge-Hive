export interface CreateEvent {
  content?: string;

  publicationDate?: Date;

  endDate?: Date;

  targetGroupsIds?: string[];

  authorId?: string;

  pictureId?: string;
}
