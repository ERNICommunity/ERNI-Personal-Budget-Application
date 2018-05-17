import { UserTag } from "./usertag";

export class User {
    id: number;
    isAdmin: boolean;
    firstName: string;
    lastName: string;
    superior: UserTag;
  } 