import { toSignal } from "@angular/core/rxjs-interop";
import { Observable, map, catchError, of } from "rxjs";

export type Result<T> =
  | { status: "pending" }
  | { status: "success"; value: T }
  | { status: "error"; error: string };

export const toResultSignal = <T>(o: Observable<T>) =>
  toSignal(
    o.pipe(
      map((value) => ({ status: "success", value } as Result<T>)),
      catchError((error) =>
        of({ status: "error", error: error.error } as Result<T>)
      )
    ),
    { initialValue: { status: "pending" } as Result<T> }
  );

export const mapSuccessfulResult = <T, U>(
  o: Result<T> | null,
  f: (t: T) => U
) => (o?.status == "success" ? f(o.value) : null);

// UNUSED
export const mapIfNotNull =
  <T, U>(f: (t: T) => U) =>
  (t: T | null) =>
    t ? f(t) : null;

export const mapNullable = <T, U>(o: T | null, f: (t: T) => U) =>
  o ? f(o) : null;
