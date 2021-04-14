import * as AppState from '../../state/app.state';
import { RequestState } from './request.reducer';

export interface State extends AppState.State {
    request: RequestState;
}