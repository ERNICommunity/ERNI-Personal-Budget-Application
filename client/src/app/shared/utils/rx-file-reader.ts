import { Observable } from 'rxjs';

/**
 * Read the text contents of a File or Blob using the FileReader interface.
 * This is an async interface, so it makes sense to handle it with Rx.
 * @return Observable<string>
 * @param blob
 */
export function readFile(blob: Blob): Observable<string | ArrayBuffer | null> {
  return new Observable((obs) => {
    if (!(blob instanceof Blob)) {
      obs.error(new Error('`blob` must be an instance of File or Blob.'));
      return;
    }

    const reader = new FileReader();

    reader.onerror = (err) => obs.error(err);
    reader.onabort = (err) => obs.error(err);
    reader.onload = () => {
      obs.next(reader.result);
      obs.complete();
    };

    reader.readAsDataURL(blob);
  });
}
