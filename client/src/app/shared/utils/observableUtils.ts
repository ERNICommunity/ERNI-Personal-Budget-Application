import { Observable } from 'rxjs';
import { scan } from 'rxjs/operators';

export interface ResetEvent<T extends { id: number }> {
  list: T[];
}

export interface RemoveEvent {
  id: number;
}

export function maintainList<T extends { id: number }>(
  opstream: Observable<ResetEvent<T> | RemoveEvent>,
): Observable<T[]> {
  return opstream.pipe(
    scan((acc, event) => {
      if ('list' in event) {
        return event.list;
      }

      return acc.filter((_) => _.id !== event.id);
    }, [] as T[]),
  );
}
