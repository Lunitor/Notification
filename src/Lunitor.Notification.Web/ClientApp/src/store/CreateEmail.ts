import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface CreateEmailState {
    isLoading: boolean;
    emailTypes: EmailType[];
    selectedEmailType: EmailType;
    emailBody: string;
    cursorPositionInEmailBody: number;
    emailSubject: string;
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

interface ModifyEmailBodyAction {
    type: 'MODIFY_EMAIL_BODY';
    emailBody: string;
}

interface UpdatecursorPositionInEmailBodyAction {
    type: 'UPDATE_CURSOR_POSITION_IN_EMAIL_BODY';
    cursorPosition: number;
}

interface ModifyEmailSubjectAction {
    type: 'MODIFY_EMAIL_SUBJECT';
    emailSubject: string;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestEmailTypesAction |
    ReceiveEmailTypesAction |
    SelectEmailTypeAction |
    ModifyEmailBodyAction |
    UpdatecursorPositionInEmailBodyAction |
    ModifyEmailSubjectAction;

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
    },

    modifyEmailBody: (emailBody: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'MODIFY_EMAIL_BODY', emailBody: emailBody })
    },

    updateCursorPositionInEmailBody: (cursorPosition: number): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'UPDATE_CURSOR_POSITION_IN_EMAIL_BODY', cursorPosition: cursorPosition })
    },

    modifyEmailSubject: (emailSubject: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        dispatch({ type: 'MODIFY_EMAIL_SUBJECT', emailSubject: emailSubject });
    }
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: CreateEmailState = {
    emailTypes: [],
    isLoading: false,
    selectedEmailType: { name: "", tags: [] },
    emailBody: "",
    cursorPositionInEmailBody: 0,
    emailSubject: ""
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
                isLoading: true,
                emailBody: state.emailBody,
                cursorPositionInEmailBody: state.cursorPositionInEmailBody,
                emailSubject: state.emailSubject
            };
        case 'RECEIVE_EMAIL_TYPES':
            return {
                selectedEmailType: action.emailTypes.length > 0 ? action.emailTypes[0]: state.selectedEmailType,
                emailTypes: action.emailTypes,
                isLoading: false,
                emailBody: state.emailBody,
                cursorPositionInEmailBody: state.cursorPositionInEmailBody,
                emailSubject: state.emailSubject
            };
        case 'SELECT_EMAIL_TYPE':
            return {
                selectedEmailType: action.selectedEmailType,
                emailTypes: state.emailTypes,
                isLoading: false,
                emailBody: state.emailBody,
                cursorPositionInEmailBody: state.cursorPositionInEmailBody,
                emailSubject: state.emailSubject
            };
        case 'MODIFY_EMAIL_BODY':
            return {
                selectedEmailType: state.selectedEmailType,
                emailTypes: state.emailTypes,
                isLoading: false,
                emailBody: action.emailBody,
                cursorPositionInEmailBody: state.cursorPositionInEmailBody,
                emailSubject: state.emailSubject
            };
        case 'UPDATE_CURSOR_POSITION_IN_EMAIL_BODY':
            return {
                selectedEmailType: state.selectedEmailType,
                emailTypes: state.emailTypes,
                isLoading: false,
                emailBody: state.emailBody,
                cursorPositionInEmailBody: action.cursorPosition,
                emailSubject: state.emailSubject
            };
        case "MODIFY_EMAIL_SUBJECT":
            return {
                selectedEmailType: state.selectedEmailType,
                emailTypes: state.emailTypes,
                isLoading: false,
                emailBody: state.emailBody,
                cursorPositionInEmailBody: state.cursorPositionInEmailBody,
                emailSubject: action.emailSubject
            };
        default: return state;
    }
};
