import { Choice } from "./choice";

export interface EditPost {
  id: string;
  content: string;
  publicationDate: Date;
  endDate: Date;
  targetGroupsIds: string[];
  pictureId?: string;
  choices?: Choice[];
};
