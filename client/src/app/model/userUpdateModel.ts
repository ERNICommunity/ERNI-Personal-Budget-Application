import { UserState } from "./userState";

export interface UserUpdateModel {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  superior: number;
}
