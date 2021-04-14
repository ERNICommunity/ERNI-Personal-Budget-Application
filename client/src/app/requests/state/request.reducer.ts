import { createReducer, on } from '@ngrx/store';
import { RequestApprovalState } from '../../model/request/requestState';
import * as RequestActions from './request.actions';

export interface RequestState {
    id: number;
    approvalState: RequestApprovalState;
}

const initialState: RequestState = {
    id: null,
    approvalState: RequestApprovalState.Request
}

export const requestCreateReducer = createReducer<RequestState>(
    initialState,
    on(RequestActions.pending, (state, action): RequestState => {
        console.log('original state:  ' + JSON.stringify(state));

        return {
            ...state,
            id: action.id,
            approvalState: RequestApprovalState.Request
        };
    })
)

export const requestInvoiceReducer = createReducer<RequestState>(
    initialState,
    on(RequestActions.invoice, (state): RequestState => {
        console.log('original state:  ' + JSON.stringify(state));

        return {
            ...state,
            approvalState: RequestApprovalState.Invoice
        };
    })
)

export const requestApproveReducer = createReducer<RequestState>(
    initialState,
    on(RequestActions.approve, (state): RequestState => {
        console.log('original state:  ' + JSON.stringify(state));

        return {
            ...state,
            approvalState: RequestApprovalState.Closed
        };
    })
)

export const requestRejectReducer = createReducer<RequestState>(
    initialState,
    on(RequestActions.reject, (state): RequestState => {
        console.log('original state:  ' + JSON.stringify(state));

        return {
            ...state,
            approvalState: RequestApprovalState.Closed
        };
    })
)