import { CardTypes } from "./card-types.enum";

export interface CreatePost {
  content?: string;
  publicationDate?: Date,
  endDate?: Date,
  targetGroupsIds?: string[];
  authorId?: string;
  pictureId?: string;
  choices?: string[];
  type?: CardTypes;
}