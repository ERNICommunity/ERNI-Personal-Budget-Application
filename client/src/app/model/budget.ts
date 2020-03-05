import { User } from "./user";
import {Request } from "./request/request";

export class Budget {
  id: number;
  year: number;
  amount: number;
  amountLeft: number;
  user: User;
  title: string;
  type: number;
  requests: Request[];
} 