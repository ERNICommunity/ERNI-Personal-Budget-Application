import { UserState } from "./userState";

export class User {
    id: number;
    isAdmin: boolean;
    firstName: string;
    lastName: string;
    superior: User;
    state: UserState;
  } 