export type StringDates<T> = {
  [Property in keyof T]: T[Property] extends Date
    ? string
    : T[Property] extends Date | null
    ? string | null
    : T[Property];
};

export const toIsoDate = (date: Date) => {
  let month = "" + (date.getMonth() + 1);
  let day = "" + date.getDate();
  const year = date.getFullYear();

  if (month.length < 2) month = "0" + month;
  if (day.length < 2) day = "0" + day;

  return [year, month, day].join("-");
};

export const toDate = (value: string | null) =>
  value ? new Date(value) : null;
