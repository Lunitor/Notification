import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface CreateEmailState {
    isLoading: boolean;
    emailTypes: EmailType[];
    selectedEmailType: EmailType;
}

export interface EmailType {
    name: string;
    tags: Tag[];
}

export interface Tag {
    name: string;
    placeholder: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestEmailTypesAction {
    type: 'REQUEST_EMAIL_TYPES';
}

interface ReceiveEmailTypesAction {
    type: 'RECEIVE_EMAIL_TYPES';
    emailTypes: EmailType[];
}

interface SelectEmailTypeAction {
    type: 'SELECT_EMAIL_TYPE';
    selectedEmailType: EmailType;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestEmailTypesAction | ReceiveEmailTypesAction | SelectEmailTypeAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
    requestEmailTypes: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)
        const appState = getState();
        if (appState && appState.createEmail) {

            const byUserTags = new Array<Tag>();
            byUserTags.push({ name: "UserName", placeholder: "{USERNAME}" });
            byUserTags.push({ name: "test", placeholder: "{TEST}" });

            const commonTags = new Array<Tag>();
            commonTags.push({ name: "common", placeholder: "{USERCOMMONNAME}" });
            commonTags.push({ name: "test 2", placeholder: "{TEST2}" });


            const testEmailTypes = new Array<EmailType>();
            testEmailTypes.push({
                name: "byuser",
                tags: byUserTags
            })
            testEmailTypes.push({
                name: "common",
                tags: commonTags
            })

            //fetch(``)
            //    //.then(response => response.json())
            //    .then(data => {
            //        //dispatch({ type: 'RECEIVE_EMAIL_TYPES', emailTypes: data });
                    
            //        dispatch({ type: 'RECEIVE_EMAIL_TYPES', emailTypes: testEmailTypes })
            //    })
            //    .catch(exception => {
            //        dispatch({ type: 'RECEIVE_EMAIL_TYPES', emailTypes: testEmailTypes })
            //    });

            dispatch({ type: 'REQUEST_EMAIL_TYPES' });
            setTimeout(() =>
            {
                dispatch({ type: 'RECEIVE_EMAIL_TYPES', emailTypes: testEmailTypes })
            }, 500);
            
        }
    },

    selectEmailType: (selectedEmailType: EmailType): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'SELECT_EMAIL_TYPE', selectedEmailType: selectedEmailType });
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: CreateEmailState = {
    emailTypes: [], isLoading: false, selectedEmailType: { name: "", tags: [] }
};

export const reducer: Reducer<CreateEmailState> = (state: CreateEmailState | undefined, incomingAction: Action): CreateEmailState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_EMAIL_TYPES':
            return {
                selectedEmailType: state.selectedEmailType,
                emailTypes: state.emailTypes,
                isLoading: true
            };
        case 'RECEIVE_EMAIL_TYPES':
            return {
                selectedEmailType: action.emailTypes.length > 0 ? action.emailTypes[0]: state.selectedEmailType,
                emailTypes: action.emailTypes,
                isLoading: false
            };
        case 'SELECT_EMAIL_TYPE':
            return {
                selectedEmailType: action.selectedEmailType,
                emailTypes: state.emailTypes,
                isLoading: false
            }
        default: return state;
    }
};
