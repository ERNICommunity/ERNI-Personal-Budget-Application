import { UserState } from "./userState";

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  superior: User;
  state: UserState;
}
