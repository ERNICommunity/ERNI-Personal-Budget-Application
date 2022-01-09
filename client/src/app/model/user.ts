import { UserState } from "./userState";

export class User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  superior: User;
  state: UserState;
} 