import { createAction, props } from "@ngrx/store";

const actionName: string = '[Request] Change Request State';

export const pending = createAction(
    actionName,
    props<{ id: number}>()
);

export const invoice = createAction(
    actionName
)

export const reject = createAction(
    actionName
)

export const approve = createAction(
    actionName
)