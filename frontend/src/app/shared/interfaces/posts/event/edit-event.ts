export interface EditEvent {
  id: string;

  content: string;

  publicationDate: Date;

  endDate: Date;

  targetGroupsIds: string[];

  pictureId: string;
}
