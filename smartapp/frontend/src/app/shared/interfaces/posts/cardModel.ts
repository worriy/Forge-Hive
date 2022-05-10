import { UserProfile } from "../user/user-profile";
import { PictureModel } from "./pictureModel";
import { Choice } from "./choice";

export interface CardModel {
  id: number;

  publicationDate: Date;

  content: string;

  type: string;

  endDate: Date;

  createdById: number;

  pictureId: number;

  createdBy: UserProfile;

  picture: PictureModel;

  choices: Choice[];
}
