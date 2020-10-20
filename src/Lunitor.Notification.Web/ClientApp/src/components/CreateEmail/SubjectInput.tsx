import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../../store';
import * as CreateEmailStore from '../../store/CreateEmail';

// At runtime, Redux will merge together...
type CreateEmailProps =
    CreateEmailStore.CreateEmailState // ... state we've requested from the Redux store
    & typeof CreateEmailStore.actionCreators // ... plus action creators we've requested
    & RouteComponentProps; // ... plus incoming routing parameters

class CreateEmail extends React.PureComponent<CreateEmailProps> {

    public render() {
        return (
            <input
                id="subjectInput"
                type="text"
                value={this.props.emailSubject}
                onChange={this.onSubjectChange.bind(this)}
                className="form-control" />
        );
    }

    private onSubjectChange(event: React.ChangeEvent<HTMLInputElement>) {
        event.preventDefault();

        this.props.modifyEmailSubject(event.currentTarget.value);
    }
}

export default connect(
    (state: ApplicationState) => state.createEmail, // Selects which state properties are merged into the component's props
    CreateEmailStore.actionCreators // Selects which action creators are merged into the component's props
)(CreateEmail as any);

